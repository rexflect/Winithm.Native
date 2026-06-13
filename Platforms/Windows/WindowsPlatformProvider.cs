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
}
