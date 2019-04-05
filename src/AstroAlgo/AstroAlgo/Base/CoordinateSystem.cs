using AstroAlgo.Model;

using System;

namespace AstroAlgo.Base
{
    /// <summary>
    /// 坐标系统
    /// </summary>
    public class CoordinateSystem
    {
        /// <summary>
        /// 计算黄经章动，交角章动
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>章动</returns>
        public static Nutation GetNutation(DateTime time)
        {
            double T = (Julian.ToJulianDay(time) - 2451545) / 36525.0;
            // 平距角
            double D = 297.8502042 + 445267.1115168 * T - 0.0016300 * T * T + T * T * T / 545868.0 - T * T * T * T / 113065000.0;
            //double D = 297.85036 + 455267.111480 * T - 0.0019142 * T * T + T * T * T / 189474.0;
            // 太阳平近点角
            double M = 357.52772 + 35999.050340 * T - 0.0001603 * T * T - T * T * T / 300000.0;
            // 月球平近点角
            double M1 = 134.96298 + 477198.867398 * T + 0.0086972 * T * T + T * T * T / 56250.0;
            // 月球纬度参数
            double F = 93.27191 + 483202.017538 * T - 0.0036825 * T * T + T * T * T / 327270.0;
            // 黄道与月球平轨道升交点黄经
            double omega = 125.04452 - 1934.136261 * T + 0.0020708 * T * T + T * T * T / 450000.0;

            D = BasicTools.AngleSimplification(D);
            M = BasicTools.AngleSimplification(M);
            M1 = BasicTools.AngleSimplification(M1);
            F = BasicTools.AngleSimplification(F);
            omega = BasicTools.AngleSimplification(omega);

            D = D * (Math.PI / 180.0);
            M = M * (Math.PI / 180.0);
            M1 = M1 * (Math.PI / 180.0);
            F = F * (Math.PI / 180.0);
            omega = omega * (Math.PI / 180.0);

            #region 黄经章动

            double psi = (-171996 - 174.2 * T) * Math.Sin(omega)
                + (-13187 - 1.6 * T) * Math.Sin(-2 * D + 2 * F + 2 * omega)
                + (-2274 - 0.2 * T) * Math.Sin(2 * F + 2 * omega)
                + (2062 + 0.2 * T) * Math.Sin(2 * omega)
                + (1426 - 3.4 * T) * Math.Sin(M)
                + (712 + 0.1 * T) * Math.Sin(M1)
                + (-517 + 1.2 * T) * Math.Sin(-2 * D + M + 2 * F + 2 * omega)
                + (-386 - 0.4 * T) * Math.Sin(2 * F + omega)
                + (-301) * Math.Sin(M1 + 2 * F + 2 * omega)
                + (217 - 0.5 * T) * Math.Sin(-2 * D - M + 2 * F + 2 * omega)
                + (-158) * Math.Sin(-2 * D + M1)
                + (129 + 0.1 * T) * Math.Sin(-2 * D + 2 * F + omega)
                + 123 * Math.Sin(-M1 + 2 * F + 2 * omega)
                + 63 * Math.Sin(2 * D)
                + (63 + 0.1 * T) * Math.Sin(M1 + omega)
                + (-59) * Math.Sin(2 * D - M1 + 2 * F + 2 * omega)
                + (-58 - 0.1 * T) * Math.Sin(-M1 + omega)
                + (-51) * Math.Sin(M1 + 2 * F + omega)
                + 48 * Math.Sin(-2 * D + 2 * M1)
                + 46 * Math.Sin(-2 * M1 + 2 * F + omega)
                + (-38) * Math.Sin(2 * D + 2 * F + 2 * omega)
                + (-31) * Math.Sin(2 * M1 + 2 * F + 2 * omega)
                + 29 * Math.Sin(2 * M1)
                + 29 * Math.Sin(-2 * D + M1 + 2 * F + 2 * omega)
                + 26 * Math.Sin(2 * F)
                + (-22) * Math.Sin(-2 * D + 2 * F)
                + 21 * Math.Sin(-M1 + 2 * F + omega)
                + (17 - 0.1 * T) * Math.Sin(2 * M)
                + 16 * Math.Sin(2 * D - M1 + omega)
                + (-16 + 0.1 * T) * Math.Sin(-2 * D + 2 * M + 2 * F + 2 * omega)
                + (-15) * Math.Sin(M + omega)
                + (-13) * Math.Sin(-2 * D + M1 + omega)
                + (-12) * Math.Sin(-M + omega)
                + 11 * Math.Sin(2 * M1 - 2 * F)
                + (-10) * Math.Sin(2 * D - M1 + 2 * F + omega)
                + (-8) * Math.Sin(2 * D + M1 + 2 * F + 2 * omega)
                + 7 * Math.Sin(M + 2 * F + 2 * omega)
                + (-7) * Math.Sin(-2 * D + M + M1)
                + (-7) * Math.Sin(-M + 2 * F + 2 * omega)
                + (-7) * Math.Sin(2 * D + 2 * F + omega)
                + 6 * Math.Sin(2 * D + M1)
                + 6 * Math.Sin(-2 * D + 2 * M1 + 2 * F + 2 * omega)
                + 6 * Math.Sin(-2 * D + M1 + 2 * F + omega)
                + (-6) * Math.Sin(2 * D - 2 * M1 + omega)
                + (-6) * Math.Sin(2 * D + omega)
                + 5 * Math.Sin(-M + M1)
                + (-5) * Math.Sin(-2 * D - M + 2 * F + omega)
                + (-5) * Math.Sin(-2 * D + omega)
                + (-5) * Math.Sin(2 * M1 + 2 * F + omega)
                + 4 * Math.Sin(-2 * D + 2 * M1 + omega)
                + 4 * Math.Sin(-2 * D + M + 2 * F + omega)
                + 4 * Math.Sin(M1 - 2 * F)
                + (-4) * Math.Sin(-D + M1)
                + (-4) * Math.Sin(-2 * D + M)
                + (-4) * Math.Sin(D)
                + 3 * Math.Sin(M1 + 2 * F)
                + (-3) * Math.Sin(-2 * M1 + 2 * F + 2 * omega)
                + (-3) * Math.Sin(-D - M + M1)
                + (-3) * Math.Sin(M + M1)
                + (-3) * Math.Sin(-M + M1 + 2 * F + 2 * omega)
                + (-3) * Math.Sin(2 * D - M - M1 + 2 * F + 2 * omega)
                + (-3) * Math.Sin(3 * M1 + 2 * F + 2 * omega)
                + (-3) * Math.Sin(2 * D - M + 2 * F + 2 * omega);

            #endregion

            #region 交角章动

            double epsilon = (92025 + 8.9 * T) * Math.Cos(omega)
                + (5736 - 3.1 * T) * Math.Cos(-2 * D + 2 * F + 2 * omega)
                + (977 - 0.5 * T) * Math.Cos(2 * F + 2 * omega)
                + (-895 + 0.5 * T) * Math.Cos(2 * omega)
                + (54 - 0.1 * T) * Math.Cos(M)
                + (-7) * Math.Cos(M1)
                + (224 - 0.6 * T) * Math.Cos(-2 * D + M + 2 * F + 2 * omega)
                + 200 * Math.Cos(2 * F + omega)
                + (129 - 0.1 * T) * Math.Cos(M1 + 2 * F + 2 * omega)
                + (-95 + 0.3 * T) * Math.Cos(-2 * D - M + 2 * F + 2 * omega)
                + Math.Cos(-2 * D + M1)
                + (-70) * Math.Cos(-2 * D + 2 * F + omega)
                + (-53) * Math.Cos(-M1 + 2 * F + 2 * omega)
                + Math.Cos(2 * D)
                + (-33) * Math.Cos(M1 + omega)
                + 26 * Math.Cos(2 * D - M1 + 2 * F + 2 * omega)
                + 32 * Math.Cos(-M1 + omega)
                + 27 * Math.Cos(M1 + 2 * F + omega)
                + Math.Cos(-2 * D + 2 * M1)
                + (-24) * Math.Cos(-2 * M1 + 2 * F + omega)
                + 16 * Math.Cos(2 * D + 2 * F + 2 * omega)
                + 13 * Math.Cos(2 * M1 + 2 * F + 2 * omega)
                + Math.Cos(2 * M1)
                + (-12) * Math.Cos(-2 * D + M1 + 2 * F + 2 * omega)
                + Math.Cos(2 * F)
                + Math.Cos(-2 * D + 2 * F)
                + (-10) * Math.Cos(-M1 + 2 * F + omega)
                + Math.Cos(2 * M)
                + (-8) * Math.Cos(2 * D - M1 + omega)
                + 7 * Math.Cos(-2 * D + 2 * M + 2 * F + 2 * omega)
                + 9 * Math.Cos(M + omega)
                + 7 * Math.Cos(-2 * D + M1 + omega)
                + 6 * Math.Cos(-M + omega)
                + Math.Cos(2 * M1 - 2 * F)
                + 5 * Math.Cos(2 * D - M1 + 2 * F + omega)
                + 3 * Math.Cos(2 * D + M1 + 2 * F + 2 * omega)
                + (-3) * Math.Cos(M + 2 * F + 2 * omega)
                + Math.Cos(-2 * D + M + M1)
                + 3 * Math.Cos(-M + 2 * F + 2 * omega)
                + 3 * Math.Cos(2 * D + 2 * F + omega)
                + Math.Cos(2 * D + M1)
                + (-3) * Math.Cos(-2 * D + 2 * M1 + 2 * F + 2 * omega)
                + (-3) * Math.Cos(-2 * D + M1 + 2 * F + omega)
                + 3 * Math.Cos(2 * D - 2 * M1 + omega)
                + 3 * Math.Cos(2 * D + omega)
                + Math.Cos(-M + M1)
                + 3 * Math.Cos(-2 * D - M + 2 * F + omega)
                + 3 * Math.Cos(-2 * D + omega)
                + 3 * Math.Cos(2 * M1 + 2 * F + omega)
                + Math.Cos(-2 * D + 2 * M1 + omega)
                + Math.Cos(-2 * D + M + 2 * F + omega)
                + Math.Cos(M1 - 2 * F)
                + Math.Cos(-D + M1)
                + Math.Cos(-2 * D + M)
                + Math.Cos(D)
                + Math.Cos(M1 + 2 * F)
                + Math.Cos(-2 * M1 + 2 * F + 2 * omega)
                + Math.Cos(-D - M + M1)
                + Math.Cos(M + M1)
                + Math.Cos(-M + M1 + 2 * F + 2 * omega)
                + Math.Cos(2 * D - M - M1 + 2 * F + 2 * omega)
                + Math.Cos(3 * M1 + 2 * F + 2 * omega)
                + Math.Cos(2 * D - M + 2 * F + 2 * omega);

            #endregion

            return new Nutation()
            {
                Longitude = psi * (0.0001 / 3600.0),
                Obliquity = epsilon * (0.0001 / 3600.0)
            };
        }

