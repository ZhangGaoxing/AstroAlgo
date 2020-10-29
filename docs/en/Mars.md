# Mars

The `Mars` class is located in the `AstroAlgo.SolarSystem` namespace. It derives from the abstract class `Planet`, and its properties inherit the virtual implementation in `Planet`.

## Constructor

```C#
// The default value of latitude and longitude of observation site is 0, the default time is the current system time, and the default time zone is the current system time zone.
Mars mars1 = new Mars();
// The longitude and latitude of the observation site is specified. The default time is the current system time, and the default time zone is the current system time zone.
Mars mars2 = new Mars(latitude: 35, longitude: 110.25);
// Constructor for full arguments
Mars mars3 = new Mars(latitude: 35, longitude: 110.25, localTime: new DateTime(2019, 10, 1, 10, 1, 0), localTimeZone: TimeZoneInfo.Utc);
```

## Location and Time

```C#
mars.Latitude = 35;
mars.Longitude = 110.25;
mars.DateTime = new DateTime(2019, 10, 1, 10, 1, 0);
mars.LocalTimeZone = TimeZoneInfo.Utc;
```

## Rising, Culmination and Setting

```C#
TimeSpan rising = mars.Rising;
TimeSpan culmination = mars.Culmination;
TimeSpan setting = mars.Setting;
```

## Equator and Ecliptic Coordinate

```C#
// Coordinates are calculated based on the DateTime of the property
Equator equator1 = mars.Equator;
Ecliptic ecliptic1 = mars.Ecliptic;
// Calculates the coordinates of the specified time
Equator equator2 = mars.GetEquatorCoordinate(DateTime.Now);
Ecliptic ecliptic2 = mars.GetEclipticCoordinate(DateTime.Now);
// Calculates the heliocentric ecliptic coordinate
Ecliptic ecliptic3 = mars.GetHeliocentricEclipticCoordinate(DateTime.Now);
```

## Distance

```C#
// Distance between Earth and Mars
double toEarth1 = mars.ToEarth;
double toEarth2 = mars.GetToEarthDistance(DateTime.Now);
// Distance between Sun and Mars
double toSun1 = mars.ToSun;
double toSun2 = mars.GetToSunDistance(DateTime.Now);
```

## Angle

```C#
double ea = mars.ElevationAngle;
double az = mars.Azimuth;
```

## Orbital Element
```C#
OrbitalElement element = mars.GetOrbitalElement(DateTime.Now);
```