# Sun

The `Sun` class is located in the `AstroAlgo.SolarSystem` namespace. It derives from the abstract class `Star`, and its properties inherit the virtual implementation in `Star`.

## Constructor

```C#
// The default value of latitude and longitude of observation site is 0, the default time is the current system time, and the default time zone is the current system time zone.
Sun sun1 = new Sun();
// The longitude and latitude of the observation site is specified. The default time is the current system time, and the default time zone is the current system time zone.
Sun sun2 = new Sun(latitude: 35, longitude: 110.25);
// Constructor for full arguments
Sun sun3 = new Sun(latitude: 35, longitude: 110.25, localTime: new DateTime(2019, 10, 1, 10, 1, 0), localTimeZone: TimeZoneInfo.Utc);
```

## Location and Time

```C#
sun.Latitude = 35;
sun.Longitude = 110.25;
sun.DateTime = new DateTime(2019, 10, 1, 10, 1, 0);
sun.LocalTimeZone = TimeZoneInfo.Utc;
```

## Rising, Culmination and Setting

```C#
TimeSpan rising = sun.Rising;
TimeSpan culmination = sun.Culmination;
TimeSpan setting = sun.Setting;
```

## Equator and Ecliptic Coordinate

```C#
// Coordinates are calculated based on the DateTime of the property
Equator equator1 = sun.Equator;
Ecliptic ecliptic1 = sun.Ecliptic;
// Calculates the coordinates of the specified time
Equator equator2 = sun.GetEquatorCoordinate(DateTime.Now);
Ecliptic ecliptic2 = sun.GetEclipticCoordinate(DateTime.Now);
```

## Distance

```C#
// Distance between Earth and Sun
double toEarth1 = sun.ToEarth;
double toEarth2 = sun.GetToEarthDistance(DateTime.Now);
```

## Angle

```C#
double ea = sun.ElevationAngle;
double az = sun.Azimuth;
```

## Equinox, Solstice and 24 Solar Terms

```C#
// Equinox and solstice
DateTime[] es = sun.GetEquinoxAndSolstice(2020, TimeZoneInfo.Local);
// 24 Solar Terms
DateTime date = sun.GetSolarTerms(2020, SolarTerm.BeginningOfSpring, TimeZoneInfo.Local);
```
