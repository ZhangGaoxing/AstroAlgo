# Sidereal Time

The `SiderealTime` class is located in the `AstroAlgo.Basic` namespace. It provides a series of methods for sidereal time.

## Calculation of Sidereal Time

```C#
// Calculate Greenwich mean sidereal time
double ut = SiderealTime.UtMeanSiderealTime(DateTime.Now);
// Calculate local mean sidereal time
double local = SiderealTime.LocalMeanSiderealTime(localTime: DateTime.Now, localTimeZone: TimeZoneInfo.Local, longitude: 110.25);
```

## Conversion of Sidereal Time

```C#
// Convert local mean sidereal time to zone time
double time = SiderealTime.SiderealTime2ZoneTime(localSiderealTime: local, date: DateTime.Now, localTimeZone: TimeZoneInfo.Local, longitude: 110.25);
// Calculate the difference between dynamical time and universal time
double diff = SiderealTime.DifferenceDtUt(DateTime.Now);
```