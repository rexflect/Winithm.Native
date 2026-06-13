using System.Runtime.InteropServices;

namespace Winithm.Native.Platforms.MacOS.Interop;

/// <summary>
/// Minimal Objective-C runtime interop for querying NSScreen.
/// <para>
/// Uses <c>objc_msgSend</c> to call <c>[[NSScreen mainScreen] visibleFrame]</c>
/// which returns the screen area excluding the menu bar and Dock.
/// </para>
/// </summary>
internal static partial class AppKit
{
  [LibraryImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_getClass",
    StringMarshalling = StringMarshalling.Utf8)]
  internal static partial nint ObjcGetClass(string name);

  [LibraryImport("/usr/lib/libobjc.A.dylib", EntryPoint = "sel_registerName",
    StringMarshalling = StringMarshalling.Utf8)]
  internal static partial nint SelRegisterName(string name);

  [LibraryImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
  internal static partial nint ObjcMsgSend(nint receiver, nint selector);

  [LibraryImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend_stret")]
  internal static partial void ObjcMsgSendStret(out NSRect result, nint receiver, nint selector);
}

/// <summary>
/// NSRect equivalent — <c>{ origin: { x, y }, size: { width, height } }</c>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct NSRect
{
  public double X;
  public double Y;
  public double Width;
  public double Height;
}
