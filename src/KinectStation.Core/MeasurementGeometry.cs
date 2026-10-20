// Kinect Station research software
// Copyright (c) Amin Amini.
// SPDX-License-Identifier: Apache-2.0
// Associated publication: https://doi.org/10.1109/HORA61326.2024.10550801

using System;

namespace KinectStation.Core
{
    public static class MeasurementGeometry
    {
        public static double Distance(Point3D a, Point3D b)
        {
            double dx = b.X - a.X;
            double dy = b.Y - a.Y;
            double dz = b.Z - a.Z;

            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static double AngleDegrees(
            Point3D vertex,
            Point3D pointB,
            Point3D pointC)
        {
            double ux = pointB.X - vertex.X;
            double uy = pointB.Y - vertex.Y;
            double uz = pointB.Z - vertex.Z;

            double vx = pointC.X - vertex.X;
            double vy = pointC.Y - vertex.Y;
            double vz = pointC.Z - vertex.Z;

            double uLength = Math.Sqrt(ux * ux + uy * uy + uz * uz);
            double vLength = Math.Sqrt(vx * vx + vy * vy + vz * vz);

            if (uLength == 0.0 || vLength == 0.0)
            {
                throw new ArgumentException(
                    "The angle requires two non-zero vectors from the vertex.");
            }

            double normalizedDot =
                (ux * vx + uy * vy + uz * vz) /
                (uLength * vLength);

            normalizedDot =
                Math.Max(-1.0, Math.Min(1.0, normalizedDot));

            return Math.Acos(normalizedDot) * 180.0 / Math.PI;
        }

        public static double QuadrilateralPerimeter(
            Point3D a,
            Point3D b,
            Point3D c,
            Point3D d)
        {
            return
                Distance(a, b) +
                Distance(b, c) +
                Distance(c, d) +
                Distance(d, a);
        }

        public static double QuadrilateralArea(
            Point3D a,
            Point3D b,
            Point3D c,
            Point3D d)
        {
            double diagonalAc = Distance(a, c);

            double firstTriangle = TriangleAreaFromSides(
                Distance(a, b),
                Distance(b, c),
                diagonalAc);

            double secondTriangle = TriangleAreaFromSides(
                diagonalAc,
                Distance(c, d),
                Distance(d, a));

            return firstTriangle + secondTriangle;
        }

        public static double TriangleAreaFromSides(
            double a,
            double b,
            double c)
        {
            if (a <= 0.0 || b <= 0.0 || c <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(a),
                    "Triangle side lengths must be greater than zero.");
            }

            double semiPerimeter = (a + b + c) / 2.0;
            double radicand =
                semiPerimeter *
                (semiPerimeter - a) *
                (semiPerimeter - b) *
                (semiPerimeter - c);

            if (radicand < -1e-12)
            {
                throw new ArgumentException(
                    "The supplied side lengths do not form a valid triangle.");
            }

            return Math.Sqrt(Math.Max(0.0, radicand));
        }
    }
}
