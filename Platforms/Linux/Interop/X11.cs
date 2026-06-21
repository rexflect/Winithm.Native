using System.Runtime.InteropServices;

namespace Winithm.Native.Platforms.Linux.Interop;

/// <summary>
/// Minimal X11 interop for reading <c>_NET_WORKAREA</c> from the root window.
/// <para>
/// This is the EWMH standard hint that desktop environments (GNOME, KDE, XFCE, etc.)
/// use to advertise the usable desktop area excluding panels/taskbars.
/// </para>
/// </summary>
internal static partial class X11
{
  private const string LibX11 = "libX11.so.6";

  [LibraryImport(LibX11)]
  internal static partial nint XOpenDisplay(nint displayName);

  [LibraryImport(LibX11)]
  internal static partial int XCloseDisplay(nint display);

  [LibraryImport(LibX11)]
  internal static partial nint XDefaultRootWindow(nint display);

  [LibraryImport(LibX11, StringMarshalling = StringMarshalling.Utf8)]
  internal static partial nint XInternAtom(nint display, string atomName,
    [MarshalAs(UnmanagedType.Bool)] bool onlyIfExists);

  [LibraryImport(LibX11)]
  internal static partial int XGetWindowProperty(
    nint display,
    nint window,
    nint property,
    long longOffset,
    long longLength,
    [MarshalAs(UnmanagedType.Bool)] bool delete,
    nint reqType,
    out nint actualTypeReturn,
    out int actualFormatReturn,
    out nuint nItemsReturn,
    out nuint bytesAfterReturn,
    out nint propReturn
  );

  [LibraryImport(LibX11)]
  internal static partial int XFree(nint data);

  /// <summary>XA_CARDINAL atom type.</summary>
  internal static readonly nint XA_CARDINAL = 6;
}
