# 坐标系统

`CoordinateSystem` 类位于 `AstroAlgo.Basic` 命名空间下，提供了赤道、黄道坐标的转换以及坐标参数的计算。

## 章动

```C#
Nutation nutation = CoordinateSystem.GetNutation(DateTime.Now);
```

## 角度的计算

```C#
Equator e = new Equator
{
    RA = 90,
    Dec = 10
};

// 黄赤交角
double eo = CoordinateSystem.GetEclipticObliquity(DateTime.Now);
// 时角
double ha = CoordinateSystem.GetHourAngle(DateTime.Now, e.RA, longitude: 110.25);
// 视差角
double pa = CoordinateSystem.GetParallacticAngle(DateTime.Now, e, latitude: 35, longitude: 110.25);
// 仰角
double ea = CoordinateSystem.GetElevationAngle(DateTime.Now, e, latitude: 35, longitude: 110.25);
// 方位角
double az = CoordinateSystem.GetAzimuth(DateTime.Now, e, latitude: 35, longitude: 110.25);
// 中天仰角
double cul = CoordinateSystem.GetCulminationAngle(e, latitude: 35);
```

## 黄赤坐标转换

```C#
Equator equator = new Equator();
Ecliptic ecliptic = new Ecliptic();

// 赤道坐标转黄道坐标
Ecliptic newEcliptic = CoordinateSystem.Equator2Ecliptic(equator);
// 黄道坐标转赤道坐标
Equator newEquator = CoordinateSystem.Ecliptic2Equator(ecliptic);
```

## 其他

```C#
// 角距离
double as = CoordinateSystem.GetAngularSeparation(equator1, equator2);
// 获取仰角对应的区时
double[] times = CoordinateSystem.ElevationAngle2Time(equator, angle: 65, latitude: 35, longitude: 110.25, date: DateTime.Now, localTimeZone: TimeZoneInfo.Local);
```