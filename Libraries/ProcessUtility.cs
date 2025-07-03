using EComerceProject.Libraries.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComerceProject.Libraries
{
  /// <summary>
  /// Class related to Process.
  /// </summary>
  public class ProcessUtility : UtilityBase
  {
    /// <summary>
    /// Object of IProcess
    /// </summary>
    public static IProcess iProcess = new ProcessHelperWrapper();

    /// <summary>
    /// Launches the specified app path.
    /// </summary>
    /// <param name="appPath">The app path.</param>
    public static bool Launch(string appPath)
    {
      Process process = null;
      try
      {
        process = Process.Start(appPath);
        Log.WriteLine("Launched the Application : " + process.ProcessName);
        return IsLaunched(process.ProcessName);
      }
      catch (Win32Exception ex)
      {
        Log.WriteLine("Unable to launch the Application: " + ex.Message, Log.LogType.ERROR);
        return false;
      }
      catch (Exception ex)
      {
        Log.WriteLine("Unable to launch the Application:" + process.ProcessName + " due to Error : " + ex.Message, Log.LogType.ERROR);
        return false;
      }
    }


    /// <summary>
    /// Determines whether the specified process is launched or NOT.
    /// </summary>
    /// <param name="processName">Name of the process.</param>
    /// <returns>
    ///   <c>true</c> if the specified process name is launched; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsLaunched(string processName)
    {
      try
      {
        Log.WriteLine("Checking Process launched or NOT");
        Process[] processes = iProcess.GetProcessesByName(processName);

        if (processes.Count() == 0)
        {
          //Log.WriteLine("Process: " + processName + " is NOT launched");
          return false;
        }
        else
        {
          UtilityBase.Log.WriteLine("Process: " + processName + " is launched");
          return true;
        }
      }
      catch (Exception ex)
      {
        Log.WriteLine("The Process: " + processName + " is NOT launched due to Error: " + ex.Message, Log.LogType.ERROR);
        return false;
      }
    }


  }
  /// <summary>
  /// Process interface
  /// </summary>
  public interface IProcess
  {
    /// <summary>
    /// Creates an array of new System.Diagnostics.Process components and associates them with all the process
    /// resources on the local computer that share the specified process name
    /// </summary>
    /// <param name="processName">Friendly name of the process</param>
    /// <returns>An array of type System.Diagnostics.Process that represents the process resources
    /// running the specified application or file</returns>
    Process[] GetProcessesByName(string processName);
  }

  /// <summary>
  /// Wrapper to Process Helper class
  /// </summary>
  public class ProcessHelperWrapper : IProcess
  {
    /// <summary>
    /// Creates an array of new System.Diagnostics.Process components and associates them with all the process
    /// resources on the local computer that share the specified process name
    /// </summary>
    /// <param name="processName">Friendly name of the process</param>
    /// <returns>An array of type System.Diagnostics.Process that represents the process resources
    /// running the specified application or file</returns>
    public Process[] GetProcessesByName(string processName)
    {
      return Process.GetProcessesByName(processName);
    }
  }
}