        /// <summary>
        /// 黄赤交角（千年误差0''.01）
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="isTrue">真黄赤交角</param>
        /// <returns>角度大小</returns>
        public static double EclipticObliquity(DateTime time, bool isTrue = true)
        {
            double JDE = Julian.ToJulianDay(time);
            double T = (JDE - 2451545) / 36525.0;
            double U = T / 100.0;

            double epsilon = ((((21.448 / 60.0) + 26) / 60.0) + 23) -
                (4680.93 / 60.0 / 60.0) * U -
                1.55 * U * U +
                1999.24 * U * U * U -
                51.38 * U * U * U * U -
                249.67 * U * U * U * U * U -
                39.05 * U * U * U * U * U * U +
                7.12 * U * U * U * U * U * U * U +
                27.87 * U * U * U * U * U * U * U * U +
                5.79 * U * U * U * U * U * U * U * U * U +
                2.45 * U * U * U * U * U * U * U * U * U * U;

            if (!isTrue)
            {
                return epsilon;
            }
            else
            {
                var n = GetNutation(time);
                double obliquity = n.Obliquity;

                return epsilon + obliquity;
            }
        }

        #region 坐标转换

        /// <summary>
        /// 黄道坐标转赤道坐标（J2000.0）
        /// </summary>
        /// <param name="e">黄道坐标</param>
        /// <returns>赤道坐标</returns>
        public static Equator Ecliptic2Equatorial(Ecliptic e)
        {
            double lambda = e.Longitude;
            double beta = e.Latitude;
            double epsilon = 23.439291;

            double tanA1 = Math.Sin(lambda * (Math.PI / 180.0)) * Math.Cos(epsilon * (Math.PI / 180.0)) - Math.Tan(beta * (Math.PI / 180.0)) * Math.Sin(epsilon * (Math.PI / 180.0));
            double tanA2 = Math.Cos(lambda * (Math.PI / 180.0));
            double sinD = Math.Sin(beta * (Math.PI / 180.0)) * Math.Cos(epsilon * (Math.PI / 180.0)) + Math.Cos(beta * (Math.PI / 180.0)) * Math.Sin(epsilon * (Math.PI / 180.0)) * Math.Sin(lambda * (Math.PI / 180.0));

            double ra = Math.Atan2(tanA1, tanA2) * (180.0 / Math.PI);
            double dec = Math.Asin(sinD) * (180.0 / Math.PI);

            if (ra < 0)
            {
                ra = 360 + ra;
            }

            return new Equator()
            {
                RA = ra,
                Dec = dec
            };
        }

