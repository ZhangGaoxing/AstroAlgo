# 太阳

`Sun` 类位于 `AstroAlgo.SolarSystem` 命名空间下，从抽象类 `Star` 中派生，属性均继承 `Star` 中的虚拟实现。

## 构造函数

```C#
// 无参数的构造函数，观测点经纬度默认为0，时间默认为系统当前时间，时区默认为系统当前时区
Sun sun1 = new Sun();
// 指定了观测点的经纬度，时间默认为系统当前时间，时区默认为系统当前时区
Sun sun2 = new Sun(latitude: 35, longitude: 110.25);
// 完整参数的构造函数
Sun sun3 = new Sun(latitude: 35, longitude: 110.25, localTime: new DateTime(2019, 10, 1, 10, 1, 0), localTimeZone: TimeZoneInfo.Utc);
```

## 位置与时间

```C#
sun.Latitude = 35;
sun.Longitude = 110.25;
sun.DateTime = new DateTime(2019, 10, 1, 10, 1, 0);
sun.LocalTimeZone = TimeZoneInfo.Utc;
```

## 升、中天、降

```C#
TimeSpan rising = sun.Rising;
TimeSpan culmination = sun.Culmination;
TimeSpan setting = sun.Setting;
```

## 黄赤坐标

```C#
// 根据属性的 DateTime 计算坐标
Equator equator1 = sun.Equator;
Ecliptic ecliptic1 = sun.Ecliptic;
// 计算指定时间的坐标
Equator equator2 = sun.GetEquatorCoordinate(DateTime.Now);
Ecliptic ecliptic2 = sun.GetEclipticCoordinate(DateTime.Now);
```

## 距离

```C#
// 根据属性的 DateTime 计算日地距离
double toEarth1 = sun.ToEarth;
// 计算指定时间的日地距离
double toEarth2 = sun.GetToEarthDistance(DateTime.Now);
```

## 角度

```C#
double ea = sun.ElevationAngle;
double az = sun.Azimuth;
```

## 分至点与二十四节气

```C#
// 分至点
DateTime[] es = sun.GetEquinoxAndSolstice(2020, TimeZoneInfo.Local);
// 计算立春的时间
DateTime date = sun.GetSolarTerms(2020, SolarTerm.BeginningOfSpring, TimeZoneInfo.Local);
```
