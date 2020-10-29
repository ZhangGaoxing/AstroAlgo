# 恒星时

`SiderealTime` 类位于 `AstroAlgo.Basic` 命名空间下，提供了恒星时的一系列操作。

## 恒星时的计算

```C#
// 计算格林尼治平恒星时
double ut = SiderealTime.UtMeanSiderealTime(DateTime.Now);
// 计算指定观测点的平恒星时
double local = SiderealTime.LocalMeanSiderealTime(localTime: DateTime.Now, localTimeZone: TimeZoneInfo.Local, longitude: 110.25);
```

## 恒星时的转换

```C#
// 恒星时转指定观测点的区时
double time = SiderealTime.SiderealTime2ZoneTime(localSiderealTime: local, date: DateTime.Now, localTimeZone: TimeZoneInfo.Local, longitude: 110.25);
// 力学时与恒星时之差
double diff = SiderealTime.DifferenceDtUt(DateTime.Now);
```