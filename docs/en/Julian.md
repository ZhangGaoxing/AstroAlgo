# Julian Day

The `Julian` class is located in the `AstroAlgo.Basic` namespace. It provides a series of methods for Julian day.

## The Conversion of Julian Day and Calendar Day

```C#
DateTime time = new DateTime(2019, 10, 1, 10, 1, 0);

// Calendar to Julian
// isModified: Is it a simplified Julian day
double julian = Julian.ToJulianDay(time, isModified: false);
// Julian to Calendar
DateTime time = Julian.ToCalendarDay(julianDay: julian, isModified: false)
```

## The Interval of Time

```C#
DateTime time1 = new DateTime(2020, 10, 1, 10, 1, 0);
DateTime time2 = new DateTime(2019, 10, 1, 10, 1, 0);

// Calculate the number of days between the given date
double interval = Julian.Date2Interval(time1, time2);
// Calculate the date for the given interval of days
DateTime time = Julian.Interval2Date(time2, interval: 366);
```

## 日期的位置

```C#
DateTime date = new DateTime(2019, 10, 1, 10, 1, 0);

// Calculate the day of the week for the given date
DayOfWeek dayOfWeek = Julian.GetDayOfWeek(date);
// Calculate the day of the year (ordinal day) for the given date
int dayOfYear = Julian.GetDayOfYear(date);
```