# Coordinate System

The `CoordinateSystem` class is located in the `AstroAlgo.Basic` namespace. It provides the conversion of equator and ecliptic coordinates and the calculation of coordinate parameters.

## Nutation

```C#
Nutation nutation = CoordinateSystem.GetNutation(DateTime.Now);
```

## Calculation of Angle

```C#
Equator e = new Equator
{
    RA = 90,
    Dec = 10
};

// Ecliptic obliquity
double eo = CoordinateSystem.GetEclipticObliquity(DateTime.Now);
// Hour angle
double ha = CoordinateSystem.GetHourAngle(DateTime.Now, e.RA, longitude: 110.25);
// Parallactic angle
double pa = CoordinateSystem.GetParallacticAngle(DateTime.Now, e, latitude: 35, longitude: 110.25);
// Elevation angle
double ea = CoordinateSystem.GetElevationAngle(DateTime.Now, e, latitude: 35, longitude: 110.25);
// Azimuth
double az = CoordinateSystem.GetAzimuth(DateTime.Now, e, latitude: 35, longitude: 110.25);
// Culmination angle
double cul = CoordinateSystem.GetCulminationAngle(e, latitude: 35);
```

## Coordinate Conversion Between Equator and Ecliptic

```C#
Equator equator = new Equator();
Ecliptic ecliptic = new Ecliptic();

// Equator to ecliptic
Ecliptic newEcliptic = CoordinateSystem.Equator2Ecliptic(equator);
// Ecliptic to equator
Equator newEquator = CoordinateSystem.Ecliptic2Equator(ecliptic);
```

## Others

```C#
// Angular separation
double as = CoordinateSystem.GetAngularSeparation(equator1, equator2);
// Elevation angle to local time
double[] times = CoordinateSystem.ElevationAngle2Time(equator, angle: 65, latitude: 35, longitude: 110.25, date: DateTime.Now, localTimeZone: TimeZoneInfo.Local);
```