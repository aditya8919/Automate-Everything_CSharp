using EComerceProject.Libraries.Logger;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EComerceProject.Libraries
{
  public class HelperUtility : UtilityBase
  {

    /// <summary>
    /// Author : Kavita Nunse <26-Aug-2021>
    /// Desc : Method will kill all processes for given parameter
    /// </summary>
    /// <param name="processName">Process name which need to be killed</param>
    /// <returns>true if required process is killed, else returns false.</returns>
    public static bool KillProcessByName(string processName)
    {
      Process[] procs;
      procs = Process.GetProcessesByName(processName);
      foreach (Process proc in procs)
      {
        try
        {
          proc.Kill();
          Log.WriteLine(processName + " process has been killed.");
        }
        catch (Exception ex)
        {
          Log.WriteLine("Exception while killing the process :" + ex.Message);
          throw ex;
        }
      }
      procs = Process.GetProcessesByName(processName);
      if (procs.Length == 0)
        return true;
      else
        return false;
    }

    /// <summary>
    /// Get Testdata Path for all current project
    /// </summary>
    /// <returns></returns>
    public static string GetTestDataPath()
    {
      return GetCurrentProjectPath() + @"\TestDataDetails.xml";
    }

    /// <summary>
    /// Get solution path
    /// </summary>
    /// <returns></returns>
    public static string GetSolutionPath()
    {
      string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
      string solutionPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(baseDirectory)));
      int place = solutionPath.LastIndexOf('\\');
      return solutionPath.Remove(place, (solutionPath.Length - place));
    }

    /// <summary>
    /// Get config path for current project
    /// </summary>
    /// <returns></returns>
    public static string GetConfigPath()
    {
      return GetCurrentProjectPath() + @"\app.config";
    }

    /// <summary>
    /// Get current project path
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentProjectPath()
    {
      return Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
    }

    /// <summary>
    /// Method to Wait for the required process to start.
    /// </summary>
    /// <param name="processName">Process name</param>
    /// <param name="windowName">Window title of the process</param>
    /// <returns>returns: true if process is strated, else returns false.</returns>
    public static bool WaitForProcessToStart(string processName, string windowName)
    {
      Thread.Sleep(1000);
      if (Path.HasExtension(windowName))
      {
        string[] filePath = windowName.Split('.');
        windowName = filePath[0];
      }

      Stopwatch sw = new Stopwatch();
      sw.Start();

      Process[] procs;
      bool processFound = false;
      do
      {
        procs = Process.GetProcessesByName(processName);
        foreach (Process proc in procs)
        {
          if (proc.MainWindowTitle.Contains(windowName))
          {
            processFound = true;
            break;
          }
          Thread.Sleep(200);
        }
      }
      while (!processFound && sw.Elapsed < TimeSpan.FromSeconds(60));

      sw.Stop();

      if (processFound)
        Log.WriteLine(processName + " process is running.", Log.LogType.INFO);
      else
        Log.WriteLine(processName + " process not found.", Log.LogType.ERROR);

      return processFound;
    }


    /// <summary>
    /// Method to Wait for the required process to end.
    /// </summary>
    /// <param name="processName"></param>
    /// <param name="windowName"></param>
    /// <returns>true if required process ends, else return false.</returns>
    public static bool WaitForProcessToEnd(string processName, string windowName)
    {
      if (Path.HasExtension(windowName))
      {
        string[] filePath = windowName.Split('.');
        windowName = filePath[0];
      }

      Stopwatch sw = new Stopwatch();
      sw.Start();

      Process[] procs;
      bool processNotFound = false;
      do
      {
        procs = Process.GetProcessesByName(processName);
        foreach (Process proc in procs)
        {
          if (!(proc.MainWindowTitle.Contains(windowName)))
          {
            processNotFound = true;
            break;
          }
        }
      }
      while (!(processNotFound) && (sw.Elapsed < TimeSpan.FromSeconds(30)));

      sw.Stop();

      Thread.Sleep(1000);
      if (processNotFound)
        Log.WriteLine(processName + " process is closed.", Log.LogType.INFO);
      else
        Log.WriteLine(processName + " process is still running.", Log.LogType.ERROR);

      return processNotFound;
    }

    /// <summary>
    /// Method to wait until boolean condition to be true
    /// </summary>
    /// <param name="condition"></param>
    /// <returns>true if given condition is satisfied, else retuen false.</returns>
    public static bool WaitUntilConditionToBeTrue(bool condition)
    {
      bool testResult = false;
      Stopwatch sw = new Stopwatch();
      sw.Start();

      try
      {
        while (sw.Elapsed.TotalSeconds <= 60)
        {
          if (condition)
          {
            testResult = true;
            break;
          }
        }
        sw.Stop();
      }
      catch (Exception ex)
      {
        Log.WriteLine("Not able to wait until the condition is true." + ex.Message);
        Log.WriteLine(ex.StackTrace, Log.LogType.ERROR, true, true);
        return testResult;
      }
      return testResult;
    }
  }

  /// <summary>
  /// Class for create .csv file
  /// </summary>
  public class CSVWriteOperations : UtilityBase
  {
    public string fileName = string.Empty;
    private string columnHeader = string.Empty;
    public CSVWriteOperations(string filePath)
    {
      this.fileName = filePath;// ProjectPaths.PIV_TESTDATA_PATH + "\\InputData\\SampleData\\" + fileName + ".csv";
    }

    public void ColumnHeaders(List<string> columnHeaders)
    {
      foreach (string column in columnHeaders)
      {
        if (this.columnHeader == string.Empty)
          this.columnHeader = column;
        else
          this.columnHeader = string.Format("{0},{1}", this.columnHeader, column);
      }
    }

    public void Writeline(string line)
    {
      if (!File.Exists(this.fileName) && columnHeader != string.Empty)
        File.WriteAllText(this.fileName, this.columnHeader + Environment.NewLine);
      else if (!File.Exists(this.fileName))
        throw new Exception(string.Format("{0} - file is not existing", this.fileName));
      else if (columnHeader == string.Empty)
        throw new Exception("ColumnHeader of the file is not defined");
      File.AppendAllText(this.fileName, line + Environment.NewLine);
    }
  }

  public static class AttributeDescriptionHelper
  {
    /// <summary>
    /// Method to get description of Enum
    /// </summary>
    /// <param name="e"></param>
    /// <returns>attribute description in string format.</returns>
    public static string GetDescription(this Enum e)
    {
      var attribute =
          e.GetType()
              .GetTypeInfo()
              .GetMember(e.ToString())
              .FirstOrDefault(member => member.MemberType == MemberTypes.Field)
              .GetCustomAttributes(typeof(DescriptionAttribute), false)
              .SingleOrDefault()
              as DescriptionAttribute;

      //return attribute?.IsDefaultAttribute ?? e.ToString();
      return attribute?.ToString() ?? e.ToString();
    }
  }
}
