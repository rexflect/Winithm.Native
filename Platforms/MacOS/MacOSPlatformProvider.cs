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

      var frameSel = AppKit.SelRegisterName("frame");
      var visibleFrameSel = AppKit.SelRegisterName("visibleFrame");

      NSRect mainFrame = GetRect(mainScreen, frameSel);
      nint targetScreen = FindScreenContainingPoint(
        nsScreenClass,
        windowX,
        windowY,
        mainFrame,
        frameSel
      );

      if (targetScreen == nint.Zero)
        targetScreen = mainScreen;

      NSRect frame = GetRect(targetScreen, visibleFrameSel);

      // macOS uses bottom-left origin; Godot window coordinates are top-left.
      int flippedY = (int)(mainFrame.Height - frame.Y - frame.Height);

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

  private static nint FindScreenContainingPoint(
    nint nsScreenClass,
    int windowX,
    int windowY,
    NSRect mainFrame,
    nint frameSel
  )
  {
    var screensSel = AppKit.SelRegisterName("screens");
    var screens = AppKit.ObjcMsgSend(nsScreenClass, screensSel);
    if (screens == nint.Zero)
      return nint.Zero;

    var countSel = AppKit.SelRegisterName("count");
    UIntPtr count = AppKit.ObjcMsgSendNUInt(screens, countSel);
    if (count == UIntPtr.Zero)
      return nint.Zero;

    var objectAtIndexSel = AppKit.SelRegisterName("objectAtIndex:");
    for (nuint i = 0; i < (nuint)count; i++)
    {
      nint screen = AppKit.ObjcMsgSend(screens, objectAtIndexSel, (UIntPtr)i);
      if (screen == nint.Zero)
        continue;

      NSRect frame = GetRect(screen, frameSel);
      if (ContainsTopLeftPoint(frame, windowX, windowY, mainFrame.Height))
        return screen;
    }

    return nint.Zero;
  }

  private static NSRect GetRect(nint receiver, nint selector)
  {
    if (RuntimeInformation.ProcessArchitecture is Architecture.Arm64)
      return AppKit.ObjcMsgSendNSRect(receiver, selector);

    AppKit.ObjcMsgSendStret(out NSRect rect, receiver, selector);
    return rect;
  }

  private static bool ContainsTopLeftPoint(NSRect frame, int x, int y, double mainScreenHeight)
  {
    double left = frame.X;
    double top = mainScreenHeight - frame.Y - frame.Height;
    double right = left + frame.Width;
    double bottom = top + frame.Height;

    return x >= left && x < right && y >= top && y < bottom;
  }
}
