using System.Runtime.InteropServices;

namespace Winithm.Native;

/// <summary>
/// Creates the appropriate <see cref="IPlatformProvider"/> for the current OS.
/// </summary>
public static class PlatformProviderFactory
{
  public static IPlatformProvider Create() => true switch
  {
    _ when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => new Platforms.Windows.WindowsPlatformProvider(),
    _ when RuntimeInformation.IsOSPlatform(OSPlatform.OSX)     => new Platforms.MacOS.MacOSPlatformProvider(),
    _ when RuntimeInformation.IsOSPlatform(OSPlatform.Linux)   => new Platforms.Linux.LinuxPlatformProvider(),
    _                                                           => new NullPlatformProvider(),
  };
}
