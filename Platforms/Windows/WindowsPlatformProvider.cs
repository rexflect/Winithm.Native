using System.Runtime.InteropServices;
using Winithm.Native.Platforms.Windows.Interop;

namespace Winithm.Native.Platforms.Windows;

/// <summary>
/// Windows platform provider using Win32 APIs.
/// <para>
/// Uses <c>MonitorFromPoint</c> + <c>GetMonitorInfo</c> to retrieve the work area
/// (desktop minus taskbar) for the monitor containing the game window.
/// </para>
/// </summary>
internal sealed class WindowsPlatformProvider : IPlatformProvider
{
  public SafeAreaRect GetWorkArea(int windowX, int windowY)
  {
    var pt = new POINT(windowX, windowY);
    var hMonitor = User32.MonitorFromPoint(pt, User32.MONITOR_DEFAULTTONEAREST);

    if (hMonitor == nint.Zero)
      return SafeAreaRect.Empty;

    var mi = new MONITORINFO { CbSize = (uint)Marshal.SizeOf<MONITORINFO>() };

    if (!User32.GetMonitorInfo(hMonitor, ref mi))
      return SafeAreaRect.Empty;

    return new SafeAreaRect(
      mi.RcWork.Left,
      mi.RcWork.Top,
      mi.RcWork.Right - mi.RcWork.Left,
      mi.RcWork.Bottom - mi.RcWork.Top
    );
  }

  public Godot.Color? GetAccentColor()
  {
    if (Dwmapi.DwmGetColorizationColor(out uint color, out _) == 0) // S_OK
    {
      // Format is AARRGGBB
      float a = ((color >> 24) & 0xFF) / 255f;
      float r = ((color >> 16) & 0xFF) / 255f;
      float g = ((color >> 8) & 0xFF) / 255f;
      float b = (color & 0xFF) / 255f;

      // We ignore alpha to just return the base accent color.
      return new Godot.Color(r, g, b, a);
    }
    return null;
  }

  private const int GWL_EXSTYLE = -20;
  private const int WS_EX_TRANSPARENT = 0x00000020;
  private const int WS_EX_LAYERED = 0x00080000;
  
  private const uint SWP_NOMOVE = 0x0002;
  private const uint SWP_NOSIZE = 0x0001;
  private const uint SWP_NOZORDER = 0x0004;
  private const uint SWP_FRAMECHANGED = 0x0020;

  public void SetClickThrough(nint hwnd, bool passthrough)
  {
    if (hwnd == 0) return;

    int exStyle = User32.GetWindowLong(hwnd, GWL_EXSTYLE);
    if (passthrough)
      exStyle |= WS_EX_TRANSPARENT | WS_EX_LAYERED;
    else
      exStyle &= ~WS_EX_TRANSPARENT; // Do not remove WS_EX_LAYERED to preserve transparency.

    User32.SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
    // Force Windows to apply the updated exStyle
    User32.SetWindowPos(hwnd, 0, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
  }
}
