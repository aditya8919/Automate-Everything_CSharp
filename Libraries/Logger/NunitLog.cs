using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Web;

namespace EComerceProject.Libraries.Logger
{
  /// <summary>
  /// This Class is Used to Log using NUnit testing Framework
  /// </summary>
  public class NUnitLog : Log
  {
    // The constructor is defined private in nature to restrict access.
    private NUnitLog(string logLocation, ICaptureScreen screen)
        : base(logLocation, screen)
    {
    }

    private static NUnitLog _instance;

    /// <summary>
    /// Returns logger object
    /// </summary>
    /// <param name="logLocation">The log location.</param>
    /// <param name="iCaptureScreen">Interface representing screen shot mechanism</param>
    /// <returns> Logger object</returns>
    public static Log Instance(string logLocation, ICaptureScreen iCaptureScreen)
    {
      return _instance ?? (_instance = new NUnitLog(logLocation, iCaptureScreen));
    }

    private static TestContext _nUnitTestContext;

    /// <summary>
    /// This Method Will Initialize The Logging for any Test Method
    /// </summary>
    /// <param name="logLocation"></param> Location of the Output Log Folder
    /// <param name="testContext"></param> Currently Running Test Method available via TestContext
    /// <param name="ignoreLogForAssemblies">Assembly Names Comma Separated to Ignore Logging</param>
    public override void StartLogging(String logLocation, object testContext, string ignoreLogForAssemblies = "")
    {
      bool FileExists = false;
      _nUnitTestContext = (TestContext)testContext;
      LogName = _nUnitTestContext.Test.Name;
      TCStartTime = DateTime.Now;

      InitializeTestSummary();

      if (LoggerInfoStr != "")
      {
        LoggerInfoStr = String.Empty;
      }

      // Configure The Logging For the Current Running Test Method
      InitializeLogger(logLocation, _nUnitTestContext.Test.Name, out FileExists, ignoreLogForAssemblies);
      Log.PreviousTimeStamp = TCStartTime;
      StartLoggingHTMLFormatting(FileExists);

      LogMessage(LogType.START, "START : [" + _nUnitTestContext.Test.Name + "]");

      OSChecker.Platform CurrentPlatform = OSChecker.RunningPlatform();
      string OSName = OSChecker.GetOSFriendlyName(CurrentPlatform);
      WriteLine(string.Format("Operating System - {0}", OSName));
    }

    /// <summary>
    /// This Method is Used to Log Different Messages assembly wise into the Log File
    /// </summary>
    /// <param name="logType"></param>Specify the Log Type
    /// <param name="Message"></param>Message to Log in the Logger
    /// <param name="displayMessageInBold">is this is set to true then message will be displayed in bold in logs</param>
    /// <param name="captureScreenShot">Is set to true it will capture the screen shot else NOT in case of failed step</param>
    private void LogMessage(LogType logType, String Message, bool displayMessageInBold = false, bool captureScreenShot = true)
    {
      // Current Date Time
      CurrentTime = DateTime.Now;
      String CurrentTimeStamp = CurrentTime.ToString();
      String loggerInfo = String.Empty;

      // Flag for logging to be done or not.
      bool flagForLogging = true;

      //Replacing curly braces with square braces as TestContext throws Format Exception
      Message = Message.Replace("{", "[").Replace("}", "]");
      Message = HttpUtility.HtmlEncode(Message);
      Message = Message.Replace("\r\n", "<br/>");

      if (displayMessageInBold)
        Message = "<B>" + Message + "</B>";

      string Path = string.Empty;

      LogMessagesHtmlFormatting(logType, Message, CurrentTimeStamp, ref loggerInfo, ref Path, captureScreenShot);

      if (assembliesToIgnoreLog != null)
      {
        Assembly assemblyToIgnoreLog = GetCallingAssembly();
        foreach (var item in assembliesToIgnoreLog)
        {
          if (string.Equals(assemblyToIgnoreLog.GetName().Name, item))
          {
            flagForLogging = false;
            break;
          }
        }
      }
      if (flagForLogging)
      {
        Logger.Info(loggerInfo);
      }
    }

    /// <summary>
    /// This Method is Used to Log Different Messages with Image into the Log File
    /// </summary>
    /// <param name="Message">Message</param>
    public override void LogImage(string Message = "Image --->")
    {
      LogMessage(LogType.IMAGE, Message);
    }

    /// <summary>
    /// This Method is Used to Log Different Messages into the Log File
    /// </summary>
    /// <param name="Message"></param>Message to Log in the Logger
    /// <param name="logType"></param>Specify the Log Type
    /// <param name="displayMessageInBold">is this is set to true then message will be displayed in bold in logs</param>
    /// <param name="captureScreenShot">Is set to true it will capture the screen shot else NOT in case of failed step</param>
    public override void WriteLine(String Message, LogType logType = LogType.INFO, bool displayMessageInBold = false, bool captureScreenShot = true)
    {
      LogMessage(logType, Message, displayMessageInBold, captureScreenShot);
    }

