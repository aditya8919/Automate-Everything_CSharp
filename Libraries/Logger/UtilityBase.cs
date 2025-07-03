using OpenQA.Selenium.DevTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComerceProject.Libraries.Logger
{
  /// <summary>
  /// this class is used to get the log class object
  /// </summary>
  public class UtilityBase
  {
    public static Log Log
    {
      get
      {
        return LogFactory.Logger;
      }
    }
  }
}
