using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Xsl;
using System.Xml;
using NLog.Targets;
using NLog.Config;
using NLog;

namespace EComerceProject.Libraries.Logger
{
  public abstract class Log
  {
    #region Enums

    /// <summary>
    /// Available Log Types
    /// </summary>
    public enum LogType
    {
      /// <summary>
      /// Start
      /// </summary>
      START,

      /// <summary>
      /// End
      /// </summary>
      END,

      /// <summary>
      /// Pass
      /// </summary>
      PASS,

      /// <summary>
      /// Fail
      /// </summary>
      FAIL,

      /// <summary>
      /// Error
      /// </summary>
      ERROR,

      /// <summary>
      /// Info
      /// </summary>
      INFO,

      /// <summary>
      /// Image
      /// </summary>
      IMAGE,

      /// <summary>
      /// Skip
      /// </summary>
      SKIP,

      /// <summary>
      /// Inconclusive
      /// </summary>
      INCONCLUSIVE,

      /// <summary>
      /// Step
      /// </summary>
      STEP
    }

    /// <summary>
    /// Available Result Type For the Test Case
    /// </summary>
    protected enum Result
    {
      /// <summary>
      /// Passed
      /// </summary>
      Passed,

      /// <summary>
      /// Failed
      /// </summary>
      Failed,

      /// <summary>
      /// InConclusive
      /// </summary>
      Inconclusive,

      /// <summary>
      /// Other
      /// </summary>
      Other
    }

    /// <summary>
    /// Represents attributes for class initialize and class cleanup methods.
    /// </summary>
    public enum MethodAttributes
    {
      /// <summary>
      /// Represents class initialize attribute.
      /// </summary>
      ClassInitialize,

      /// <summary>
      /// Represents class cleanup attribute.
      /// </summary>
      ClassCleanup,

      /// <summary>
      /// Represents TestFixtureSetup attribute.
      /// </summary>
      TestFixtureSetup,

      /// <summary>
      /// Represents TestFixtureTearDown attribute.
      /// </summary>
      TestFixtureTearDown
    }

    #endregion Enums

    /// <summary> The location</summary>
    protected static string Location;

    /// <summary> The log name</summary>
    protected static string LogName = "Initialization";

    /// <summary>
    /// Time at which current statement is logged
    /// </summary>
    protected static DateTime CurrentTime;
    /// <summary>
    /// Time at which previous Log was done
    /// </summary>
    protected static DateTime? PreviousTimeStamp = null;
    /// <summary>
    /// Difference between last log and current log
    /// </summary>
    protected static double LogTimeDifference;

    /// <summary>
    /// Object representing screen shot capture mechanism
    /// </summary>
    protected ICaptureScreen CaptureScreen;

    /// <summary>Initializes a new instance of the <see cref="Log"/> class.</summary>
    public Log()
    {
      TestResultCollection = new Dictionary<string, TestSummary>();
    }

    /// <summary>
    /// Parameterized constructor for Log Class
    /// </summary>
    /// <param name="logLocation">The log location.</param>
    /// <param name="screen">Interface for screen shot mechanism</param>
    protected Log(string logLocation, ICaptureScreen screen)
        : this()
    {
      Location = logLocation;
      CaptureScreen = screen;
      SetExecutionSummaryLocation();
      ScreenShotPath = Path.Combine(Location, "ScreenShots");
      if (!FileUtility.FolderExists(ScreenShotPath))
      {
        FileUtility.CreateFolder(ScreenShotPath);
      }

      //Create Download Path
      _downloadPath = Path.Combine(Location, "Download");
      if (!FileUtility.FolderExists(DownloadPath))
      {
        FileUtility.CreateFolder(DownloadPath);
      }

      // Configuring Nlog Logging Mechanism
      var target = new FileTarget
      {
        Layout = "${message}",
        FileName = Location + "\\" + LogName + ".html",
        KeepFileOpen = false
      };
      SimpleConfigurator.ConfigureForTargetLogging(target, NLog.LogLevel.Info);

      StartLoggingHTMLFormatting(false);
    }

    /// <summary>
    /// Flag for inconclusive state
    /// </summary>
    protected bool isInconclusive = false;

    /// <summary>
    /// Image Log Switch on Fail Error
    /// </summary>
    protected static bool ImgLogSwitchOnFailError = false;

    /// <summary>
    /// Image Log Switch on all log types
    /// </summary>
    protected static bool ImgLogSwitchOnAllLogLypes = false;

    /// <summary>
    /// Get Logger Instance for the Current Class
    /// </summary>
    protected static NLog.Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// String For Logging Messages
    /// </summary>
    protected static String LoggerInfoStr = "";

    /// <summary>
    /// Path to Store the Screen Shot.
    /// </summary>
    public static string ScreenShotPath = string.Empty;

    /// <summary>
    /// Test Execution Summary Path
    /// </summary>
    protected static string _summaryPath;

    /// <summary>
    /// Path to Store the Downloaded file
    /// </summary>
    protected static string _downloadPath;

    public string DownloadPath
    {
      get { return _downloadPath; }
    }

    /// <summary>
    /// Test Execution XML Summary File Name
    /// </summary>
    public const string XMLSummaryFileName = "TestExecutionStatus.xml";

