# Changelog

All notable changes to this cleaned research-software release will be documented here.

## [Unreleased]

### Added

- Portable measurement core independent of the Kinect SDK.
- Euclidean 3-D distance calculation.
- Relative-angle calculation using normalized vectors and dot product.
- Quadrilateral perimeter calculation.
- Quadrilateral area calculation by splitting into triangles ABC and ACD and applying Heron's formula.
- Historical Kinect depth-to-Cartesian projection constants and equation.
- Clean Windows WPF Kinect v2 front end.
- Dependency-free core smoke tests.
- Windows and Linux CI for the portable measurement core.
- Build, algorithm, provenance and validation documentation.
- Apache-2.0 licensing and machine-readable citation metadata.

### Changed

- Reconstructed the release from the private `KinectTotalStation` research repository.
- Replaced the old Microsoft sample namespace/project identity with `KinectStation`.
- Separated sensor/UI code from measurement mathematics.
- Removed the WPF layout-specific horizontal coordinate offset from the measurement projection.
- Kept points C and D distinct when the fourth point is selected, correcting the historical point-C overwrite before area calculation.
- Removed depth-buffer text-file export from the distance-calculation path.
- Removed generated, compiled and IDE-specific files from the release.

### Not included

- Microsoft Kinect SDK binaries.
- Historical build outputs.
- Historical `depthZData.txt` experiment output.
- Approximate-volume functionality, because no active volume-calculation implementation was found in the recovered project.
