using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComerceProject.Libraries.Logger
{
    public interface ICaptureScreen
  {
        /// <parameter name = "screenshotPath">path to save screenshot</parameter>

        string GetSnapShot(string screenShotPath);


    }
}