    /// <summary>
    /// Test case file path
    /// </summary>
    protected static string _testcaseFilePath;

    /// <summary>
    /// Taking assemblies to ignore log.
    /// </summary>
    protected static string[] assembliesToIgnoreLog = null;

    /// <summary>
    /// Test Case Start Time
    /// </summary>
    protected static DateTime TCStartTime;

    /// <summary>
    /// Test Case End Time
    /// </summary>
    protected static DateTime TCEndTime;

    /// <summary>
    /// Test Environment
    /// </summary>
    protected static string _testEnvironment = string.Empty;

    /// <summary>
    /// Test Environment
    /// </summary>
    public string TestEnvironment
    {
      get
      {
        return _testEnvironment;
      }
      set
      {
        _testEnvironment = value;
      }
    }

    /// <summary>
    /// This property will be accessed to get the Log folder location
    /// </summary>
    public string LogFolderPath
    {
      get { return Location; }
    }

    /// <summary>
    /// This Property Will Return the Summary Path of the Execution
    /// </summary>
    public string SummaryPath
    {
      get
      {
        return _summaryPath;
      }
    }

    /// <summary>
    /// The test result collection
    /// </summary>
    protected Dictionary<string, TestSummary> TestResultCollection;

    /// <summary>
    /// This property will be accessed to get the Log file location
    /// </summary>
    public string TestCaseFilePath
    {
      get { return _testcaseFilePath; }
    }

    /// <summary>
    ///  This Method is Used to set the Execution Summary and Deployment Path for any test execution
    /// </summary>
    private void SetExecutionSummaryLocation()
    {
      try
      {
        // Defining Folder Structure of the Log Location
        Location = Path.Combine(Location,
            "TestResults_" + TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now).ToString("MMddhssfff"));
        ConfigurationManager.AppSettings.Set("LogLocation", Location);

        // Set the Summary Path for any Test Execution
        if (string.IsNullOrEmpty(_summaryPath))
        {
          _summaryPath = Location;

          if (!FileUtility.FolderExists(_summaryPath))
            FileUtility.CreateFolder(_summaryPath);
        }
      }
      catch (Exception ex)
      {
        WriteLine("Exception occurred while setting the execution summary location -" + ex.ToString(), Log.LogType.ERROR);
        throw;
      }
    }
    /// <summary>
    /// This Method is Used to log End logging html messages with Proper Formatting
    /// </summary>
    /// <param name="currentTimeStamp">TimeStamp</param>
    /// <param name="path">Input Path</param>
    /// <param name="result">Result</param>
    /// <returns>Screen Shot Path If Captured</returns>
    protected string EndLoggingHtmlFormatting(string currentTimeStamp, string path, Result result)
    {
      try
      {
        // Log the Final Test Case Result
        switch (result)
        {
          case Result.Passed:
            LoggerInfoStr += "<tr  class='tdPass' >";
            LoggerInfoStr += "<td  class='tdPass' ><font color='green'>" + currentTimeStamp + "</font></td>";
            LoggerInfoStr += "<td  class='tdPass' ><font color='green'>" + "</font></td>";
            LoggerInfoStr += "<td  class='tdPass' ><font color='green'><B>TEST RESULT</B></font></td>";
            LoggerInfoStr += "<td  class='tdPass' ><font color='green'><B>PASSED</B></font></td>";
            LoggerInfoStr += "<td  class='tdPass' ><font color='green'>" + "</font></td></tr>";
            break;

          case Result.Failed:
            if (ImgLogSwitchOnFailError || ImgLogSwitchOnAllLogLypes)
            {
              path = CaptureScreen.GetSnapShot(ScreenShotPath);
              LoggerInfoStr += "<tr  class='tdFail' >";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + currentTimeStamp + "</font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "</font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>TEST RESULT</B></font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>FAILED</B></font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "<a href=" + path + "> <img src=" + path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            }
            else
            {
              LoggerInfoStr += "<tr  class='tdFail' >";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + currentTimeStamp + "</font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "</font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>TEST RESULT</B></font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>FAILED</B></font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "</font></td></tr>";
            }
            break;

          case Result.Inconclusive:
            LoggerInfoStr += "<tr  class='tdFail' >";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + currentTimeStamp + "</font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "</font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>TEST RESULT</B></font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>INCONCLUSIVE</B></font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "</font></td></tr>";
            break;

          default:
            if (ImgLogSwitchOnFailError || ImgLogSwitchOnAllLogLypes)
            {
              path = CaptureScreen.GetSnapShot(ScreenShotPath);
              LoggerInfoStr += "<tr  class='tdFail' >";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + currentTimeStamp + "</font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "</font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>TEST RESULT</B></font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>" + result + "</B></font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>" + "<a href=" + path + "> <img src=" + path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</B></font></td></tr>";
            }
            else
            {
              LoggerInfoStr += "<tr  class='tdFail' >";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + currentTimeStamp + "</font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "</font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>TEST RESULT</B>";
              LoggerInfoStr += "</font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>" + result + "</B></font></td>";
              LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>" + "</B></font></td></tr>";
            }
            break;
        }
      }
      catch (Exception ex)
      {
        WriteLine(string.Format("Exception thrown while ending log file in HTML format. Exception: {0}", ex), LogType.ERROR);
        throw ex;
      }
      return path;
    }

