# Historical source provenance

## Source repository

The cleaned release was reconstructed from:

```text
aminio8086/KinectTotalStation
```

The historical private repository remains unchanged.

The project contains a Kinect v2 WPF application with:

- Kinect depth acquisition;
- `MapDepthFrameToCameraSpace` use for camera-space inspection;
- fixed focal-value projection used by the interactive measurement path;
- 3-D Euclidean distance;
- three-point relative-angle calculation;
- four-edge perimeter;
- quadrilateral area using a two-triangle/Heron approach.

The README in that repository uses the same "Kinect Station" title as the 2024 publication.

## Research-preparation history

The historical Git history includes 2023 commits labelled:

- `UCLan update`;
- `Fixed and ready for research`;
- `Fixed AngleCalculator`.

The final of those corrected a degree/radian issue around the 30-degree focal-length calculation.

These commits pre-date the 2024 HORA publication and provide strong provenance for the repository's relationship to the paper.

## Point-C overwrite in the historical UI

Across the recovered 2023 revisions, the fourth-point code performs the equivalent of:

```text
distance CD = distance(point 3, point 4)
pointC = point 4
pointD = point 4
diagonal = distance(pointA, pointC)
```

That makes C and D identical immediately before the diagonal/area calculation.

This conflicts with both:

- the code comment stating that the quadrilateral is cut into two triangles; and
- the intended edge naming AB, BC, CD and DA.

The cleaned release treats this as a point-bookkeeping defect and keeps C as the third selected point and D as the fourth. The area is therefore calculated from triangles ABC and ACD using diagonal AC.

This correction is documented explicitly rather than being presented as verbatim historical behaviour.

## Depth-file side effect

The historical `DistanceCalculator` computes the Euclidean distance and then serializes the full depth-Z buffer to `depthZData.txt`.

That file export is unrelated to the distance equation and can create a very large output file during normal interaction.

The cleaned distance method contains no I/O side effects.

## Historical horizontal correction

The recovered measurement path repeatedly uses `clickPoint.X - 20` before calculating the X coordinate.

Although the old XAML also contains layout margins, the source does not establish that the 20-pixel correction is purely a presentation offset. To avoid changing the recovered research behaviour on an unsupported assumption, the cleaned release preserves it as `HistoricalHorizontalPixelOffset = 20`.

This is intentionally documented as a historical implementation constant rather than a newly justified calibration parameter.

## Microsoft sample lineage

The private project retains identifiers such as:

```text
Microsoft.Samples.Kinect.DepthBasics
DepthBasics-WPF
```

and a Microsoft copyright notice in `MainWindow.xaml.cs`.

The cleaned release uses its own `KinectStation` project/namespace and does not redistribute the Microsoft Kinect SDK DLL.

Microsoft Kinect sample and SDK lineage is acknowledged in `NOTICE`.

## Generated and binary files excluded

The historical repository contains development artefacts including:

- `.vs/`;
- `bin/`;
- `obj/`;
- `.exe`;
- `.pdb`;
- a copied `Microsoft.Kinect.dll`;
- generated depth data.

They are intentionally excluded from the cleaned source release.

## Volume

The old README mentions approximate volume as a research aim. No active volume-calculation method or volume UI was found in the recovered application.

The cleaned release therefore documents and implements only the measurements supported by the recovered active source: distance, relative angle, area and perimeter.
