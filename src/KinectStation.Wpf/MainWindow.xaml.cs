// Kinect Station research software
// Copyright (c) Amin Amini.
// SPDX-License-Identifier: Apache-2.0
// Associated publication: https://doi.org/10.1109/HORA61326.2024.10550801

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KinectStation.Core;
using Microsoft.Kinect;

namespace KinectStation
{
    public partial class MainWindow : Window
    {
        private readonly KinectSensor _sensor;
        private readonly DepthFrameReader _depthReader;
        private readonly FrameDescription _depthDescription;
        private readonly WriteableBitmap _depthBitmap;
        private readonly ushort[] _depthData;
        private readonly byte[] _depthPixels;
        private readonly List<Point3D> _selectedPoints =
            new List<Point3D>(4);
        private readonly List<Point> _selectedPixels =
            new List<Point>(4);

        private ushort _minimumReliableDepth;
        private ushort _maximumReliableDepth;

        public MainWindow()
        {
            InitializeComponent();

            _sensor = KinectSensor.GetDefault();

            if (_sensor == null)
            {
                StatusText.Text = "No Kinect v2 sensor found.";
                return;
            }

            _depthDescription =
                _sensor.DepthFrameSource.FrameDescription;

            _depthData =
                new ushort[_depthDescription.LengthInPixels];

            _depthPixels =
                new byte[_depthDescription.LengthInPixels];

            _depthBitmap = new WriteableBitmap(
                _depthDescription.Width,
                _depthDescription.Height,
                96.0,
                96.0,
                PixelFormats.Gray8,
                null);

            DepthImage.Source = _depthBitmap;

            _depthReader =
                _sensor.DepthFrameSource.OpenReader();

            _depthReader.FrameArrived += DepthReader_FrameArrived;
            _sensor.IsAvailableChanged += Sensor_IsAvailableChanged;

            _sensor.Open();
            UpdateSensorStatus();
        }

        private void DepthReader_FrameArrived(
            object sender,
            DepthFrameArrivedEventArgs e)
        {
            using (DepthFrame frame = e.FrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return;
                }

                frame.CopyFrameDataToArray(_depthData);

                _minimumReliableDepth = frame.DepthMinReliableDistance;
                _maximumReliableDepth = frame.DepthMaxReliableDistance;

                for (int i = 0; i < _depthData.Length; i++)
                {
                    ushort depth = _depthData[i];

                    if (depth < _minimumReliableDepth ||
                        depth > _maximumReliableDepth)
                    {
                        _depthPixels[i] = 0;
                        continue;
                    }

                    _depthPixels[i] =
                        (byte)Math.Min(255, depth / 31);
                }

                _depthBitmap.WritePixels(
                    new Int32Rect(
                        0,
                        0,
                        _depthDescription.Width,
                        _depthDescription.Height),
                    _depthPixels,
                    _depthDescription.Width,
                    0);
            }
        }

        private void Sensor_IsAvailableChanged(
            object sender,
            IsAvailableChangedEventArgs e)
        {
            UpdateSensorStatus();
        }

        private void UpdateSensorStatus()
        {
            StatusText.Text =
                _sensor != null && _sensor.IsAvailable
                    ? "Kinect v2 available"
                    : "Kinect v2 unavailable";
        }

        private void DepthImage_MouseMove(
            object sender,
            MouseEventArgs e)
        {
            Point pixel = e.GetPosition(DepthImage);

            if (!TryGetDepthPixel(pixel, out int x, out int y, out ushort depth))
            {
                PointerText.Text = "No reliable depth at pointer";
                return;
            }

            Point3D point =
                HistoricalProjection.ToCartesianMetres(
                    x,
                    y,
                    depth);

            PointerText.Text =
                string.Format(
                    "Pixel ({0}, {1}), depth {2:F3} m, point ({3:F3}, {4:F3}, {5:F3}) m",
                    x,
                    y,
                    depth / 1000.0,
                    point.X,
                    point.Y,
                    point.Z);
        }

        private void DepthImage_MouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (_selectedPoints.Count == 4)
            {
                ResetMeasurement();
            }

            Point pixel = e.GetPosition(DepthImage);

            if (!TryGetDepthPixel(pixel, out int x, out int y, out ushort depth))
            {
                return;
            }

