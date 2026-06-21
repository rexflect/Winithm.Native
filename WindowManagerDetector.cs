using System;
using System.Runtime.InteropServices;

namespace Winithm.Native;

/// <summary>
/// Supported windowing environments and OS versions.
/// </summary>
public enum WindowManagerType
{
  Unknown,
  Windows10,
  Windows11,
  MacOS,
  X11,
  Wayland
}

/// <summary>
/// Detects the current window manager or desktop environment session type.
/// </summary>
public static class WindowManagerDetector
{
  /// <summary>
  /// Identifies the active WM/OS based on runtime info and environment variables.
  /// </summary>
  public static WindowManagerType Detect()
  {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      // Windows 11 starts at build 22000
      var version = Environment.OSVersion.Version;
      return version.Build >= 22000 ? WindowManagerType.Windows11 : WindowManagerType.Windows10;
    }

    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
      return WindowManagerType.MacOS;
    }

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
      // Check standard session type first
      var sessionType = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");
      if (string.Equals(sessionType, "wayland", StringComparison.OrdinalIgnoreCase))
      {
        return WindowManagerType.Wayland;
      }

      // Fallback to Wayland display socket
      var waylandDisplay = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
      if (!string.IsNullOrEmpty(waylandDisplay))
      {
        return WindowManagerType.Wayland;
      }

      return WindowManagerType.X11;
    }

    return WindowManagerType.Unknown;
  }
}