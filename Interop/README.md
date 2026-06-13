# Winithm.Native — Rust FFI Interop Guide

This directory is reserved for Rust-compiled native libraries that provide
platform-specific functionality beyond what C# P/Invoke to OS APIs can offer.

## Architecture

```
Interop/
├── runtimes/
│   ├── win-x64/native/      → winithm_native_win.dll
│   ├── osx-x64/native/      → libwinithm_native_mac.dylib
│   ├── osx-arm64/native/    → libwinithm_native_mac.dylib
│   └── linux-x64/native/    → libwinithm_native_linux.so
└── README.md                → This file
```

## How to Add a Rust Native Library

### 1. Create the Rust Project

```bash
cargo init --lib winithm-native-win
```

In `Cargo.toml`:
```toml
[lib]
crate-type = ["cdylib"]
```

### 2. Export a C-ABI Function

```rust
#[no_mangle]
pub extern "C" fn defer_window_pos(
    hwnd: isize,
    x: i32, y: i32,
    width: i32, height: i32,
) -> bool {
    // ... Win32 DeferWindowPos implementation ...
    true
}
```

### 3. Build for Target Platform

```bash
cargo build --release --target x86_64-pc-windows-msvc
```

### 4. Place the Binary

Copy the built `.dll`/`.so`/`.dylib` to the appropriate
`runtimes/{rid}/native/` directory.

### 5. Declare in C# Provider

```csharp
// In Platforms/Windows/Interop/WinithmNativeWin.cs
internal static partial class WinithmNativeWin
{
    [LibraryImport("winithm_native_win")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DeferWindowPos(
        nint hwnd, int x, int y, int width, int height);
}
```

### 6. Add to csproj (if using runtimes/ convention)

```xml
<ItemGroup>
  <None Include="Interop/runtimes/**/*" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

## Why Rust?

- **DeferWindowPos**: Batch window positioning requires holding Win32 handles
  across calls — Rust's ownership model prevents handle leaks.
- **Desktop Capture**: DXGI Desktop Duplication API is easier to manage in Rust
  with proper RAII for COM objects.
- **Performance**: Hot-path native code avoids .NET marshalling overhead.