        /// <summary>
        /// 黄道坐标转赤道坐标
        /// </summary>
        /// <param name="e">黄道坐标</param>
        /// <param name="time">时间（用于计算黄赤交角）</param>
        /// <param name="isApparent">是否为视黄道坐标</param>
        /// <returns>赤道坐标</returns>
        public static Equator Ecliptic2Equatorial(Ecliptic e, DateTime time, bool isApparent)
        {
            double lambda = e.Longitude;
            double beta = e.Latitude;
            double epsilon;

            if (isApparent)
            {
                epsilon = EclipticObliquity(time);
            }
            else
            {
                epsilon = EclipticObliquity(time, false);
            }

            double tanA1 = Math.Sin(lambda * (Math.PI / 180.0)) * Math.Cos(epsilon * (Math.PI / 180.0)) - Math.Tan(beta * (Math.PI / 180.0)) * Math.Sin(epsilon * (Math.PI / 180.0));
            double tanA2 = Math.Cos(lambda * (Math.PI / 180.0));
            double sinD = Math.Sin(beta * (Math.PI / 180.0)) * Math.Cos(epsilon * (Math.PI / 180.0)) + Math.Cos(beta * (Math.PI / 180.0)) * Math.Sin(epsilon * (Math.PI / 180.0)) * Math.Sin(lambda * (Math.PI / 180.0));

            double ra = Math.Atan2(tanA1, tanA2) * (180.0 / Math.PI);
            double dec = Math.Asin(sinD) * (180.0 / Math.PI);

            if (ra < 0)
            {
                ra = 360 + ra;
            }

            return new Equator()
            {
                RA = ra,
                Dec = dec
            };
        }

