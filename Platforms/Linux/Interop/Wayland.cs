using System.IO;
using System.Linq;

namespace Winithm.Native.Platforms.Linux.Interop;

/// <summary>
/// Wayland fallback logic.
/// Reads primary monitor native resolution from sysfs since Wayland
/// isolates clients and prevents global work area queries without a toolkit.
/// </summary>
internal static class Wayland
{
  public static SafeAreaRect TryGetScreenBounds()
  {
    try
    {
      const string drmPath = "/sys/class/drm";
      if (!Directory.Exists(drmPath)) return SafeAreaRect.Empty;

      foreach (var dir in Directory.GetDirectories(drmPath, "card*-*"))
      {
        string modesFile = Path.Combine(dir, "modes");
        if (!File.Exists(modesFile)) continue;

        string? firstMode = File.ReadLines(modesFile).FirstOrDefault();
        if (string.IsNullOrEmpty(firstMode)) continue;

        var resPart = firstMode.Split(' ')[0];
        var parts = resPart.Split('x');
        if (parts.Length == 2 && int.TryParse(parts[0], out int w) && int.TryParse(parts[1], out int h))
        {
          return new SafeAreaRect(0, 0, w, h);
        }
      }
    }
    catch { }

    return SafeAreaRect.Empty;
  }
}