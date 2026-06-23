namespace Winithm.Native;

/// <summary>
/// Abstraction for all platform-specific native APIs.
/// <para>
/// Extend this interface when adding new platform capabilities
/// (e.g. <c>DeferWindowPos</c> via Rust FFI, desktop capture, etc.).
/// Each platform implements its own provider; unsupported platforms
/// fall back to <see cref="NullPlatformProvider"/>.
/// </para>
/// </summary>
public interface IPlatformProvider
{
  /// <summary>
  /// Returns the usable work area (safe area excluding taskbar/dock/panel)
  /// for the monitor that contains the given window position.
  /// </summary>
  /// <param name="windowX">Window X position in physical pixels.</param>
  /// <param name="windowY">Window Y position in physical pixels.</param>
  /// <returns>
  /// A <see cref="SafeAreaRect"/> describing the work area,
  /// or <see cref="SafeAreaRect.Empty"/> if the query fails.
  /// </returns>
  SafeAreaRect GetWorkArea(int windowX, int windowY);

  // ── Future API surface ──────────────────────────────────────────────
  // nint BeginDeferWindowPos(int count);
  // nint DeferWindowPos(nint hdwp, nint hwnd, nint hwndInsertAfter,
  //                     int x, int y, int cx, int cy, uint flags);
  // bool EndDeferWindowPos(nint hdwp);

  /// <summary>
  /// Returns the OS accent color, or null if unavailable.
  /// </summary>
  Godot.Color? GetAccentColor();
}
