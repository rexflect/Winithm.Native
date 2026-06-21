using System.Runtime.InteropServices;

namespace Winithm.Native.Platforms.Linux.Interop;

/// <summary>
/// Minimal X11 interop for reading EWMH properties from the root window.
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

  /// <summary>Any property type for requests.</summary>
  internal static readonly nint XA_ANY_PROPERTY_TYPE = 0;

  /// <summary>XA_CARDINAL atom type.</summary>
  internal static readonly nint XA_CARDINAL = 6;
}