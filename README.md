# Image Dupe Identifier

Find duplicate photos and organize large photo libraries (100K+ images) on **Windows and Mac**.
Runs **completely offline** — your photos never leave your computer. See [SECURITY.md](SECURITY.md).

> Status: early development (v0.x). Exact-duplicate detection engine is in place; the UI is a shell.

## Download

Go to **[Releases](../../releases)** and pick your file:

| Your computer | Download |
|---|---|
| Windows (most PCs) | `ImageDupe-<version>-win-x64.zip` |
| Windows on ARM (Surface Pro X, Snapdragon laptops) | `ImageDupe-<version>-win-arm64.zip` |
| Mac with Apple chip (M1/M2/M3/M4…) | `ImageDupe-<version>-osx-arm64.zip` |
| Mac with Intel chip | `ImageDupe-<version>-osx-x64.zip` |

No installer and nothing else to install — .NET is bundled inside.

**Windows:** unzip, run `ImageDupe.exe`. If SmartScreen says "Windows protected your PC",
click **More info → Run anyway** (the app isn't code-signed yet).

**Mac:** unzip, drag **Image Dupe Identifier** to Applications. The first time, open
**System Settings → Privacy & Security**, scroll down, and click **Open Anyway**
(the app isn't notarized by Apple yet).

## Build from source

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/ImageDupe.App
```

## Repository layout

```
src/ImageDupe.Core/        Engine: scanning, hashing, duplicate grouping. No UI. Fully unit-tested.
src/ImageDupe.App/         Desktop app (Avalonia UI, MVVM). Windows + Mac from one codebase.
tests/ImageDupe.Core.Tests xUnit tests for the engine.
build/                     macOS .app packaging.
.github/workflows/         CI (every push), CodeQL security scan, Release (on version tags).
Directory.Build.props      Shared build + security settings for every project.
Directory.Packages.props   Every NuGet package version, in one place.
BannedSymbols.txt          APIs the compiler refuses to allow (network, process launch, hard delete).
```

Developer setup and release process: [docs/SETUP.md](docs/SETUP.md).

## License

[MIT](LICENSE)