    /// <summary>
    /// This Method is Used to Log End result logging message for the Suite level logging
    /// </summary>
    /// <param name="methodAttribute">MethodAttributes</param>
    /// <param name="CurrentTimeStamp">CurrentTimeStamp</param>
    /// <param name="Path">ScreenShot Path</param>
    /// <param name="result">result</param>
    /// <returns>Screen Shot Path If Captured</returns>
    protected string EndSuiteLoggingHtmlFormatting(MethodAttributes methodAttribute, String CurrentTimeStamp, string Path, string result)
    {
      switch (result)
      {
        case "Passed":
          LoggerInfoStr += "<tr  class='tdPass' >";
          LoggerInfoStr += "<td  class='tdPass' ><font color='green'>" + CurrentTimeStamp + "</font></td>";
          LoggerInfoStr += "<td  class='tdPass' ><font color='green'>" + "</font></td>";
          LoggerInfoStr += "<td  class='tdPass' ><font color='green'><B>" + methodAttribute.ToString() + "</B></font></td>";
          LoggerInfoStr += "<td  class='tdPass' ><font color='green'><B>PASSED</B></font></td>";
          LoggerInfoStr += "<td  class='tdPass' ><font color='green'>" + "</font></td></tr>";
          break;

        case "Failed":
          if (ImgLogSwitchOnFailError || ImgLogSwitchOnAllLogLypes)
          {
            Path = CaptureScreen.GetSnapShot(ScreenShotPath);
            LoggerInfoStr += "<tr  class='tdFail' >";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + CurrentTimeStamp + "</font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "</font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>" + methodAttribute.ToString() + "</B></font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>FAILED</B></font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
          }
          else
          {
            LoggerInfoStr += "<tr  class='tdFail' >";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + CurrentTimeStamp + "</font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "</font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>" + methodAttribute.ToString() + "</B></font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>FAILED</B></font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "</font></td></tr>";
          }
          break;

        default:
          if (ImgLogSwitchOnFailError || ImgLogSwitchOnAllLogLypes)
          {
            Path = CaptureScreen.GetSnapShot(ScreenShotPath);
            LoggerInfoStr += "<tr  class='tdFail' >";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + CurrentTimeStamp + "</font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "</font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>" + methodAttribute.ToString() + "</B></font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>" + result + "</B></font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</B></font></td></tr>";
          }
          else
          {
            LoggerInfoStr += "<tr  class='tdFail' >";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + CurrentTimeStamp + "</font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'>" + "</font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>" + methodAttribute.ToString() + "</B>";
            LoggerInfoStr += "</font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>" + result + "</B></font></td>";
            LoggerInfoStr += "<td  class='tdFail' ><font color='red'><B>" + "</B></font></td></tr>";
          }
          break;
      }
      return Path;
    }

    /// <summary>
    /// This Function Will Change the Image Switch ON(true)/OFF(false)
    /// </summary>
    /// <param name="settingsFailError">settingsFailError</param>
    /// <param name="settingsAllLogType">settingsAllLogType</param>
    public static void OverrideImageSwitch(bool settingsFailError, bool settingsAllLogType)
    {
      ImgLogSwitchOnFailError = settingsFailError;
      ImgLogSwitchOnAllLogLypes = settingsAllLogType;
    }

    /// <summary>
    /// Gets calling assembly of current executing method
    /// </summary>
    /// <returns>Returns calling assembly.</returns>
    protected static Assembly GetCallingAssembly()
    {
      // Getting current executing assembly.
      Assembly currentAssembly = Assembly.GetExecutingAssembly();

      //Getting stack trace
      StackTrace stackTrace = new StackTrace();
      StackFrame[] stackFrames = stackTrace.GetFrames();

      // Getting first mismatch for currently executing assembly, which is calling assembly.
      foreach (StackFrame stackFrame in stackFrames)
      {
        Assembly callingAssembly = stackFrame.GetMethod().DeclaringType.Assembly;
        if (callingAssembly != currentAssembly)
          return callingAssembly;
      }
      return currentAssembly;
    }

    /// <summary>
    /// This Method will initialize the Logger
    /// </summary>
    /// <param name="LogLocation">Takes location to log.</param>
    /// <param name="CurrentTestMethod">Takes current test method name.</param>
    /// <param name="FileExists">Takes file exists as out param.</param>
    /// <param name="ignoreLogForAssemblies">Takes assemblies for ignore logging.</param>
    protected static void InitializeLogger(String LogLocation, String CurrentTestMethod, out bool FileExists, string ignoreLogForAssemblies)
    {
      string htmllogfileName = Path.Combine(LogLocation, CurrentTestMethod + ".html");

      Location = LogLocation;
      _testcaseFilePath = htmllogfileName;

      if (FileUtility.FileExists(htmllogfileName))
      {
        FileExists = true;
      }
      else
      {
        FileExists = false;
      }

      // Create Directory if it Does Not Exists
      if (!FileUtility.FolderExists(LogLocation))
      {
        FileUtility.CreateFolder(LogLocation);
      }

      ScreenShotPath = Path.Combine(LogLocation, "ScreenShots");
      if (!FileUtility.FolderExists(ScreenShotPath))
      {
        FileUtility.CreateFolder(ScreenShotPath);
      }

      // Configuring Nlog Logging Mechanism
      FileTarget target = new FileTarget();
      target.Layout = "${message}";
      target.FileName = htmllogfileName;
      target.KeepFileOpen = false;
      SimpleConfigurator.ConfigureForTargetLogging(target, NLog.LogLevel.Info);

      if (!string.IsNullOrEmpty(ignoreLogForAssemblies))
        assembliesToIgnoreLog = ignoreLogForAssemblies.Split(',');
    }

