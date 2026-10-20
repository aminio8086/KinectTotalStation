# Building Kinect Station

## Core library and tests

The portable measurement core targets .NET Standard 2.0, while the smoke-test executable targets .NET 8.

Requirements:

- .NET 8 SDK.

Build and run:

```bash
dotnet restore KinectStation.sln
dotnet build src/KinectStation.Core/KinectStation.Core.csproj --configuration Release
dotnet run --project tests/KinectStation.Core.SmokeTests --configuration Release
```

## WPF application

The Kinect application is intentionally Windows-only.

Requirements:

- Windows 10 or Windows 11;
- Visual Studio with .NET Framework 4.8 development tools;
- Microsoft Kinect for Windows SDK v2;
- Kinect v2 sensor;
- `KINECTSDK20_DIR` configured by the Kinect SDK installation.

Open `KinectStation.sln` in Visual Studio and build `KinectStation.Wpf`.

The project references:

```text
$(KINECTSDK20_DIR)Assemblies\Microsoft.Kinect.dll
```

The Kinect SDK assembly is not committed to this repository.

## CI scope

GitHub Actions builds the portable core and runs its smoke tests on Windows and Linux.

The WPF project is not built in hosted CI because GitHub runners do not include the Kinect for Windows SDK v2 and no live sensor is available.

## Runtime use

1. Connect and power the Kinect v2 sensor.
2. Start the WPF application.
3. Wait for the status to show that the sensor is available.
4. Left-click points on the depth view.
5. Right-click to reset the current selection.

The application only accepts depth pixels inside the Kinect reliable depth range.
