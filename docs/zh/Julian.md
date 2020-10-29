# 儒略日

`Julian` 类位于 `AstroAlgo.Basic` 命名空间下，提供了儒略日的一系列操作。

## 儒略日和历日的转换

```C#
DateTime time = new DateTime(2019, 10, 1, 10, 1, 0);

// 历日转儒略日
// time 为要转换的历日；isModified 标识是否为简化儒略日
double julian = Julian.ToJulianDay(time, isModified: false);
// 儒略日转历日
DateTime time = Julian.ToCalendarDay(julianDay: julian, isModified: false)
```

## 时间的间隔

```C#
DateTime time1 = new DateTime(2020, 10, 1, 10, 1, 0);
DateTime time2 = new DateTime(2019, 10, 1, 10, 1, 0);

// 获取两个时间的间隔天数
double interval = Julian.Date2Interval(time1, time2);
// 根据间隔天数计算新的时间
DateTime time = Julian.Interval2Date(time2, interval: 366);
```

## 日期的位置

```C#
DateTime date = new DateTime(2019, 10, 1, 10, 1, 0);

// 获取日期是周几
DayOfWeek dayOfWeek = Julian.GetDayOfWeek(date);
// 获取日期是该年的第几天
int dayOfYear = Julian.GetDayOfYear(date);
```