    /// <summary>
    /// This Method takes Care of the Formatting/JavaScript for starting the Logging for any test case
    /// </summary>
    /// <param name="FileExists">bool true if File Already Present Else False</param>
    protected static void StartLoggingHTMLFormatting(bool FileExists)
    {
      if (!FileExists)
      {
        LoggerInfoStr = "<Head><title>Execution Logs</title><script type=" + @"""text/javascript""" + ">function altRows(id){if (document.getElementsByTagName){var table = document.getElementsByName(id);for (var j = 0; j < table.length; j++){var rows = table[j].getElementsByTagName(" + @"""tr""" + ");for (i = 0; i < rows.length; i++){if (i % 2 == 0){rows[i].className = " + @"""evenrowcolor""" + ";}else {rows[i].className = " + @"""oddrowcolor""" + ";}}}}}window.onload = function () { altRows('datatable'); }</script><STYLE>table.altrowstable { font-family: verdana,arial,sans-serif; font-size:11px; color:#333333; border-width: 1px; border-color: #a9c6c9; border-collapse: collapse; width:100%;}table.altrowstable th { border-width: 1px; padding: 8px; border-style: solid; border-color: #a9c6c9;}table.altrowstable td { border-width: 1px; padding: 8px; border-style: solid; border-color: #a9c6c9;}.oddrowcolor{ background-color:#d4e3e5;}.evenrowcolor{background-color:#c3dde0;}</STYLE></Head>";
        LoggerInfoStr += "<form name=" + @"""datatable""" + "><table class=" + @"""altrowstable""" + " id=" + @"""alternatecolor""" + " >";
        LoggerInfoStr += "<tr  class='tdDefault' >";
        LoggerInfoStr += "<td  class='tdDefault' ><font color='blue'><B>TimeStamp</B></font></td>";
        LoggerInfoStr += "<td  class='tdDefault' ><font color='blue'><B>TimeDelta(in seconds)</B></font></td>";
        LoggerInfoStr += "<td  class='tdDefault' ><font color='blue'><B>Description</B></font></td>";
        LoggerInfoStr += "<td  class='tdDefault' ><font color='blue'><B>LogType</B></font></td>";
        LoggerInfoStr += "<td  class='tdDefault' ><font color='blue'><B>ScreenShots</B></font></td></tr>";
        Logger.Info(LoggerInfoStr);
      }
      else
      {
        LoggerInfoStr += "<form name=" + @"""datatable""" + "><table class=" + @"""altrowstable""" + " id=" + @"""alternatecolor""" + " >";
        LoggerInfoStr += "<tr  class='tdDefault' >";
        LoggerInfoStr += "<td  class='tdDefault' ><font color='blue'><B>TimeStamp</B></font></td>";
        LoggerInfoStr += "<td  class='tdDefault' ><font color='blue'><B>TimeDelta(in seconds)</B></font></td>";
        LoggerInfoStr += "<td  class='tdDefault' ><font color='blue'><B>Description</B></font></td>";
        LoggerInfoStr += "<td  class='tdDefault' ><font color='blue'><B>LogType</B></font></td>";
        LoggerInfoStr += "<td  class='tdDefault' ><font color='blue'><B>ScreenShots</B></font></td></tr>";
        Logger.Info(LoggerInfoStr);
      }
    }

