using System.Runtime.InteropServices;

namespace Winithm.Native.Platforms.Windows.Interop;

internal static partial class Dwmapi
{
  [LibraryImport("dwmapi.dll")]
  internal static partial int DwmGetColorizationColor(out uint pcrColorization, [MarshalAs(UnmanagedType.Bool)] out bool pfOpaqueBlend);
}
