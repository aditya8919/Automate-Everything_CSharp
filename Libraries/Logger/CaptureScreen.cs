using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using EComerceProject.Libraries.Logger;
using OpenQA.Selenium.BiDi.BrowsingContext;

namespace CCH.Automation.Utilities.HTMLLogger
{
    /// <summary>
    /// This class will keep all the functionality for capturing the desktop.
    /// </summary>
    public class CaptureScreen : UtilityBase, ICaptureScreen
    {
        private const int SRCCOPY = 13369376;

        /// <summary>
        /// The width of the virtual screen, in pixels. The virtual screen is the bounding rectangle of all display monitors. The SM_XVIRTUALSCREEN metric is the coordinates for the left side of the virtual screen.
        /// </summary>
        private const int SM_CXSCREEN = 78;

        /// <summary>
        /// The height of the virtual screen, in pixels. The virtual screen is the bounding rectangle of all display monitors. The SM_YVIRTUALSCREEN metric is the coordinates for the top of the virtual screen.
        /// </summary>
        private const int SM_CYSCREEN = 79;

        private IBitmapMethods nativeMethods = new BitmapNativeMethods();

        /// <summary>
        ///
        /// </summary>
        public IBitmapMethods NativeMethods
        {
            set
            {
                nativeMethods = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public CaptureScreen()
        {
        }

        /// <summary>
        /// This Function is Used to get the BitMap for the Current Visible UI on the Desktop
        /// </summary>
        /// <returns></returns>
        public Bitmap GetDesktopImage()
        {
            //In size variable we shall keep the size of the screen.
            int width, height;

            //Variable to keep the handle to bitmap.
            IntPtr hBitmap;

            //Here we get the handle to the desktop device context.
            IntPtr hDC = nativeMethods.GetDC(nativeMethods.GetDesktopWindow());

            //Here we make a compatible device context in memory for screen device context.
            IntPtr hMemDC = nativeMethods.CreateCompatibleDC(hDC);

            //We pass SM_CXSCREEN constant to GetSystemMetrics to get the X coordinates of screen.
            width = nativeMethods.GetSystemMetrics(SM_CXSCREEN);

            //We pass SM_CYSCREEN constant to GetSystemMetrics to get the Y coordinates of screen.
            height = nativeMethods.GetSystemMetrics(SM_CYSCREEN);

            //We create a compatible bitmap of screen size using screen device context.
            hBitmap = nativeMethods.CreateCompatibleBitmap(hDC, width, height);

            //As hBitmap is IntPtr we can not check it against null. For this purpose IntPtr.Zero is used.
            if (hBitmap != IntPtr.Zero)
            {
                // selecting an bitmap object into the  device context
                IntPtr hOld = (IntPtr)nativeMethods.SelectObject(hMemDC, hBitmap);

                //We copy the Bitmap to the memory device context.
                nativeMethods.BitBlt(hMemDC, 0, 0, width, height, hDC, 0, 0, SRCCOPY);

                //We select the old bitmap back to the memory device context.
                nativeMethods.SelectObject(hMemDC, hOld);

                //We delete the memory device context.
                nativeMethods.DeleteDC(hMemDC);

                //We release the screen device context.
                nativeMethods.ReleaseDC(nativeMethods.GetDesktopWindow(), hDC);

                //Image is created by Image bitmap handle and stored in local variable.
                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);

                //Release the memory to avoid memory leaks.
                nativeMethods.DeleteObject(hBitmap);

                //This statement runs the garbage collector manually.
                GC.Collect();

                //Return the bitmap
                return bmp;
            }
            return null;
        }

        /// <summary>
        /// This Function Will Save the Snapshot to the ErrorScreenShot Folder in the Log Location
        /// </summary>
        /// <returns>string - Returns the location of the snapshot</returns>
        public string GetSnapShot(string screenShotPath)
        {
            try
            {
                string ImageLocation = Path.Combine(screenShotPath, DateTime.Now.ToString("hhmmffff") + ".jpg");

                Bitmap bitmap = GetDesktopImage();
                bitmap.Save(ImageLocation, System.Drawing.Imaging.ImageFormat.Jpeg);

                // Changing the absolute path to relative path to be used for href and src for images.
                ImageLocation = GetRelativePath(ImageLocation);

                return ImageLocation;
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, Log.LogType.ERROR);
                throw;
            }
        }

        /// <summary>
        /// This Method with set the absolute path to relative path to be used for href and src for images.
        /// </summary>
        /// <param name="ImageLocation">ImageLocation</param>
        /// <returns>string - Relative Path</returns>
        private static string GetRelativePath(string ImageLocation)
        {
            ImageLocation = "." + ImageLocation.Substring(ImageLocation.IndexOf(Path.DirectorySeparatorChar + "ScreenShots"),
                ImageLocation.Length - ImageLocation.IndexOf(Path.DirectorySeparatorChar + "ScreenShots"));
            return ImageLocation;
        }
    }
}
