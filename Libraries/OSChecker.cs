using EComerceProject.Libraries.Logger;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace EComerceProject.Libraries
{
  /// <summary>
  /// OS Version Detection
  /// </summary>
  public class OSChecker : UtilityBase
  {
    /// <summary>
    /// Returns true if Windows 64-bit Environment else it will return
    /// false.
    /// </summary>
    /// <returns></returns>
    public static bool Is64()
    {
      return IntPtr.Size == 8 ? true : false;
    }

    /// <summary>
    /// Return true if Windows 8 or newer
    /// </summary>
    /// <returns></returns>
    public static bool IsWindows8OrNewer()
    {
      var os = Environment.OSVersion;
      return os.Platform == PlatformID.Win32NT &&
      (os.Version.Major > 6 || (os.Version.Major == 6 && os.Version.Minor >= 2));
    }

    /// <summary>
    /// Return true if Windows 7
    /// </summary>
    /// <returns></returns>
    public static bool IsWindows7()
    {
      var os = Environment.OSVersion;
      return (os.Version.Build >= 7600 || (os.Version.Major == 6 && os.Version.Minor >= 1));
    }

    /// <summary>
    /// Return true if Windows XP
    /// </summary>
    /// <returns></returns>
    public static bool IsWindowsXP()
    {
      return Environment.OSVersion.Version.ToString() == "5.1.2600.0" ? true : false;
    }

    /// <summary>
    /// Return true if Windows XP 64-bit
    /// </summary>
    /// <returns></returns>
    public static bool IsWindowsXP64()
    {
      return Environment.OSVersion.Version.ToString() == "5.2.3790.0" ? true : false;
    }

    /// <summary>
    /// Return true if Windows Vista
    /// </summary>
    /// <returns></returns>
    public static bool IsWindowsVista()
    {
      return Environment.OSVersion.Version.ToString() == "6.0.6000.0" ? true : false;
    }

    /// <summary>
    /// Return true if Windows Vista + SP2
    /// </summary>
    /// <returns></returns>
    public static bool IsWindowsVistaSP2()
    {
      return Environment.OSVersion.Version.ToString() == "6.0.6002.0" ? true : false;
    }

    /// <summary>Get the current platform.</summary>
    /// <returns></returns>
    public static Platform RunningPlatform()
    {
      switch (Environment.OSVersion.Platform)
      {
        case PlatformID.Unix:
          // Well, there are chances MacOSX is reported as Unix instead of MacOSX.
          // Instead of platform check, we'll do a folder check (Mac specific root folders)
          if (Directory.Exists("/Applications")
              & Directory.Exists("/System")
              & Directory.Exists("/Users")
              & Directory.Exists("/Volumes"))
            return Platform.Mac;
          else
            return Platform.Linux;

        case PlatformID.MacOSX:
          return Platform.Mac;

        default:
          return Platform.Windows;
      }
    }

    /// <summary>
    ///
    /// </summary>
    public enum Platform
    {
      /// <summary>Windows Platform</summary>
      Windows,

      /// <summary>Linux Platform</summary>
      Linux,

      /// <summary>OSX Platform</summary>
      Mac
    }

    /// <summary>
    /// This Method is Used to get the Friendly Name of the OS
    /// </summary>
    /// <param name="platform">Platform</param>
    /// <returns>return Friendly OS name</returns>
    public static string GetOSFriendlyName(Platform platform)
    {
      string OSFriendlyName = string.Empty;

      try
      {
        switch (platform)
        {
          case Platform.Windows:
            OSFriendlyName = GetWinOSFriendlyName();
            break;

          case Platform.Mac:
            OSFriendlyName = Environment.OSVersion.VersionString;
            break;

          case Platform.Linux:
            OSFriendlyName = Environment.OSVersion.VersionString;
            break;

          default:
            OSFriendlyName = GetWinOSFriendlyName();
            break;
        }
        return OSFriendlyName;
      }
      catch (Exception ex)
      {
        Log.WriteLine("Exception Occurred while getting the Friendly OS Name -" + ex.ToString(), Log.LogType.ERROR);
        throw;
      }
    }

    /// <summary>
    /// This Method is Used to Get the Friendly Name of the Windows OS
    /// </summary>
    /// <returns>Return Friendly Name</returns>
    private static string GetWinOSFriendlyName()
    {
      try
      {
        string osName = string.Empty;
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
        foreach (ManagementObject os in searcher.Get())
        {
          osName = os["Caption"].ToString();
          break;
        }

        return osName.Trim();
      }
      catch (Exception ex)
      {
        Log.WriteLine("Exception occurred while getting the Win OS Friendly Name -" + ex.ToString(), Log.LogType.ERROR);
        throw;
      }
    }

    /// <summary>
    /// Returns Machine RAM
    /// </summary>
    /// <returns>Return Machine Ram</returns>
    public static string GetMachineRAM()
    {
      try
      {
        ManagementScope oMs = new ManagementScope();
        ObjectQuery oQuery = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
        ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
        ManagementObjectCollection oCollection = oSearcher.Get();
        ulong MemSize = 0;
        ulong mCap = 0;
        foreach (ManagementObject obj in oCollection)
        {
          mCap = (ulong)obj["Capacity"];
          MemSize += mCap;
        }
        MemSize = (MemSize / 1024) / 1024;
        return MemSize + " MB";
      }
      catch (Exception ex)
      {
        Log.WriteLine("Exception occurred while getting the Machine RAM -" + ex.ToString(), Log.LogType.ERROR);
        throw;
      }
    }

    /// <summary>
    /// Returns Machine Processor
    /// </summary>
    /// <returns>return Processor Name</returns>
    public static string GetMachineProcessor()
    {
      try
      {
        RegistryKey RegKey = Registry.LocalMachine;
        RegKey = RegKey.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
        Object cpuSpeed = RegKey.GetValue("~MHz");
        Object cpuType = RegKey.GetValue("VendorIdentifier");
        return cpuType.ToString() + "-" + cpuSpeed.ToString() + "MHz";
      }
      catch (Exception ex)
      {
        Log.WriteLine("Exception occurred while getting the Machine Processor -" + ex.ToString(), Log.LogType.ERROR);
        throw;
      }
    }
  }
}
