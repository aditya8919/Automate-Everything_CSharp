using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EComerceProject.Libraries.Logger
{
  internal class BitmapNativeMethods : IBitmapMethods
  {
    #region Class Methods

    /// <summary>
    /// This function creates a memory device context (DC) compatible with the specified device.
    /// </summary>
    /// <param name="hdc">IntPtr of Device</param>
    /// <returns>IntPtr</returns>
    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
    private static extern IntPtr NativeCreateCompatibleDC(IntPtr hdc);

    /// <summary>
    /// function creates a bitmap compatible with the device that is associated with the specified device context.
    /// </summary>
    /// <param name="hdc">A handle to a device context.</param>
    /// <param name="nWidth">The bitmap width, in pixels.</param>
    /// <param name="nHeight">The bitmap height, in pixels.</param>
    /// <returns>IntPtr</returns>
    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
    private static extern IntPtr NativeCreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

    /// <summary>
    /// The SelectObject function selects an object into the specified device context (DC).
    /// </summary>
    /// <param name="hdc">IntPtr -A handle to the DC</param>
    /// <param name="bmp">IntPtr -A handle to the object to be selected.</param>
    /// <returns></returns>
    [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
    private static extern IntPtr NativeSelectObject(IntPtr hdc, IntPtr bmp);

    /// <summary>
    /// The BitBlt function performs a bit-block transfer of the color data corresponding to a rectangle of pixels from the specified source device context into a destination device context.
    /// </summary>
    /// <param name="hdcDest">A handle to the destination device context.</param>
    /// <param name="xDest">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
    /// <param name="yDest">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
    /// <param name="wDest">The width, in logical units, of the source and destination rectangles.</param>
    /// <param name="hDest">The height, in logical units, of the source and the destination rectangles.</param>
    /// <param name="hdcSource">A handle to the source device context</param>
    /// <param name="xSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
    /// <param name="ySrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
    /// <param name="RasterOp">A raster-operation code. These codes define how the color data for the source rectangle is to be combined with the color data for the destination rectangle to achieve the final color.</param>
    /// <returns></returns>
    [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
    private static extern bool NativeBitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

    /// <summary>
    /// The DeleteDC function deletes the specified device context (DC).
    /// </summary>
    /// <param name="hDc">A handle to the device context.</param>
    /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero.</returns>
    [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
    internal static extern int NativeDeleteDC(IntPtr hDc);

    /// <summary>
    /// The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object.
    /// </summary>
    /// <param name="hDc">A handle to a logical pen, brush, font, bitmap, region, or palette.</param>
    /// <returns>If the function succeeds, the return value is nonzero.If the specified handle is not valid or is currently selected into a DC, the return value is zero.</returns>
    [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
    internal static extern int NativeDeleteObject(IntPtr hDc);

    /// <summary>
    /// Retrieves a handle to the desktop window. The desktop window covers the entire screen. The desktop window is the area on top of which other windows are painted
    /// </summary>
    /// <returns>IntPtr</returns>
    [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
    private static extern IntPtr NativeGetDesktopWindow();

    /// <summary>
    /// The GetDC function retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen.
    /// </summary>
    /// <param name="ptr">IntPtr- A handle to the window whose DC is to be retrieved</param>
    /// <returns>IntPtr</returns>
    [DllImport("user32.dll", EntryPoint = "GetDC")]
    private static extern IntPtr NativeGetDC(IntPtr ptr);

    /// <summary>
    /// Retrieves the specified system metric or system configuration setting
    /// </summary>
    /// <param name="abc">int - The system metric or configuration setting to be retrieved.</param>
    /// <returns></returns>
    [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
    private static extern int NativeGetSystemMetrics(int abc);

    /// <summary>
    /// The ReleaseDC function releases a device context (DC), freeing it for use by other applications.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose DC is to be released.</param>
    /// <param name="hDc">A handle to the DC to be released.</param>
    /// <returns>The return value indicates whether the DC was released. If the DC was released, the return value is 1.If the DC was not released, the return value is zero.</returns>
    [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
    internal static extern int NativeReleaseDC(IntPtr hWnd, IntPtr hDc);

    #endregion Class Methods

    #region Methods

    /// <summary>
    /// This function creates a memory device context (DC) compatible with the specified device.
    /// </summary>
    /// <param name="hdc">IntPtr of Device</param>
    /// <returns>IntPtr</returns>
    public IntPtr CreateCompatibleDC(IntPtr hdc)
    {
      return NativeCreateCompatibleDC(hdc);
    }

    /// <summary>
    /// function creates a bitmap compatible with the device that is associated with the specified device context.
    /// </summary>
    /// <param name="hdc">A handle to a device context.</param>
    /// <param name="nWidth">The bitmap width, in pixels.</param>
    /// <param name="nHeight">The bitmap height, in pixels.</param>
    /// <returns>IntPtr</returns>
    public IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight)
    {
      return NativeCreateCompatibleBitmap(hdc, nWidth, nHeight);
    }

    /// <summary>
    /// The SelectObject function selects an object into the specified device context (DC).
    /// </summary>
    /// <param name="hdc">IntPtr -A handle to the DC</param>
    /// <param name="bmp">IntPtr -A handle to the object to be selected.</param>
    /// <returns></returns>
    public IntPtr SelectObject(IntPtr hdc, IntPtr bmp)
    {
      return NativeSelectObject(hdc, bmp);
    }

    /// <summary>
    /// The BitBlt function performs a bit-block transfer of the color data corresponding to a rectangle of pixels from the specified source device context into a destination device context.
    /// </summary>
    /// <param name="hdcDest">A handle to the destination device context.</param>
    /// <param name="xDest">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
    /// <param name="yDest">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
    /// <param name="wDest">The width, in logical units, of the source and destination rectangles.</param>
    /// <param name="hDest">The height, in logical units, of the source and the destination rectangles.</param>
    /// <param name="hdcSource">A handle to the source device context</param>
    /// <param name="xSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
    /// <param name="ySrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
    /// <param name="RasterOp">A raster-operation code. These codes define how the color data for the source rectangle is to be combined with the color data for the destination rectangle to achieve the final color.</param>
    /// <returns></returns>
    public bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp)
    {
      return NativeBitBlt(hdcDest, xDest, yDest, wDest, hDest, hdcSource, xSrc, ySrc, RasterOp);
    }

    /// <summary>
    /// The DeleteDC function deletes the specified device context (DC).
    /// </summary>
    /// <param name="hDc">A handle to the device context.</param>
    /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero.</returns>
    public int DeleteDC(IntPtr hDc)
    {
      return NativeDeleteDC(hDc);
    }

    /// <summary>
    /// The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object.
    /// </summary>
    /// <param name="hDc">A handle to a logical pen, brush, font, bitmap, region, or palette.</param>
    /// <returns>If the function succeeds, the return value is nonzero.If the specified handle is not valid or is currently selected into a DC, the return value is zero.</returns>
    public int DeleteObject(IntPtr hDc)
    {
      return NativeDeleteObject(hDc);
    }

    /// <summary>
    /// Retrieves a handle to the desktop window. The desktop window covers the entire screen. The desktop window is the area on top of which other windows are painted
    /// </summary>
    /// <returns>IntPtr</returns>
    public IntPtr GetDesktopWindow()
    {
      return NativeGetDesktopWindow();
    }

    /// <summary>
    /// The GetDC function retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen.
    /// </summary>
    /// <param name="ptr">IntPtr- A handle to the window whose DC is to be retrieved</param>
    /// <returns>IntPtr</returns>
    public IntPtr GetDC(IntPtr ptr)
    {
      return NativeGetDC(ptr);
    }

    /// <summary>
    /// Retrieves the specified system metric or system configuration setting
    /// </summary>
    /// <param name="abc">int - The system metric or configuration setting to be retrieved.</param>
    /// <returns></returns>
    public int GetSystemMetrics(int abc)
    {
      return NativeGetSystemMetrics(abc);
    }

    /// <summary>
    /// The ReleaseDC function releases a device context (DC), freeing it for use by other applications.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose DC is to be released.</param>
    /// <param name="hDc">A handle to the DC to be released.</param>
    /// <returns>The return value indicates whether the DC was released. If the DC was released, the return value is 1.If the DC was not released, the return value is zero.</returns>
    public int ReleaseDC(IntPtr hWnd, IntPtr hDc)
    {
      return NativeReleaseDC(hWnd, hDc);
    }

    #endregion Methods
  }

  /// <summary>
  /// Interface for Bitmap methods
  /// </summary>
  public interface IBitmapMethods
  {
    /// <summary>
    /// This function creates a memory device context (DC) compatible with the specified device.
    /// </summary>
    /// <param name="hdc">IntPtr of Device</param>
    /// <returns>IntPtr</returns>
    IntPtr CreateCompatibleDC(IntPtr hdc);

    /// <summary>
    /// function creates a bitmap compatible with the device that is associated with the specified device context.
    /// </summary>
    /// <param name="hdc">A handle to a device context.</param>
    /// <param name="nWidth">The bitmap width, in pixels.</param>
    /// <param name="nHeight">The bitmap height, in pixels.</param>
    /// <returns>IntPtr</returns>
    IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

    /// <summary>
    /// The SelectObject function selects an object into the specified device context (DC).
    /// </summary>
    /// <param name="hdc">IntPtr -A handle to the DC</param>
    /// <param name="bmp">IntPtr -A handle to the object to be selected.</param>
    /// <returns></returns>
    IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

    /// <summary>
    /// The BitBlt function performs a bit-block transfer of the color data corresponding to a rectangle of pixels from the specified source device context into a destination device context.
    /// </summary>
    /// <param name="hdcDest">A handle to the destination device context.</param>
    /// <param name="xDest">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
    /// <param name="yDest">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
    /// <param name="wDest">The width, in logical units, of the source and destination rectangles.</param>
    /// <param name="hDest">The height, in logical units, of the source and the destination rectangles.</param>
    /// <param name="hdcSource">A handle to the source device context</param>
    /// <param name="xSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
    /// <param name="ySrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
    /// <param name="RasterOp">A raster-operation code. These codes define how the color data for the source rectangle is to be combined with the color data for the destination rectangle to achieve the final color.</param>
    /// <returns></returns>
    bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

    /// <summary>
    /// The DeleteDC function deletes the specified device context (DC).
    /// </summary>
    /// <param name="hDc">A handle to the device context.</param>
    /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero.</returns>
    int DeleteDC(IntPtr hDc);

    /// <summary>
    /// The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object.
    /// </summary>
    /// <param name="hDc">A handle to a logical pen, brush, font, bitmap, region, or palette.</param>
    /// <returns>If the function succeeds, the return value is nonzero.If the specified handle is not valid or is currently selected into a DC, the return value is zero.</returns>
    int DeleteObject(IntPtr hDc);

    /// <summary>
    /// Retrieves a handle to the desktop window. The desktop window covers the entire screen. The desktop window is the area on top of which other windows are painted
    /// </summary>
    /// <returns>IntPtr</returns>
    IntPtr GetDesktopWindow();

    /// <summary>
    /// The GetDC function retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen.
    /// </summary>
    /// <param name="ptr">IntPtr- A handle to the window whose DC is to be retrieved</param>
    /// <returns>IntPtr</returns>
    IntPtr GetDC(IntPtr ptr);

    /// <summary>
    /// Retrieves the specified system metric or system configuration setting
    /// </summary>
    /// <param name="abc">int - The system metric or configuration setting to be retrieved.</param>
    /// <returns></returns>
    int GetSystemMetrics(int abc);

    /// <summary>
    /// The ReleaseDC function releases a device context (DC), freeing it for use by other applications.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose DC is to be released.</param>
    /// <param name="hDc">A handle to the DC to be released.</param>
    /// <returns>The return value indicates whether the DC was released. If the DC was released, the return value is 1.If the DC was not released, the return value is zero.</returns>
    int ReleaseDC(IntPtr hWnd, IntPtr hDc);
  }
}