        /// <summary>
        /// 赤道坐标转黄道坐标（J2000.0）
        /// </summary>
        /// <param name="e">赤道坐标</param>
        /// <returns>黄道坐标</returns>
        public static Ecliptic Equatorial2Ecliptic(Equator e)
        {
            double alpha = e.RA;
            double delta = e.Dec;
            double epsilon = 23.439291;

            double tanD = Math.Sin(alpha * (Math.PI / 180.0)) * Math.Cos(epsilon * (Math.PI / 180.0)) + Math.Tan(delta * (Math.PI / 180.0)) * Math.Sin(epsilon * (Math.PI / 180.0));
            double tanL2 = Math.Cos(alpha * (Math.PI / 180.0));
            double sinB = Math.Sin(delta * (Math.PI / 180.0)) * Math.Cos(epsilon * (Math.PI / 180.0)) - Math.Cos(delta * (Math.PI / 180.0)) * Math.Sin(epsilon * (Math.PI / 180.0)) * Math.Sin(alpha * (Math.PI / 180.0));

            double longitude = Math.Atan2(tanD, tanL2) * (180.0 / Math.PI);
            double latitude = Math.Asin(sinB) * (180.0 / Math.PI);

            if (longitude < 0)
            {
                longitude = 360 + longitude;
            }

            return new Ecliptic()
            {
                Longitude = longitude,
                Latitude = latitude
            };
        }

        /// <summary>
        /// 赤道坐标转黄道坐标
        /// </summary>
        /// <param name="e">赤道坐标</param>
        /// <param name="time">时间（用于计算黄赤交角）</param>
        /// <param name="isApparent">是否为视赤道坐标</param>
        /// <returns>黄道坐标</returns>
        public static Ecliptic Equatorial2Ecliptic(Equator e, DateTime time, bool isApparent)
        {
            double alpha = e.RA;
            double delta = e.Dec;
            double epsilon;

            if (isApparent)
            {
                epsilon = EclipticObliquity(time);
            }
            else
            {
                epsilon = EclipticObliquity(time, false);
            }

            double tanD = Math.Sin(alpha * (Math.PI / 180.0)) * Math.Cos(epsilon * (Math.PI / 180.0)) + Math.Tan(delta * (Math.PI / 180.0)) * Math.Sin(epsilon * (Math.PI / 180.0));
            double tanL2 = Math.Cos(alpha * (Math.PI / 180.0));
            double sinB = Math.Sin(delta * (Math.PI / 180.0)) * Math.Cos(epsilon * (Math.PI / 180.0)) - Math.Cos(delta * (Math.PI / 180.0)) * Math.Sin(epsilon * (Math.PI / 180.0)) * Math.Sin(alpha * (Math.PI / 180.0));

            double longitude = Math.Atan2(tanD, tanL2) * (180.0 / Math.PI);
            double latitude = Math.Asin(sinB) * (180.0 / Math.PI);

            if (longitude < 0)
            {
                longitude = 360 + longitude;
            }

            return new Ecliptic()
            {
                Longitude = longitude,
                Latitude = latitude
            };
        }