    /// <summary>
    /// Asserts whether the given condition is true
    /// </summary>
    /// <param name="condition">condition to verify</param>
    /// <param name="conditionMessage">description of the condition</param>
    public override void AssertTrue(bool condition, string conditionMessage)
    {
      if (condition)
        this.WriteLine(conditionMessage, LogType.PASS);
      else
        this.WriteLine(conditionMessage, LogType.FAIL);

      Assert.That(condition, string.Format("{0}: {1}", LogType.FAIL, conditionMessage));
    }

    /// <summary>
    /// Asserts whether the given condition is false
    /// </summary>
    /// <param name="condition">condition to verify</param>
    /// <param name="conditionMessage">description of the condition</param>
    public override void AssertFalse(bool condition, string conditionMessage)
    {
      if (!condition)
        this.WriteLine(conditionMessage, LogType.PASS);
      else
        this.WriteLine(conditionMessage, LogType.FAIL);

      Assert.That(condition, string.Format("{0}: {1}", LogType.FAIL, conditionMessage));
    }

    /// <summary>
    /// Asserts whether the given condition is Inconclusive
    /// </summary>
    /// <param name="conditionMessage">Description of the condition</param>
    /// <param name="stopTestExecution">Accepts bool value. Accepts True if test execution needs to be stopped immediately. False, if test execution needs to be continued. Default value is True</param>
    public override void AssertInconclusive(string conditionMessage, bool stopTestExecution = true)
    {
      try
      {
        this.WriteLine(conditionMessage, LogType.INCONCLUSIVE);
        Assert.Inconclusive(conditionMessage);
        //Assert.Inconclusive(conditionMessage, string.Format("{0}: {1}", LogType.INCONCLUSIVE, conditionMessage));
      }
      catch (InconclusiveException expInconclusive)
      {
        //Assert.Inconclusive always throw a InconclusiveException by default
        isInconclusive = true;

        if (stopTestExecution)
        {
          throw expInconclusive;
        }
      }
    }

    /// <summary>
    /// This Function will Log End logging Message and overall result For any Particular Test
    /// </summary>
    /// <param name="testCaseResultobj">Test Case Result</param>
    public override void EndLogging(object testCaseResultobj)
    {
      TestContext TestContextObj = (TestContext)testCaseResultobj;
      TCEndTime = DateTime.Now;

      // Log test result summary for report
      LogTestSummary(LogName, TestContextObj);

      // Get the Result From Test Context
      TestStatus testCaseResult = TestContextObj.Result.Outcome.Status;
      //ResultState testCaseResult = TestContextObj.Result.Outcome.Status;

      // Current Date Time
      String CurrentTimeStamp = DateTime.Now.ToString();
      string Path = string.Empty;

      if (LoggerInfoStr != "")
      {
        LoggerInfoStr = String.Empty;
      }

      Result result;
      switch (testCaseResult)
      {
        case TestStatus.Passed:
          //Verifying if any of the step was inconclusive. If yes, then entire test result should be set as InConclusive
          if (isInconclusive)
          {
            result = Result.Inconclusive;
          }
          else
          {
            result = Result.Passed;
          }
          break;

        case TestStatus.Failed:
          //Verifying if any of the step was inconclusive. If yes, then entire test result should be set as InConclusive
          if (isInconclusive)
          {
            result = Result.Inconclusive;
          }
          else
          {
            result = Result.Failed;
          }
          break;

        case TestStatus.Inconclusive:
          result = Result.Inconclusive;
          break;

        default:
          result = Result.Other;
          break;
      }

      // Log the Final Test Case Result
      Path = EndLoggingHtmlFormatting(CurrentTimeStamp, Path, result);

      LogMessage(LogType.END, "END : [" + _nUnitTestContext.Test.Name + "]");
      LoggerInfoStr += "</table></form>";
      Logger.Info(LoggerInfoStr);
      _nUnitTestContext = null;

      if (result.Equals(Result.Inconclusive))
      {
        isInconclusive = false;
        Assert.Inconclusive(string.Format("Test - {0} is identified as Inconclusive", LogName));
      }
    }

