using OpenQA.Selenium.DevTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComerceProject.Libraries.Logger
{
  public enum LoggerType
  {
    MSTest,
    NUnit

  }

  public class LogFactory
  {


    private static Log log;


    /// <summary>
    /// This method is used to get the Log Object for any inputs Logger type
    /// </summary>
    /// <param name="logType"></param>
    /// <param name="logLocation"></param>
    /// <param name="ICaptureScreen"></param>
    /// <returns>Log Object</returns>
    public static Log LoadLog(LoggerType logType, string logLocation, ICaptureScreen ICaptureScreen = null)
    {

      if (ICaptureScreen == null)
        ICaptureScreen = new DesktopScreenCapture();

      switch(logType)
      {

        case LoggerType.MSTest:

          // log = MSTestLog.Instance(logLocation,ICaptureScreen);

          break;

        case LoggerType.NUnit:
          log = NUnitLog.Instance(logLocation, ICaptureScreen);
          break;
      }
      return log;
    }

    //public static Log LoadLog(LoggerType logType, string loglocation, ICapturescreen ICapturescreen = null)
    //{
    //  if snapshot mechanism is not implemented then this method
    //}
    public static Log Logger
    {
      get
      {
        return log;
      }
      set
      {
        log = value;
      }
    }








  }
}
