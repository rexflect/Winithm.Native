using System;
using System.Runtime.InteropServices;
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
  private const string NET_WORKAREA = "_NET_WORKAREA";
  private const string NET_CURRENT_DESKTOP = "_NET_CURRENT_DESKTOP";

  public SafeAreaRect GetWorkArea(int windowX, int windowY)
  {
    nint display = nint.Zero;
    try
    {
      display = X11.XOpenDisplay(nint.Zero);
      if (display == nint.Zero)
        return SafeAreaRect.Empty;

      var root = X11.XDefaultRootWindow(display);

      if (!TryReadCardinalProperty(display, root, NET_WORKAREA, 1024, out ulong[] workAreaValues))
        return SafeAreaRect.Empty;

      int desktopCount = workAreaValues.Length / 4;
      if (desktopCount <= 0)
        return SafeAreaRect.Empty;

      ulong currentDesktop = 0;
      if (TryReadCardinalProperty(display, root, NET_CURRENT_DESKTOP, 1, out ulong[] desktopValues)
        && desktopValues.Length > 0)
      {
        currentDesktop = desktopValues[0];
      }

      int desktopIndex = currentDesktop < (ulong)desktopCount
        ? (int)currentDesktop
        : 0;

      int offset = desktopIndex * 4;
      if (!TryToInt32(workAreaValues[offset], out int x)
        || !TryToInt32(workAreaValues[offset + 1], out int y)
        || !TryToInt32(workAreaValues[offset + 2], out int width)
        || !TryToInt32(workAreaValues[offset + 3], out int height))
        return SafeAreaRect.Empty;

      return new SafeAreaRect(x, y, width, height);
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

  private static bool TryReadCardinalProperty(
    nint display,
    nint window,
    string propertyName,
    long length,
    out ulong[] values
  )
  {
    values = [];

    nint propertyAtom = X11.XInternAtom(display, propertyName, onlyIfExists: true);
    if (propertyAtom == nint.Zero)
      return false;

    int result = X11.XGetWindowProperty(
      display,
      window,
      propertyAtom,
      longOffset: 0,
      longLength: length,
      delete: false,
      reqType: X11.XA_CARDINAL,
      out nint actualType,
      out int actualFormat,
      out nuint nItems,
      out _,
      out nint propReturn
    );

    if (result != 0 || propReturn == nint.Zero)
    {
      if (propReturn != nint.Zero) _ = X11.XFree(propReturn);
      return false;
    }

    try
    {
      if (actualType != X11.XA_CARDINAL || actualFormat != 32 || nItems == 0)
        return false;

      if (nItems > int.MaxValue)
        return false;

      int count = (int)nItems;
      values = new ulong[count];

      // Xlib stores format-32 property data as native C longs. On Linux x64
      // that means 8-byte slots even though the property format is 32 bits.
      int stride = IntPtr.Size;
      for (int i = 0; i < count; i++)
      {
        values[i] = stride == 8
          ? unchecked((ulong)Marshal.ReadInt64(propReturn, i * stride))
          : unchecked((uint)Marshal.ReadInt32(propReturn, i * stride));
      }

      return true;
    }
    finally
    {
      _ = X11.XFree(propReturn);
    }
  }

  private static bool TryToInt32(ulong value, out int result)
  {
    if (value > int.MaxValue)
    {
      result = 0;
      return false;
    }

    result = (int)value;
    return true;
  }
}
