namespace Winithm.Native;

/// <summary>
/// Platform-independent safe area rectangle (excludes OS chrome: taskbar, dock, panels).
/// All values in physical (unscaled) pixels.
/// </summary>
public readonly record struct SafeAreaRect(int X, int Y, int Width, int Height)
{
  public bool IsEmpty => Width <= 0 || Height <= 0;

  public static SafeAreaRect Empty => default;
}
