# 基础工具

`BasicTools` 类位于 `AstroAlgo.Basic` 命名空间下，提供了程序所需的基础计算函数。

## 角度的简化

```C#
double angle = 360.11;
double simplified = BasicTools.SimplifyAngle(angle);
```

## 角度的转换

```C#
double angle = 95.6;

// 角度转度分秒
(int degrees, int minutes, double seconds) dms = BasicTools.Angle2DMS(angle);
// 角度转时分秒
(int hours, int minutes, double seconds) hms = BasicTools.Angle2HMS(angle);
```