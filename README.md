# Kinect Station

Cleaned research-software release associated with:

> **Amin Amini**,  
> **"Kinect Station: Using Microsoft Kinect v2 as a Total Station Theodolite for Distance and Angle Determination in a 3D Cartesian Environment"**,  
> 2024 International Congress on Human-Computer Interaction, Optimization and Robotic Applications (HORA).  
> https://doi.org/10.1109/HORA61326.2024.10550801

Kinect Station uses Microsoft Kinect v2 depth data as a low-cost 3-D measurement tool for:

- point-to-point distance;
- relative angle;
- quadrilateral perimeter;
- quadrilateral area.

This repository is a cleaned release reconstructed from the original private research project `KinectTotalStation`. The private historical repository remains unchanged.

## Release design

The historical application mixed Kinect acquisition, WPF drawing, measurement mathematics, generated Visual Studio files, compiled binaries and experimental data in one project.

This release separates those concerns:

```text
src/
├── KinectStation.Core/
│   ├── Point3D.cs
│   ├── MeasurementGeometry.cs
│   └── HistoricalProjection.cs
└── KinectStation.Wpf/
    ├── App.xaml
    ├── App.xaml.cs
    ├── MainWindow.xaml
    └── MainWindow.xaml.cs

tests/
└── KinectStation.Core.SmokeTests/

docs/
├── algorithm.md
├── provenance.md
└── validation.md
```

The core geometry library has no Kinect SDK dependency and can therefore be inspected and tested independently.

## Historical measurement model

The original research code used the Kinect v2 depth frame dimensions and fixed focal values:

- depth frame: **512 × 424**;
- horizontal focal value: **365.605**;
- vertical focal value: **367.194**.

For a selected depth pixel, the historical implementation derived a 3-D point from pixel position and depth. The cleaned release preserves that projection in `HistoricalProjection`, including the recovered 20-pixel horizontal correction because its original rationale is not documented.

The geometric calculations then use Euclidean 3-D distance, vector dot-product angle calculation, perimeter as the sum of four edges, and quadrilateral area as two Heron-formula triangles separated by diagonal AC.

See [docs/algorithm.md](docs/algorithm.md).

## Important historical corrections

The private source contains a few development artefacts and one clear point-bookkeeping defect. These are documented rather than hidden.

Most importantly, on the fourth point the historical UI overwrites `pointC` with the fourth point immediately before calculating the diagonal and area. That conflicts with its own comment that the quadrilateral is split into two triangles. The cleaned release keeps C and D distinct and calculates the intended diagonal AC.

The original `DistanceCalculator` also wrote the full depth-Z buffer to `depthZData.txt` every time a distance was calculated. That side effect is unrelated to the measurement equation and has been removed.

See [docs/provenance.md](docs/provenance.md).

## Building

### Core library and smoke tests

Requires .NET 8 SDK:

```bash
dotnet build KinectStation.sln --configuration Release
dotnet run --project tests/KinectStation.Core.SmokeTests --configuration Release
```

### Kinect v2 WPF application

The WPF application additionally requires:

- Windows;
- .NET Framework 4.8 Developer Pack;
- Microsoft Kinect for Windows SDK v2;
- Kinect v2 sensor and its required adapter/power supply.

The application expects the Kinect SDK environment variable `KINECTSDK20_DIR` to be available.

See [BUILDING.md](BUILDING.md).

## Controls

In the WPF application:

- move the pointer over the depth image to inspect the current depth;
- **left-click** to select points;
- select two points for distance;
- select three points for relative angle;
- select four points for perimeter and area;
- **right-click** to clear the current selection.

## Validation

The portable core is covered by smoke tests for:

- 3-D Euclidean distance;
- right-angle and straight-angle calculations;
- square perimeter;
- square area via the same two-triangle decomposition;
- historical depth-to-Cartesian projection;
- invalid/degenerate geometry handling.

GitHub Actions builds and runs these tests on Windows and Linux.

The Kinect v2 WPF application itself still requires live hardware validation because the Kinect v2 SDK and sensor are not available in GitHub-hosted CI.

## Historical repository hygiene

The cleaned release intentionally excludes:

- `.vs/`;
- `bin/`;
- `obj/`;
- compiled `.exe`, `.dll` and `.pdb` files;
- generated `depthZData.txt`;
- Microsoft Kinect SDK binaries;
- stale Visual Studio caches and generated files.

The Microsoft Kinect SDK is a prerequisite, not redistributed content.

## Citation

If you use this software in scholarly work, please cite the associated paper:

> Amini, A. (2024). "Kinect Station: Using Microsoft Kinect v2 as a Total Station Theodolite for Distance and Angle Determination in a 3D Cartesian Environment."  
> *2024 International Congress on Human-Computer Interaction, Optimization and Robotic Applications (HORA).*  
> https://doi.org/10.1109/HORA61326.2024.10550801

A machine-readable [CITATION.cff](CITATION.cff) is included.

## Licence

Copyright (c) Amin Amini.

The cleaned software in this repository is distributed under the Apache License 2.0. See [LICENSE](LICENSE), [NOTICE](NOTICE) and [COPYRIGHT.md](COPYRIGHT.md).

The associated IEEE publication and Microsoft Kinect SDK are separate copyrighted works and are not redistributed by this repository.
