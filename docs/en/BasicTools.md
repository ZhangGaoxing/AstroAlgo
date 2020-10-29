# Basic Tools

The `BasicTools` class is located in the `AstroAlgo.Basic` namespace. It provides the basic calculation functions required by the program.

## Simplification of Angle

```C#
double angle = 360.11;
double simplified = BasicTools.SimplifyAngle(angle);
```

## Conversion of Angle

```C#
double angle = 95.6;

// Angle converted to degrees-minutes-seconds
(int degrees, int minutes, double seconds) dms = BasicTools.Angle2DMS(angle);
// Angle converted to hours-minutes-seconds
(int hours, int minutes, double seconds) hms = BasicTools.Angle2HMS(angle);
```