// Kinect Station research software
// Copyright (c) Amin Amini.
// SPDX-License-Identifier: Apache-2.0
// Associated publication: https://doi.org/10.1109/HORA61326.2024.10550801

using System;

namespace KinectStation.Core
{
    /// <summary>
    /// Reproduces the active pixel/depth-to-Cartesian equations recovered from
    /// the historical KinectTotalStation measurement path.
    /// </summary>
    public static class HistoricalProjection
    {
        public const int FrameWidth = 512;
        public const int FrameHeight = 424;
        public const double HorizontalFocalValue = 365.605;
        public const double VerticalFocalValue = 367.194;

        public static Point3D ToCartesianMetres(
            int pixelX,
            int pixelY,
            ushort depthMillimetres)
        {
            if (pixelX < 0 || pixelX >= FrameWidth)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pixelX),
                    "Pixel X must be inside the 512-pixel depth frame.");
            }

            if (pixelY < 0 || pixelY >= FrameHeight)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pixelY),
                    "Pixel Y must be inside the 424-pixel depth frame.");
            }

            if (depthMillimetres == 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(depthMillimetres),
                    "Depth must be greater than zero.");
            }

            double x =
                (depthMillimetres * pixelX / HorizontalFocalValue) / 1000.0;

            double y =
                (depthMillimetres * pixelY / VerticalFocalValue) / 1000.0;

            double z = depthMillimetres / 1000.0;

            return new Point3D(x, y, z);
        }
    }
}