    /// <summary>
    /// Method used to start logging for TestFixtureSetup or TextFixtureTearDown methods.
    /// </summary>
    /// <param name="logLocation">Takes location to store log.</param>
    /// <param name="className">Takes class name as string.</param>
    /// <param name="methodAttribute">Takes object of MethodAttributes enum. </param>
    /// <param name="ignoreLogForAssemblies">Takes comma separated assembly names to ignore logging. </param>
    public override void StartLogging(String logLocation, string className, MethodAttributes methodAttribute, string ignoreLogForAssemblies = "")
    {
      bool FileExists = false;
      if (LoggerInfoStr != "")
      {
        LoggerInfoStr = String.Empty;
      }

      // Configure The Logging For the Current Running Test Method
      InitializeLogger(logLocation, className + methodAttribute.ToString(), out FileExists, ignoreLogForAssemblies);
      Log.PreviousTimeStamp = DateTime.Now;
      StartLoggingHTMLFormatting(FileExists);
      LogMessage(LogType.START, "START : [" + className + methodAttribute.ToString() + "]");
    }

    /// <summary>
    /// This Function will Log End logging Message and overall result For TestFixtureSetup or TextFixtureTearDown
    /// </summary>
    /// <param name="className">Takes class name as string.</param>
    /// <param name="methodAttribute">Takes object of MethodAttributes enum.</param>
    /// <param name="resultFlag">Takes final result as "Passed" OR "Failed".</param>
    public override void EndLogging(string className, MethodAttributes methodAttribute, bool resultFlag)
    {
      try
      {
        // Current Date Time
        String CurrentTimeStamp = DateTime.Now.ToString();
        string Path = string.Empty;

        string result = resultFlag ? "Passed" : "Failed";

        if (LoggerInfoStr != "")
        {
          LoggerInfoStr = String.Empty;
        }

        // Log the Final Test Case Result
        Path = EndSuiteLoggingHtmlFormatting(methodAttribute, CurrentTimeStamp, Path, result);

        LogMessage(LogType.END, "END : [" + className + methodAttribute.ToString() + "]");
        LoggerInfoStr += "</table></form>";
        Logger.Info(LoggerInfoStr);
      }
      catch (Exception ex)
      {
        WriteLine(string.Format("Exception encountered while ending log. Exception info: {0}", ex), LogType.ERROR);
      }
      finally
      {
        if (isInconclusive)
        {
          //resetting the flag to false
          isInconclusive = false;
          Assert.Inconclusive(string.Format("Test - {0} is identified as Inconclusive", LogName));
        }
      }
    }

    /// <summary>
    /// This Method is Used to log test summary details
    /// </summary>
    /// <param name="currentTest">The current test.</param>
    /// <param name="_testContext">Currently executing test case/ test method context</param>
    public override void LogTestSummary(string currentTest, object _testContext)
    {
      TestContext testContext = (TestContext)_testContext;

      try
      {
        base.LogTestSummary(LogName, _testContext);

        // Add test Info in the Test Summary Object

        TestResultCollection[currentTest].SuiteName = testContext.Test.FullName.Split('.')[testContext.Test.FullName.Split('.').Length - 2];
        TestResultCollection[currentTest].TestMethodName = testContext.Test.Name;

        // Storing the Result in the Test Summary Object 
        switch (testContext.Result.Outcome.Status.ToString().ToLower())
        {
          case "passed":
            if (isInconclusive)
            {
              TestResultCollection[currentTest].TestCaseResult = Result.Inconclusive.ToString();
            }
            else
            {
              TestResultCollection[currentTest].TestCaseResult = Result.Passed.ToString();
            }
            break;
          case "failed":
            if (isInconclusive)
            {
              TestResultCollection[currentTest].TestCaseResult = Result.Inconclusive.ToString();
            }
            else
            {
              TestResultCollection[currentTest].TestCaseResult = Result.Failed.ToString();
            }
            break;
          case "inconclusive":
            TestResultCollection[currentTest].TestCaseResult = Result.Inconclusive.ToString();
            break;
          default:
            TestResultCollection[currentTest].TestCaseResult = Result.Other.ToString();
            break;
        }

        // Changing Absolute LogPath to Relative LogPath
        string LogPath = this.LogFolderPath + "\\" + TestResultCollection[currentTest].TestMethodName + ".html";
        //LogPath = LogPath.Substring(LogPath.IndexOf(TestResultCollection[currentTest].SuiteName));
        //LogPath = ".\\" + LogPath;
        TestResultCollection[currentTest].LogPath = LogPath;
        TestResultCollection[currentTest].Env = TestEnvironment;

        // Generate XML for summary report after every test
        string XMLPath = Path.Combine(SummaryPath, XMLSummaryFileName);
        WriteXMLSummary(XMLPath, TestResultCollection[currentTest]);
        // Generate HTML Report
        GenerateSummaryReport();
      }
      catch (Exception ex)
      {
        WriteLine(string.Format("Exception occurred while storing the information for the test -{0} in TestSummary object -", testContext.Test.Name) + ex.ToString(), Log.LogType.ERROR);
        throw;
      }
    }
  }
}