    /// <summary>
    /// This Method is Use to Log Messages with Proper HTML Formatting
    /// </summary>
    /// <param name="logType">LogType</param>
    /// <param name="Message">Message</param>
    /// <param name="CurrentTimeStamp">CurrentTimeStamp</param>
    /// <param name="loggerInfo">loggerInfo</param>
    /// <param name="Path">Path</param>
    /// <param name="captureScreenShot">If set to true it will capture the screen shot else NOT in case of failed step</param>
    protected void LogMessagesHtmlFormatting(EComerceProject.Libraries.Logger.Log.LogType logType, String Message, String CurrentTimeStamp, ref String loggerInfo, ref string Path, bool captureScreenShot = true)
    {
      if (!ImgLogSwitchOnAllLogLypes)
      {
        switch (logType)
        {
          case LogType.PASS:
            loggerInfo += "<tr  class='tdPass'><td  class='tdPass'><font color='green'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdPass", "green");
            loggerInfo += "<td  class='tdPass'><font color='green'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdPass'><font color='green'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdPass'><font color='green'>" + "</font></td></tr>";
            break;

          case LogType.FAIL:
            if (ImgLogSwitchOnFailError && captureScreenShot)
            {
              Path = CaptureScreen.GetSnapShot(ScreenShotPath);
              loggerInfo += "<tr  class='tdFail' ><td  class='tdFail' ><font color='red'>" + CurrentTimeStamp + "</font></td>";
              TimeDifferenceLogging(ref loggerInfo, "tdFail", "red");
              loggerInfo += "<td  class='tdFail' ><font color='red'>" + Message + "</font></td>";
              loggerInfo += "<td  class='tdFail' ><font color='red'>" + logType + "</font></td>";
              loggerInfo += "<td  class='tdFail' ><font color='red'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            }
            else if (captureScreenShot)
            {
              Path = CaptureScreen.GetSnapShot(ScreenShotPath);
              loggerInfo += "<tr  class='tdFail' ><td  class='tdFail' ><font color='red'>" + CurrentTimeStamp + "</font></td>";
              TimeDifferenceLogging(ref loggerInfo, "tdFail", "red");
              loggerInfo += "<td  class='tdFail' ><font color='red'>" + Message + "</font></td>";
              loggerInfo += "<td  class='tdFail' ><font color='red'>" + logType + "</font></td>";
              loggerInfo += "<td  class='tdFail' ><font color='red'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            }
            else
            {
              loggerInfo += "<tr  class='tdFail' ><td  class='tdFail' ><font color='red'>" + CurrentTimeStamp + "</font></td>";
              TimeDifferenceLogging(ref loggerInfo, "tdFail", "red");
              loggerInfo += "<td  class='tdFail' ><font color='red'>" + Message + "</font></td>";
              loggerInfo += "<td  class='tdFail' ><font color='red'>" + logType + "</font></td>";
              loggerInfo += "<td  class='tdFail' ><font color='red'>" + "</font></td></tr>";
            }
            break;

          case LogType.INFO:
            loggerInfo += "<tr  class='tdInformation' ><td  class='tdInformation' ><font color='gray'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdInformation", "gray");
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + "</font></td></tr>";
            break;

          case LogType.STEP:
            loggerInfo += "<tr  class='tdInformation'><td  class='tdInformation'><font color='DodgerBlue'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdInformation", "DodgerBlue");
            loggerInfo += "<td  class='tdInformation'><font color='DodgerBlue'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdInformation'><font color='DodgerBlue'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdInformation'><font color='DodgerBlue'>" + "</font></td></tr>";
            break;

          case LogType.IMAGE:
            Path = CaptureScreen.GetSnapShot(ScreenShotPath);
            loggerInfo += "<tr  class='tdInformation' ><td  class='tdInformation' ><font color='gray'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdInformation", "gray");
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            break;

          case LogType.ERROR:
            if (ImgLogSwitchOnFailError)
            {
              Path = CaptureScreen.GetSnapShot(ScreenShotPath);
              loggerInfo += "<tr  class='tdException' ><td  class='tdException' ><font color='DarkRed'>" + CurrentTimeStamp + "</font></td>";
              TimeDifferenceLogging(ref loggerInfo, "tdException", "DarkRed");
              loggerInfo += "<td  class='tdException'><font color='DarkRed'>" + Message + "</font></td>";
              loggerInfo += "<td  class='tdException'><font color='DarkRed'>" + logType + "</font></td>";
              loggerInfo += "<td  class='tdException'><font color='DarkRed'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></font></td></tr>";
            }
            else
            {
              loggerInfo += "<tr  class='tdException' ><td  class='tdException' ><font color='DarkRed'>" + CurrentTimeStamp + "</font></td>";
              TimeDifferenceLogging(ref loggerInfo, "tdException", "DarkRed");
              loggerInfo += "<td  class='tdException'><font color='DarkRed'>" + Message + "</font></td>";
              loggerInfo += "<td  class='tdException'><font color='DarkRed'>" + logType + "</font></font></td>";
              loggerInfo += "<td  class='tdException'><font color='DarkRed'>" + "</font></font></td></tr>";
            }
            break;

          case LogType.SKIP:
            Path = CaptureScreen.GetSnapShot(ScreenShotPath);
            loggerInfo += "<tr  class='tdInformation' ><td  class='tdInformation' ><font color='red'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdInformation", "red");
            loggerInfo += "<td  class='tdInformation' ><font color='red'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='red'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='red'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            break;