        #endregion

        /// <summary>
        /// 本地时角
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="RA">赤经</param>
        /// <param name="longitude">观测站经度</param>
        /// <returns>时角</returns>
        public static double HourAngle(DateTime time, double RA, double longitude)
        {
            double ra = RA;
            double localSiderealTime = SiderealTime.LocalSiderealTime(time, TimeZoneInfo.Local, longitude);

            double ha = localSiderealTime - ra;

            return ha;
        }

        /// <summary>
        /// 视差角
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="e">赤道坐标</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <returns>视差角</returns>
        public static double ParallacticAngle(DateTime time, Equator e, double latitude, double longitude)
        {
            double H = HourAngle(time, e.RA, longitude);

            double tanP1 = Math.Sin(H * (Math.PI / 180.0));
            double tanP2 = Math.Tan(latitude * (Math.PI / 180.0)) * Math.Cos(e.Dec * (Math.PI / 180.0)) - Math.Sin(e.Dec * (Math.PI / 180.0)) * Math.Cos(H * (Math.PI / 180.0));

            return Math.Atan2(tanP1, tanP2);
        }

        /// <summary>
        /// 计算角距离
        /// </summary>
        /// <param name="e1">天体1赤道坐标</param>
        /// <param name="e2">天体2赤道坐标</param>
        /// <returns>角距离，单位为度</returns>
        public static double AngularSeparation(Equator e1, Equator e2)
        {
            double sin2 = Math.Sin(Math.Abs(e1.Dec - e2.Dec) / 2 * (Math.PI / 180.0)) * Math.Sin(Math.Abs(e1.Dec - e2.Dec) / 2 * (Math.PI / 180.0))
                + Math.Cos(e1.Dec * (Math.PI / 180.0)) * Math.Cos(e2.Dec * (Math.PI / 180.0)) * Math.Sin(Math.Abs(e1.RA - e2.RA) / 2 * (Math.PI / 180.0)) * Math.Sin(Math.Abs(e1.RA - e2.RA) / 2 * (Math.PI / 180.0));

            double sin = Math.Pow(sin2, 0.5);

            double theta = Math.Asin(sin) * 180.0 / Math.PI;

            return theta * 2;
        }

        /// <summary>
        /// 高度角转换为区时
        /// </summary>
        /// <param name="e">赤道坐标</param>
        /// <param name="angle">高度角</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <param name="date">日期</param>
        /// <param name="localZone">时区</param>
        /// <returns>单位为“度”的时间</returns>
        public static double[] ElevationAngle2Time(Equator e, double angle, double latitude, double longitude, DateTime date, TimeZoneInfo localZone)
        {
            double ra = e.RA;
            double dec = e.Dec;

            double sinH = Math.Sin(angle * (Math.PI / 180.0));
            double cosT = (sinH - Math.Sin(latitude * Math.PI / 180.0) * Math.Sin(dec * Math.PI / 180.0)) / (Math.Cos(latitude * Math.PI / 180.0) * Math.Cos(dec * Math.PI / 180.0));
            if (Math.Abs(cosT) > 1)
            {
                return new double[] { 0, 0 };
            }
            double T = Math.Acos(cosT) * 180.0 / Math.PI;

            double t1 = 360 - T;
            double t2 = T;

            double localSiderealTime1 = t1 + ra;
            double zoneTime1 = SiderealTime.LocalSiderealTime2ZoneTime(localSiderealTime1, date, localZone, longitude);

            double localSiderealTime2 = t2 + ra;
            double zoneTime2 = SiderealTime.LocalSiderealTime2ZoneTime(localSiderealTime2, date, localZone, longitude);

            return new double[] { zoneTime1, zoneTime2 };
        }

