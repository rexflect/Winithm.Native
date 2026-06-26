namespace Winithm.Native;

/// <summary>
/// Fallback provider for unsupported platforms.
/// Every API returns a safe default (empty/noop).
/// </summary>
internal sealed class NullPlatformProvider : IPlatformProvider
{
  public SafeAreaRect GetWorkArea(int windowX, int windowY) => SafeAreaRect.Empty;

  public Godot.Color? GetAccentColor() => null;

  public void SetClickThrough(nint hwnd, bool passthrough) { }
}
