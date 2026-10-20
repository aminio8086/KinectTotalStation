# Validation status

## Static/source validation

The cleaned release separates geometry from Kinect acquisition so the mathematical operations can be tested without sensor hardware.

The core smoke tests cover:

- Euclidean 3-D distance;
- 90-degree angle;
- 180-degree angle;
- square perimeter;
- square area;
- historical pixel/depth projection;
- degenerate-angle rejection;
- invalid depth/pixel input rejection.

## Automated CI

GitHub Actions builds `KinectStation.Core` and runs the smoke tests on:

- Ubuntu;
- Windows.

## What CI does not validate

Hosted CI does not include the Microsoft Kinect for Windows SDK v2 or physical Kinect v2 hardware.

The following therefore still require a Windows/Kinect hardware pass:

- Kinect sensor discovery;
- live depth-frame acquisition;
- reliable-depth filtering;
- WPF rendering;
- mouse-to-depth-pixel selection;
- physical measurement accuracy.

## Research-result boundary

This repository does not claim that the smoke tests reproduce the empirical accuracy figures from the 2024 paper.

The tests validate the cleaned software equations and state flow. Reproducing the paper's physical accuracy experiment would require the original physical targets, distances, Kinect setup and measurement protocol.