            Point3D point =
                HistoricalProjection.ToCartesianMetres(
                    x,
                    y,
                    depth);

            _selectedPoints.Add(point);
            _selectedPixels.Add(new Point(x, y));

            DrawSelection();
            UpdateMeasurements();
        }

        private void DepthImage_MouseRightButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            ResetMeasurement();
        }

        private bool TryGetDepthPixel(
            Point pixel,
            out int x,
            out int y,
            out ushort depth)
        {
            x = (int)Math.Floor(pixel.X);
            y = (int)Math.Floor(pixel.Y);
            depth = 0;

            if (x < 0 ||
                x >= HistoricalProjection.FrameWidth ||
                y < 0 ||
                y >= HistoricalProjection.FrameHeight)
            {
                return false;
            }

            int index = y * HistoricalProjection.FrameWidth + x;

            if (_depthData == null || index >= _depthData.Length)
            {
                return false;
            }

            depth = _depthData[index];

            return
                depth != 0 &&
                depth >= _minimumReliableDepth &&
                depth <= _maximumReliableDepth;
        }

        private void UpdateMeasurements()
        {
            PointsText.Text =
                string.Format("{0} / 4", _selectedPoints.Count);

            DistanceText.Text = "—";
            AngleText.Text = "—";
            PerimeterText.Text = "—";
            AreaText.Text = "—";

            if (_selectedPoints.Count >= 2)
            {
                DistanceText.Text =
                    MeasurementGeometry.Distance(
                        _selectedPoints[0],
                        _selectedPoints[1])
                    .ToString("F4");
            }

            if (_selectedPoints.Count >= 3)
            {
                AngleText.Text =
                    MeasurementGeometry.AngleDegrees(
                        _selectedPoints[0],
                        _selectedPoints[1],
                        _selectedPoints[2])
                    .ToString("F3");
            }

            if (_selectedPoints.Count == 4)
            {
                PerimeterText.Text =
                    MeasurementGeometry.QuadrilateralPerimeter(
                        _selectedPoints[0],
                        _selectedPoints[1],
                        _selectedPoints[2],
                        _selectedPoints[3])
                    .ToString("F4");

                AreaText.Text =
                    MeasurementGeometry.QuadrilateralArea(
                        _selectedPoints[0],
                        _selectedPoints[1],
                        _selectedPoints[2],
                        _selectedPoints[3])
                    .ToString("F4");
            }
        }

        private void DrawSelection()
        {
            Overlay.Children.Clear();

            for (int i = 0; i < _selectedPixels.Count; i++)
            {
                Point point = _selectedPixels[i];

                var marker = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = Brushes.Red
                };

                Canvas.SetLeft(marker, point.X - 4);
                Canvas.SetTop(marker, point.Y - 4);
                Overlay.Children.Add(marker);

                if (i == 0)
                {
                    continue;
                }

                DrawLine(
                    _selectedPixels[i - 1],
                    point);
            }

            if (_selectedPixels.Count == 4)
            {
                DrawLine(
                    _selectedPixels[3],
                    _selectedPixels[0]);
            }
        }

        private void DrawLine(Point start, Point end)
        {
            Overlay.Children.Add(
                new Line
                {
                    X1 = start.X,
                    Y1 = start.Y,
                    X2 = end.X,
                    Y2 = end.Y,
                    Stroke = Brushes.LimeGreen,
                    StrokeThickness = 2
                });
        }

        private void ResetMeasurement()
        {
            _selectedPoints.Clear();
            _selectedPixels.Clear();
            Overlay.Children.Clear();

            PointsText.Text = "0 / 4";
            DistanceText.Text = "—";
            AngleText.Text = "—";
            PerimeterText.Text = "—";
            AreaText.Text = "—";
        }

        private void Window_Closing(
            object sender,
            CancelEventArgs e)
        {
            if (_depthReader != null)
            {
                _depthReader.FrameArrived -=
                    DepthReader_FrameArrived;

                _depthReader.Dispose();
            }

            if (_sensor != null)
            {
                _sensor.IsAvailableChanged -=
                    Sensor_IsAvailableChanged;

                if (_sensor.IsOpen)
                {
                    _sensor.Close();
                }
            }
        }
    }
}
