# 火星

`Mars` 类位于 `AstroAlgo.SolarSystem` 命名空间下，从抽象类 `Planet` 中派生，属性均继承 `Planet` 中的虚拟实现。

## 构造函数

```C#
// 无参数的构造函数，观测点经纬度默认为0，时间默认为系统当前时间，时区默认为系统当前时区
Mars mars1 = new Mars();
// 指定了观测点的经纬度，时间默认为系统当前时间，时区默认为系统当前时区
Mars mars2 = new Mars(latitude: 35, longitude: 110.25);
// 完整参数的构造函数
Mars mars3 = new Mars(latitude: 35, longitude: 110.25, localTime: new DateTime(2019, 10, 1, 10, 1, 0), localTimeZone: TimeZoneInfo.Utc);
```

## 位置与时间

```C#
mars.Latitude = 35;
mars.Longitude = 110.25;
mars.DateTime = new DateTime(2019, 10, 1, 10, 1, 0);
mars.LocalTimeZone = TimeZoneInfo.Utc;
```

## 升、中天、降

```C#
TimeSpan rising = mars.Rising;
TimeSpan culmination = mars.Culmination;
TimeSpan setting = mars.Setting;
```

## 黄赤坐标

```C#
// 根据属性的 DateTime 计算坐标
Equator equator1 = mars.Equator;
Ecliptic ecliptic1 = mars.Ecliptic;
// 计算指定时间的坐标
Equator equator2 = mars.GetEquatorCoordinate(DateTime.Now);
Ecliptic ecliptic2 = mars.GetEclipticCoordinate(DateTime.Now);
// 计算日心黄道坐标
Ecliptic ecliptic3 = mars.GetHeliocentricEclipticCoordinate(DateTime.Now);
```

## 距离

```C#
// 计算地火距离
double toEarth1 = mars.ToEarth;
double toEarth2 = mars.GetToEarthDistance(DateTime.Now);
// 计算日火距离
double toSun1 = mars.ToSun;
double toSun2 = mars.GetToSunDistance(DateTime.Now);
```

## 角度

```C#
double ea = mars.ElevationAngle;
double az = mars.Azimuth;
```

## 轨道要素
```C#
OrbitalElement element = mars.GetOrbitalElement(DateTime.Now);
```