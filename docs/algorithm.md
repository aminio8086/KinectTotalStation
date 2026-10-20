# Measurement algorithm

## Historical source

The cleaned release is based on the private `KinectTotalStation` project.

The recovered application receives Kinect v2 depth frames at 512 × 424 pixels, stores each selected point in 3-D Cartesian form, and then derives distance, angle, perimeter and area.

## Depth-to-Cartesian projection

The historical source contains fixed focal values:

```text
horizontal focal value = 365.605
vertical focal value   = 367.194
```

For a selected pixel `(x, y)` and depth `d` in millimetres, the active historical equations correspond to:

```text
X = (d * x / 365.605) / 1000
Y = (d * y / 367.194) / 1000
Z = d / 1000
```

The resulting coordinates are in metres.

The old WPF code subtracted 20 pixels from the horizontal screen coordinate because its overlay and image layout were offset. The cleaned UI aligns its image and overlay, so that presentation-specific correction is not part of `HistoricalProjection`.

## Distance

For 3-D points A and B:

```text
distance(A, B) =
    sqrt(
        (Bx - Ax)^2 +
        (By - Ay)^2 +
        (Bz - Az)^2
    )
```

## Relative angle

For three points A, B and C, the historical angle routine forms two vectors originating at A:

```text
u = B - A
v = C - A
```

and computes:

```text
theta = acos((u · v) / (|u| |v|))
```

The result is returned in degrees.

The cleaned implementation clamps the normalized dot product to [-1, 1] before `acos` to protect against floating-point round-off.

## Perimeter

For selected quadrilateral vertices A, B, C and D:

```text
perimeter = AB + BC + CD + DA
```

## Area

The recovered code comments that the quadrilateral is split into two triangles.

The intended decomposition is:

```text
ABC + ACD
```

using diagonal AC.

For a triangle with side lengths `a`, `b` and `c`, Heron's formula is:

```text
s = (a + b + c) / 2
area = sqrt(s(s-a)(s-b)(s-c))
```

The cleaned implementation calculates both triangle areas from the unrounded 3-D distances and adds them.

## Presentation rounding

The historical UI rounded several displayed side lengths to two decimal places before its later area calculation.

The cleaned core keeps full-precision values for calculations and leaves rounding to the user interface. This prevents display formatting from changing the geometric result.
