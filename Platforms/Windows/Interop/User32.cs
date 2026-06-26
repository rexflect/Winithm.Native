using System.Runtime.InteropServices;

namespace Winithm.Native.Platforms.Windows.Interop;

/// <summary>
/// Win32 user32.dll interop declarations using source-generated <see cref="LibraryImportAttribute"/>.
/// <para>
/// When adding Rust-built native libraries in the future, create a separate interop class
/// (e.g. <c>WinithmNativeWin.cs</c>) with <c>[LibraryImport("winithm_native_win")]</c>.
/// </para>
/// </summary>
internal static partial class User32
{
  internal const uint MONITOR_DEFAULTTONEAREST = 2;

  [LibraryImport("user32.dll")]
  internal static partial nint MonitorFromPoint(POINT pt, uint dwFlags);

  [LibraryImport("user32.dll", EntryPoint = "GetMonitorInfoW", StringMarshalling = StringMarshalling.Utf16)]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static partial bool GetMonitorInfo(nint hMonitor, ref MONITORINFO lpmi);
  [LibraryImport("user32.dll", EntryPoint = "GetWindowLongW")]
  internal static partial int GetWindowLong(nint hWnd, int nIndex);

  [LibraryImport("user32.dll", EntryPoint = "SetWindowLongW")]
  internal static partial int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

  [LibraryImport("user32.dll", SetLastError = false)]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static partial bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
}

[StructLayout(LayoutKind.Sequential)]
internal struct POINT(int x, int y)
{
  public int X = x;
  public int Y = y;
}

[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
  public int Left;
  public int Top;
  public int Right;
  public int Bottom;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct MONITORINFO
{
  public uint CbSize;
  public RECT RcMonitor;
  public RECT RcWork;
  public uint DwFlags;
}