          case LogType.INCONCLUSIVE:
            Path = CaptureScreen.GetSnapShot(ScreenShotPath);
            loggerInfo += "<tr  class='tdInformation' ><td  class='tdInformation' ><font color='red'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdInformation", "red");
            loggerInfo += "<td  class='tdInformation' ><font color='red'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='red'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='red'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            isInconclusive = true;
            break;

          default:
            loggerInfo += "<tr  class='tdInformation' ><td  class='tdInformation' ><font color='gray'>" + CurrentTimeStamp + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + "" + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + "</font></td></tr>";
            break;
        }
      }
      else
      {
        Path = CaptureScreen.GetSnapShot(ScreenShotPath);
        switch (logType)
        {
          case LogType.PASS:
            loggerInfo += "<tr  class='tdPass'><td  class='tdPass'><font color='green'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdPass", "green");
            loggerInfo += "<td  class='tdPass'><font color='green'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdPass'><font color='green'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdPass'><font color='green'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            break;

          case LogType.FAIL:
            loggerInfo += "<tr  class='tdFail' ><td  class='tdFail' ><font color='red'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdFail", "red");
            loggerInfo += "<td  class='tdFail' ><font color='red'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdFail' ><font color='red'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdFail' ><font color='red'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            break;

          case LogType.INFO:
            loggerInfo += "<tr  class='tdInformation' ><td  class='tdInformation' ><font color='gray'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdInformation", "gray");
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            break;

          case LogType.STEP:
            loggerInfo += "<tr  class='tdInformation'><td  class='tdInformation'><font color='DodgerBlue'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdInformation", "DodgerBlue");
            loggerInfo += "<td  class='tdInformation'><font color='DodgerBlue'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdInformation'><font color='DodgerBlue'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdInformation'><font color='DodgerBlue'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            break;

          case LogType.IMAGE:
            loggerInfo += "<tr  class='tdInformation' ><td  class='tdInformation' ><font color='gray'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdInformation", "gray");
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            break;

          case LogType.ERROR:
            loggerInfo += "<tr  class='tdException' ><td  class='tdException' ><font color='DarkRed'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdException", "DarkRed");
            loggerInfo += "<td  class='tdException'><font color='DarkRed'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdException'><font color='DarkRed'>" + logType + "</font></font></td>";
            loggerInfo += "<td  class='tdException'><font color='DarkRed'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></font></td></tr>";
            break;

          case LogType.SKIP:
            Path = CaptureScreen.GetSnapShot(ScreenShotPath);
            loggerInfo += "<tr  class='tdInformation' ><td  class='tdInformation' ><font color='red'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdInformation", "red");
            loggerInfo += "<td  class='tdInformation' ><font color='red'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='red'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='red'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            break;

          case LogType.INCONCLUSIVE:
            Path = CaptureScreen.GetSnapShot(ScreenShotPath);
            loggerInfo += "<tr  class='tdInformation' ><td  class='tdInformation' ><font color='red'>" + CurrentTimeStamp + "</font></td>";
            TimeDifferenceLogging(ref loggerInfo, "tdInformation", "red");
            loggerInfo += "<td  class='tdInformation' ><font color='red'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='red'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='red'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            isInconclusive = true;
            break;

          default:
            loggerInfo += "<tr  class='tdInformation' ><td  class='tdInformation' ><font color='gray'>" + CurrentTimeStamp + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + "" + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + Message + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + logType + "</font></td>";
            loggerInfo += "<td  class='tdInformation' ><font color='gray'>" + "<a href=" + Path + "> <img src=" + Path + " height=" + @"""75""" + " width=" + @"""100""" + "/></a>" + "</font></td></tr>";
            break;
        }
      }
    }

    /// <summary>
    /// Function to write final test summary report in XML format
    /// </summary>
    /// <param name="XMLFilePath">Destination file path</param>
    /// <param name="objTestSummary">TestSummary object</param>
    /// <returns>Returns true for successful operation</returns>
    protected bool WriteXMLSummary(string XMLFilePath, TestSummary objTestSummary)
    {
      try
      {
        //For the first time creating XML file
        if (!FileUtility.FileExists(XMLFilePath))
        {
          using (XmlWriter writer = XmlWriter.Create(XMLFilePath))
          {
            writer.WriteStartDocument();
            writer.WriteStartElement("TestExecutionStatus");
            writer.WriteEndElement();
            writer.WriteEndDocument();
          }
        }
        // Appending XML Node to XML file
        AddNode(XMLFilePath, objTestSummary);
        return true;
      }
      catch (Exception ex)
      {
        WriteLine("Exception occurred while writing the test summary xml file -" + ex.ToString(), Log.LogType.ERROR);
        throw;
      }
    }

    /// <summary>
    /// Function to add new node in specified XML file
    /// </summary>
    /// <param name="fileName">XML file name with path</param>
    /// <param name="objTestSummary">TestSummary object</param>
    protected void AddNode(string fileName, TestSummary objTestSummary)
    {
      XmlDocument _xmlDocument = null;
      try
      {
        using (XmlTextReader _xmlTextReader = new XmlTextReader(fileName))
        {
          _xmlDocument = new XmlDocument();
          _xmlDocument.Load(_xmlTextReader);
        }

        XmlElement tcLoggerElement = _xmlDocument.CreateElement("TCLogger");

        XmlElement osElement = _xmlDocument.CreateElement("OS");
        osElement.InnerText = objTestSummary.OS;
        tcLoggerElement.AppendChild(osElement);

        XmlElement machineNameElement = _xmlDocument.CreateElement("MachineName");
        machineNameElement.InnerText = objTestSummary.MachineName;
        tcLoggerElement.AppendChild(machineNameElement);

        XmlElement MachineRAMElement = _xmlDocument.CreateElement("MachineRAM");
        MachineRAMElement.InnerText = objTestSummary.MachineRAM;
        tcLoggerElement.AppendChild(MachineRAMElement);

        XmlElement machineProcessorElement = _xmlDocument.CreateElement("MachineProcessor");
        machineProcessorElement.InnerText = objTestSummary.MachineProcessor;
        tcLoggerElement.AppendChild(machineProcessorElement);

        XmlElement envElement = _xmlDocument.CreateElement("Env");
        envElement.InnerText = objTestSummary.Env;
        tcLoggerElement.AppendChild(envElement);

        XmlElement buildVersionElement = _xmlDocument.CreateElement("BuildVersion");
        buildVersionElement.InnerText = objTestSummary.BuildVersion;
        tcLoggerElement.AppendChild(buildVersionElement);

        XmlElement suiteNameElement = _xmlDocument.CreateElement("SuiteName");
        suiteNameElement.InnerText = objTestSummary.SuiteName;
        tcLoggerElement.AppendChild(suiteNameElement);

        XmlElement testMethodNameElement = _xmlDocument.CreateElement("TestMethodName");
        testMethodNameElement.InnerText = objTestSummary.TestMethodName;
        tcLoggerElement.AppendChild(testMethodNameElement);

        XmlElement testCaseResultElement = _xmlDocument.CreateElement("TestCaseResult");
        testCaseResultElement.InnerText = objTestSummary.TestCaseResult;
        tcLoggerElement.AppendChild(testCaseResultElement);

        XmlElement logPathElement = _xmlDocument.CreateElement("LogPath");
        logPathElement.InnerText = objTestSummary.LogPath;
        tcLoggerElement.AppendChild(logPathElement);

        XmlElement startTimeElement = _xmlDocument.CreateElement("StartTime");
        startTimeElement.InnerText = objTestSummary.StartTime;
        tcLoggerElement.AppendChild(startTimeElement);

        XmlElement endTimeElement = _xmlDocument.CreateElement("EndTime");
        endTimeElement.InnerText = objTestSummary.EndTime;
        tcLoggerElement.AppendChild(endTimeElement);

        XmlElement executionTimeElement = _xmlDocument.CreateElement("ExecutionTime");
        executionTimeElement.InnerText = objTestSummary.ExecutionTime;
        tcLoggerElement.AppendChild(executionTimeElement);

        _xmlDocument.DocumentElement.AppendChild(tcLoggerElement);
        _xmlDocument.Save(fileName);
      }
      catch (Exception ex)
      {
        WriteLine("Exception occurred while adding new node into the test summary xml file -" + ex.ToString(), Log.LogType.ERROR);
        throw;
      }
    }

    /// <summary>
    /// Function to generate final HTML summary report
    /// </summary>
    /// <returns>true if Report was Created Successfully else False</returns>
    protected bool GenerateSummaryReport()
    {
      try
      {
        string XMLPath = Path.Combine(SummaryPath, XMLSummaryFileName);

        // Get the xsl Embedded Resource from the Assembly
        string x = (from item in this.GetType().Assembly.GetManifestResourceNames()
                    where item.Contains("Transformer.xsl")
                    select item).First();

        using (Stream strm = this.GetType().Assembly.GetManifestResourceStream(x))
        using (XmlReader reader = XmlReader.Create(strm))
        {
          XslCompiledTransform transform = new XslCompiledTransform();
          transform.Load(reader);
          transform.Transform(XMLPath, SummaryPath + "\\Report.html");
        }
        return true;
      }
      catch (Exception ex)
      {
        WriteLine("Exception occurred while generating the html test summary " + ex.ToString(), Log.LogType.ERROR);
        throw;
      }
    }

    /// <summary>
    /// Asserts whether the given condition is false
    /// </summary>
    /// <param name="condition">condition to verify</param>
    /// <param name="conditionMessage">description of the condition</param>
    public abstract void AssertFalse(bool condition, string conditionMessage);

    /// <summary>
    /// Asserts whether the given condition is true
    /// </summary>
    /// <param name="condition">condition to verify</param>
    /// <param name="conditionMessage">description of the condition</param>
    public abstract void AssertTrue(bool condition, string conditionMessage);

    /// <summary>
    /// Asserts whether the given condition is Inconclusive
    /// </summary>
    /// <param name="conditionMessage">Description of the condition</param>
    /// <param name="stopTestExecution">Accepts bool value. Accepts True if test execution needs to be stopped immediately. False, if test execution needs to be continued. Default value is True</param>
    public abstract void AssertInconclusive(string conditionMessage, bool stopTestExecution = true);

    /// <summary>
    /// This Function will Log End logging Message and overall result For any Particular Test
    /// </summary>
    /// <param name="testCaseResult">Test Case Result</param>
    public abstract void EndLogging(object testCaseResult);

    /// <summary>
    /// This Method is Used to Log Different Messages with Image into the Log File
    /// </summary>
    /// <param name="Message">Message</param>
    public abstract void LogImage(string Message = "Image --->");

    /// <summary>
    /// This Method Will Initialize The Logging for any Test Method
    /// </summary>
    /// <param name="logLocation"></param> Location of the Output Log Folder
    /// <param name="testContext"></param> Currently Running Test Method available via TestContext
    /// <param name="ignoreLogForAssemblies">Assembly Names Comma Separated to Ignore Logging</param>
    public abstract void StartLogging(string logLocation, object testContext, string ignoreLogForAssemblies = "");

    /// <summary>
    /// Initializes the test summary.
    /// </summary>
    protected void InitializeTestSummary()
    {
      TestSummary test = new TestSummary
      {
        OS = OSChecker.GetOSFriendlyName(OSChecker.RunningPlatform()),
        MachineName = Environment.MachineName,
        MachineRAM = OSChecker.GetMachineRAM(),
        MachineProcessor = OSChecker.GetMachineProcessor(),
        BuildVersion = ""
      };
      // Storing info for the test

      if (!TestResultCollection.ContainsKey(LogName))
      {
        test.StartTime = !string.IsNullOrEmpty(TCStartTime.ToString()) ? TCStartTime.ToString() : DateTime.Now.ToString();

        test.TestCaseResult = "In Progress";

        TestResultCollection[LogName] = test;
      }
    }

    /// <summary>
    /// This Method is Used to Log Different Messages into the Log File
    /// </summary>
    /// <param name="Message"></param>Message to Log in the Logger
    /// <param name="logType"></param>Specify the Log Type
    /// <param name="displayMessageInBold">is this is set to true then message will be displayed in bold in logs</param>
    /// <param name="captureScreenShot">Is set to true it will capture the screen shot else NOT in case of failed step</param>
    public abstract void WriteLine(string Message, LogType logType = LogType.INFO, bool displayMessageInBold = false, bool captureScreenShot = true);

    /// <summary>
    /// Method used to start logging for class initialize or class cleanup methods.
    /// </summary>
    /// <param name="logLocation">Takes location to store log.</param>
    /// <param name="className">Takes class name as string.</param>
    /// <param name="methodAttribute">Takes object of MethodAttributes enum. </param>
    /// <param name="ignoreLogForAssemblies">Takes comma separated assembly names to ignore logging. </param>
    public abstract void StartLogging(String logLocation, string className, MethodAttributes methodAttribute, string ignoreLogForAssemblies = "");

    /// <summary>
    /// This Function will Log End logging Message and overall result For class initialize and class cleanup
    /// </summary>
    /// <param name="className">Takes class name as string.</param>
    /// <param name="methodAttribute">Takes object of MethodAttributes enum.</param>
    /// <param name="resultFlag">Takes final result as "Passed" OR "Failed".</param>
    public abstract void EndLogging(string className, MethodAttributes methodAttribute, bool resultFlag);

    /// <summary>
    /// This Method is Used to log test summary details
    /// </summary>
    /// <param name="currentTest">The current test.</param>
    /// <param name="testContext">Currently executing test case/ test method context</param>
    public virtual void LogTestSummary(string currentTest, object testContext)
    {
      if (!string.IsNullOrEmpty(TCEndTime.ToString()))
      {
        TestResultCollection[currentTest].EndTime = TCEndTime.ToString();
      }
      else
      {
        TestResultCollection[currentTest].EndTime = DateTime.Now.ToString();
      }

      int totalSeconds = (int)(TCEndTime - TCStartTime).TotalSeconds;
      TestResultCollection[currentTest].ExecutionTime = totalSeconds.ToString();
    }

    /// <summary>
    /// Method to log the time difference between 2 consecutive log statements
    /// </summary>
    /// <param name="classValue">string classvalue</param>
    /// <param name="fontColor">fontcolor</param>
    /// <param name="loggerInfo">loggerInfo</param>
    private void TimeDifferenceLogging(ref String loggerInfo, string classValue, string fontColor)
    {
      if (PreviousTimeStamp == null)
      {
        loggerInfo += "<td  class=" + classValue + "><font color=" + fontColor + ">" + "</font></td>";
        PreviousTimeStamp = CurrentTime;
      }
      else
      {
        DateTime previousTime = (DateTime)PreviousTimeStamp;
        LogTimeDifference = Convert.ToDouble(string.Format("{0:0.00}", (CurrentTime - previousTime).TotalSeconds));
        PreviousTimeStamp = CurrentTime;
        if (LogTimeDifference > 30.00)
          loggerInfo += "<td  class=" + classValue + "><font color='orange'>" + LogTimeDifference + "</font></td>";
        else
          loggerInfo += "<td  class=" + classValue + "><font color=" + fontColor + ">" + LogTimeDifference + "</font></td>";
      }
    }

  }

  /// <summary>
  /// This Class Represents the XML Nodes for Every Executed Test Case
  /// </summary>
  public class TestSummary
  {
    /// <summary>
    /// OS details
    /// </summary>
    public string OS { get; set; }

    /// <summary>
    /// Machine Name
    /// </summary>
    public string MachineName { get; set; }

    /// <summary>
    /// Machine Ram
    /// </summary>
    public string MachineRAM { get; set; }

    /// <summary>
    /// Machine Processor
    /// </summary>
    public string MachineProcessor { get; set; }

    /// <summary>
    /// Test Environment
    /// </summary>
    public string Env { get; set; }

    /// <summary>
    /// Build Version
    /// </summary>
    public string BuildVersion { get; set; }

    /// <summary>
    /// Test Suite Name
    /// </summary>
    public string SuiteName { get; set; }

    /// <summary>
    /// Test Case Name
    /// </summary>
    public string TestMethodName { get; set; }

    /// <summary>
    /// Test Case Result
    /// </summary>
    public string TestCaseResult { get; set; }

    /// <summary>
    /// Relative Log Path
    /// </summary>
    public string LogPath { get; set; }

    /// <summary>
    /// Test Case Start Time
    /// </summary>
    public string StartTime { get; set; }

    /// <summary>
    /// Test Case End Time
    /// </summary>
    public string EndTime { get; set; }

    /// <summary>
    /// Total Execution Time of Test Case
    /// </summary>
    public string ExecutionTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [execution completed].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [execution completed]; otherwise, <c>false</c>.
    /// </value>
    public bool ExecutionCompleted { get; set; }
  }
}
