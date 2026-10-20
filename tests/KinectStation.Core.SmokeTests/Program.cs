// Kinect Station research software
// Copyright (c) Amin Amini.
// SPDX-License-Identifier: Apache-2.0
// Associated publication: https://doi.org/10.1109/HORA61326.2024.10550801

using KinectStation.Core;

try
{
    TestDistance();
    TestRightAngle();
    TestStraightAngle();
    TestSquarePerimeter();
    TestSquareArea();
    TestHistoricalProjection();
    TestDegenerateAngleRejected();
    TestInvalidProjectionRejected();

    Console.WriteLine("All Kinect Station core smoke tests passed.");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
    return 1;
}

static void TestDistance()
{
    double distance = MeasurementGeometry.Distance(
        new Point3D(0.0, 0.0, 0.0),
        new Point3D(3.0, 4.0, 0.0));

    AssertNear(5.0, distance, 1e-10, "3-4-5 distance");
}

static void TestRightAngle()
{
    double angle = MeasurementGeometry.AngleDegrees(
        new Point3D(0.0, 0.0, 0.0),
        new Point3D(1.0, 0.0, 0.0),
        new Point3D(0.0, 1.0, 0.0));

    AssertNear(90.0, angle, 1e-10, "Right angle");
}

static void TestStraightAngle()
{
    double angle = MeasurementGeometry.AngleDegrees(
        new Point3D(0.0, 0.0, 0.0),
        new Point3D(1.0, 0.0, 0.0),
        new Point3D(-1.0, 0.0, 0.0));

    AssertNear(180.0, angle, 1e-10, "Straight angle");
}

static void TestSquarePerimeter()
{
    double perimeter =
        MeasurementGeometry.QuadrilateralPerimeter(
            new Point3D(0.0, 0.0, 0.0),
            new Point3D(1.0, 0.0, 0.0),
            new Point3D(1.0, 1.0, 0.0),
            new Point3D(0.0, 1.0, 0.0));

    AssertNear(4.0, perimeter, 1e-10, "Unit-square perimeter");
}

static void TestSquareArea()
{
    double area =
        MeasurementGeometry.QuadrilateralArea(
            new Point3D(0.0, 0.0, 0.0),
            new Point3D(1.0, 0.0, 0.0),
            new Point3D(1.0, 1.0, 0.0),
            new Point3D(0.0, 1.0, 0.0));

    AssertNear(1.0, area, 1e-10, "Unit-square area");
}

static void TestHistoricalProjection()
{
    Point3D point =
        HistoricalProjection.ToCartesianMetres(
            100,
            200,
            2000);

    AssertNear(
        (2000.0 *
            (100.0 - HistoricalProjection.HistoricalHorizontalPixelOffset) /
            HistoricalProjection.HorizontalFocalValue) /
        1000.0,
        point.X,
        1e-12,
        "Historical X projection");

    AssertNear(
        (2000.0 * 200.0 / HistoricalProjection.VerticalFocalValue) / 1000.0,
        point.Y,
        1e-12,
        "Historical Y projection");

    AssertNear(2.0, point.Z, 1e-12, "Historical Z projection");
}

static void TestDegenerateAngleRejected()
{
    bool rejected = false;

    try
    {
        MeasurementGeometry.AngleDegrees(
            new Point3D(0.0, 0.0, 0.0),
            new Point3D(0.0, 0.0, 0.0),
            new Point3D(1.0, 0.0, 0.0));
    }
    catch (ArgumentException)
    {
        rejected = true;
    }

    Assert(rejected, "Degenerate angle should be rejected.");
}

static void TestInvalidProjectionRejected()
{
    bool rejected = false;

    try
    {
        HistoricalProjection.ToCartesianMetres(
            -1,
            0,
            1000);
    }
    catch (ArgumentOutOfRangeException)
    {
        rejected = true;
    }

    Assert(rejected, "Out-of-range projection input should be rejected.");
}

static void AssertNear(
    double expected,
    double actual,
    double tolerance,
    string message)
{
    if (Math.Abs(expected - actual) > tolerance)
    {
        throw new InvalidOperationException(
            $"{message}: expected {expected}, actual {actual}.");
    }
}

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
