using Pranas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComerceProject.Libraries.Logger
{
  /// <summary>
  /// This Class Represents the ScreenShot Capture Mechanism for Mac and Windows
  /// </summary>
  public class DesktopScreenCapture : ICaptureScreen
  {
    /// <summary>
    /// This Function Will Save the Snapshot to the ErrorScreenShot Folder in the Log Location
    /// </summary>
    /// <returns>string - Returns the location of the snapshot</returns>
    public string GetSnapShot(string screenShotPath)
    {
      string imageLocation = Path.Combine(screenShotPath, DateTime.Now.ToString("hhmmffff") + ".jpg");

      Image a = ScreenshotCapture.TakeScreenshot();
      a.Save(imageLocation, System.Drawing.Imaging.ImageFormat.Jpeg);

      // Changing the absolute path to relative path to be used for href and src for images.
      imageLocation = GetRelativePath(imageLocation);

      return imageLocation;
    }

    /// <summary>
    /// This Method with set the absolute path to relative path to be used for href and src for images.
    /// </summary>
    /// <param name="imageLocation">ImageLocation</param>
    /// <returns>string - Relative Path</returns>
    private static string GetRelativePath(string imageLocation)
    {
      imageLocation = "." + imageLocation.Substring(imageLocation.IndexOf(Path.DirectorySeparatorChar + "ScreenShots"),
      imageLocation.Length - imageLocation.IndexOf(Path.DirectorySeparatorChar + "ScreenShots"));
      return imageLocation;
    }
  }
}
