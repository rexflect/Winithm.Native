using System;
using Winithm.Native.Platforms.Linux.Interop;

namespace Winithm.Native.Platforms.Linux;

/// <summary>
/// Linux platform provider.
/// <para>
/// Reads the <c>_NET_WORKAREA</c> EWMH property from the X11 root window
/// to determine the desktop area excluding panels/taskbars.
/// Falls back to <see cref="SafeAreaRect.Empty"/> if X11 is unavailable
/// (e.g. Wayland-only sessions without XWayland).
/// </para>
/// </summary>
internal sealed class LinuxPlatformProvider : IPlatformProvider
{
  public SafeAreaRect GetWorkArea(int windowX, int windowY)
  {
    nint display = nint.Zero;
    try
    {
      display = X11.XOpenDisplay(nint.Zero);
      if (display == nint.Zero)
        return SafeAreaRect.Empty;

      var root = X11.XDefaultRootWindow(display);
      var workAreaAtom = X11.XInternAtom(display, "_NET_WORKAREA", onlyIfExists: true);

      if (workAreaAtom == nint.Zero)
        return SafeAreaRect.Empty;

      int result = X11.XGetWindowProperty(
        display, root, workAreaAtom,
        longOffset: 0, longLength: 4,
        delete: false,
        reqType: X11.XA_CARDINAL,
        out _,                   // actualType
        out int actualFormat,
        out nuint nItems,
        out _,                   // bytesAfter
        out nint propReturn
      );

      if (result != 0 || propReturn == nint.Zero || nItems < 4)
      {
        if (propReturn != nint.Zero) _ = X11.XFree(propReturn);
        return SafeAreaRect.Empty;
      }

      try
      {
        // _NET_WORKAREA is an array of CARDINAL (32-bit unsigned ints): x, y, width, height
        // repeated per desktop. We read the first desktop's work area.
        ReadOnlySpan<int> values;
        unsafe
        {
          values = new ReadOnlySpan<int>((void*)propReturn, 4);
        }

        return new SafeAreaRect(values[0], values[1], values[2], values[3]);
      }
      finally
      {
        _ = X11.XFree(propReturn);
      }
    }
    catch
    {
      return SafeAreaRect.Empty;
    }
    finally
    {
      if (display != nint.Zero)
        _ = X11.XCloseDisplay(display);
    }
  }
}