        /// <summary>
        /// 中天高度角
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="e">赤道坐标</param>
        /// <param name="latitude">纬度</param>
        /// <returns>中天高度角</returns>
        public static double CulminationAngle(DateTime date, Equator e, double latitude)
        {
            double dec = e.Dec;

            double sinH = Math.Sin(latitude * Math.PI / 180.0) * Math.Sin(dec * Math.PI / 180.0) + Math.Cos(latitude * Math.PI / 180.0) * Math.Cos(dec * Math.PI / 180.0);
            double H = Math.Asin(sinH) * 180.0 / Math.PI;

            return H;
        }

        /// <summary>
        /// 计算指定时间的高度角
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="e">赤道坐标</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <returns>高度角</returns>
        public static double ElevationAngle(DateTime time, Equator e, double latitude, double longitude)
        {
            double ra = e.RA;
            double dec = e.Dec;
            double hourAngle = HourAngle(time, ra, longitude);

            double sinH = Math.Sin(latitude * Math.PI / 180.0) * Math.Sin(dec * Math.PI / 180.0) + Math.Cos(latitude * Math.PI / 180.0) * Math.Cos(dec * Math.PI / 180.0) * Math.Cos(hourAngle * Math.PI / 180.0);
            double H = Math.Asin(sinH) * 180.0 / Math.PI;

            return H;
        }

        /// <summary>
        /// 计算方位角
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="e">赤道坐标</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <returns></returns>
        public static double Azimuth(DateTime time, Equator e, double latitude, double longitude)
        {
            double elevationAngle = ElevationAngle(time, e, latitude, longitude);

            double ra = e.RA;
            double dec = e.Dec;

            double omega = HourAngle(time, ra, longitude);

            double cosA = (Math.Sin(dec * (Math.PI / 180.0)) - Math.Sin(elevationAngle * (Math.PI / 180.0)) * Math.Sin(latitude * (Math.PI / 180.0))) / (Math.Cos(elevationAngle * (Math.PI / 180.0)) * Math.Cos(latitude * (Math.PI / 180.0)));

            double A;
            if (omega < 0)
            {
                A = Math.Acos(cosA) * 180.0 / Math.PI;
            }
            else
            {
                A = 360 - (Math.Acos(cosA) * 180.0 / Math.PI);
            }

            return A;
        }

        /*
        public static Nutation LowAccuracy(DateTime time)
        {
            double T = (Julian.ToJulianDay(time) - 2451545) / 36525.0;
            double omega = 125.04452 - 1934.136261 * T + 0.0020708 * T * T + T * T * T / 450000.0;

            double L = 280.4665 + 36000.7698 * T;
            double L1 = 218.3165 + 481267.8813 * T;

            if (omega <= 0)
            {
                while (!(omega >= 0 && omega <= 360))
                {
                    omega += 360;
                }
            }
            else
            {
                while (!(omega >= 0 && omega <= 360))
                {
                    omega -= 360;
                }
            }

            if (L <= 0)
            {
                while (!(L >= 0 && L <= 360))
                {
                    L += 360;
                }
            }
            else
            {
                while (!(L >= 0 && L <= 360))
                {
                    L -= 360;
                }
            }

            if (L1 <= 0)
            {
                while (!(L1 >= 0 && L1 <= 360))
                {
                    L1 += 360;
                }
            }
            else
            {
                while (!(L1 >= 0 && L1 <= 360))
                {
                    L1 -= 360;
                }
            }

            L = L * (Math.PI / 180.0);
            L1 = L1 * (Math.PI / 180.0);
            omega = omega * (Math.PI / 180.0);

            double psi = (-17.2 / 60.0 / 60.0) * Math.Sin(omega) - (1.32 / 60.0 / 60.0) * Math.Sin(2 * L) - (0.23 / 60.0 / 60.0) * Math.Sin(2 * L1) + (0.21 / 60.0 / 60.0) * Math.Sin(2 * omega);
            double epsilon = (9.2 / 60.0 / 60.0) * Math.Cos(omega) + (0.57 / 60.0 / 60.0) * Math.Cos(2 * L) + (0.1 / 60.0 / 60.0) * Math.Cos(2 * L1) - (0.09 / 60.0 / 60.0) * Math.Cos(2 * omega);

            return new Nutation()
            {
                Longitude = psi,
                Obliquity = epsilon
            };
        }
        */
    }
}
