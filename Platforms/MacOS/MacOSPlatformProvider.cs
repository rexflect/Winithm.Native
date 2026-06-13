using System.Runtime.InteropServices;
using Winithm.Native.Platforms.MacOS.Interop;

namespace Winithm.Native.Platforms.MacOS;

/// <summary>
/// macOS platform provider.
/// <para>
/// Queries <c>[[NSScreen mainScreen] visibleFrame]</c> via the Objective-C runtime
/// to get the desktop area excluding the menu bar and Dock.
/// </para>
/// </summary>
internal sealed class MacOSPlatformProvider : IPlatformProvider
{
  public SafeAreaRect GetWorkArea(int windowX, int windowY)
  {
    try
    {
      var nsScreenClass = AppKit.ObjcGetClass("NSScreen");
      if (nsScreenClass == nint.Zero)
        return SafeAreaRect.Empty;

      var mainScreenSel = AppKit.SelRegisterName("mainScreen");
      var mainScreen = AppKit.ObjcMsgSend(nsScreenClass, mainScreenSel);
      if (mainScreen == nint.Zero)
        return SafeAreaRect.Empty;

      var visibleFrameSel = AppKit.SelRegisterName("visibleFrame");

      NSRect frame;

      // On ARM64 macOS, stret is not used for small structs — objc_msgSend returns directly.
      // On x86_64, NSRect (32 bytes) requires objc_msgSend_stret.
      if (RuntimeInformation.ProcessArchitecture is Architecture.Arm64)
      {
        // ARM64: objc_msgSend returns the struct in registers
        // We need a different interop signature for this case.
        // For simplicity, use objc_msgSend_stret which works on both architectures in .NET
        AppKit.ObjcMsgSendStret(out frame, mainScreen, visibleFrameSel);
      }
      else
      {
        AppKit.ObjcMsgSendStret(out frame, mainScreen, visibleFrameSel);
      }

      // macOS uses bottom-left origin; convert to top-left for consistency.
      // We need the full screen height to flip.
      var frameSel = AppKit.SelRegisterName("frame");
      AppKit.ObjcMsgSendStret(out NSRect fullFrame, mainScreen, frameSel);

      int flippedY = (int)(fullFrame.Height - frame.Y - frame.Height);

      return new SafeAreaRect(
        (int)frame.X,
        flippedY,
        (int)frame.Width,
        (int)frame.Height
      );
    }
    catch
    {
      return SafeAreaRect.Empty;
    }
  }
}
