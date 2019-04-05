using System;

namespace AstroAlgo.Base
{
    /// <summary>
    /// 恒星时
    /// </summary>
    public class SiderealTime
    {
        /// <summary>
        /// 计算格林尼治恒星时
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns>单位为“度”的格林尼治恒星时</returns>
        public static double UTSiderealTime(DateTime date)
        {
            double julianDay = Julian.ToJulianDay(date);

            double T = (julianDay - 2451545.0) / 36525.0;
            double theta = 280.46061837 + 360.98564736629 * (julianDay - 2451545.0) + 0.000387933 * T * T - T * T * T / 38710000.0;

            theta = BasicTools.AngleSimplification(theta);

            //TimeSpan time = TimeSpan.FromSeconds(t / 15 * 3600);

            return theta;
        }

        /// <summary>
        /// 计算地方恒星时
        /// </summary>
        /// <param name="zoneTime">区时</param>
        /// <param name="localZone">时区</param>
        /// <param name="longitude">经度</param>
        /// <returns>单位为“度”的地方恒星时</returns>
        public static double LocalSiderealTime(DateTime zoneTime, TimeZoneInfo localZone, double longitude)
        {
            double t0 = UTSiderealTime(new DateTime(zoneTime.Year, zoneTime.Month, zoneTime.Day));

            double t = t0 + ((zoneTime.Hour + ((zoneTime.Minute + (zoneTime.Second / 60.0)) / 60.0)) * 15 - (localZone.BaseUtcOffset.Hours + ((localZone.BaseUtcOffset.Minutes + (localZone.BaseUtcOffset.Seconds / 60.0)) / 60.0)) * 15) *
                       (1 + 1 / 365.2422) + longitude;

            t = BasicTools.AngleSimplification(t);

            return t;
        }

        /// <summary>
        /// 地方恒星时转为区时
        /// </summary>
        /// <param name="localSiderealTime">地方恒星时(0°-360°)</param>
        /// <param name="date">日期</param>
        /// <param name="localZone">时区</param>
        /// <param name="longitude">经度</param>
        /// <returns>单位为“度”的区时</returns>
        public static double LocalSiderealTime2ZoneTime(double localSiderealTime, DateTime date, TimeZoneInfo localZone, double longitude)
        {
            double t0 = UTSiderealTime(new DateTime(date.Year, date.Month, date.Day));

            double zoneTime = ((localSiderealTime - longitude - t0) / (1.0 + 1.0 / 365.2422)) + (localZone.BaseUtcOffset.Hours + ((localZone.BaseUtcOffset.Minutes + (localZone.BaseUtcOffset.Seconds / 60.0)) / 60.0)) * 15 + 0.06527778;

            zoneTime = BasicTools.AngleSimplification(zoneTime);

            return zoneTime - 1.04166667;
        }

        /// <summary>
        /// 力学时与世界时之差
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>差值</returns>
        public static double DifferenceTdUt(DateTime time)
        {
            return -15 + Math.Pow((Julian.ToJulianDay(time) - 2382148), 2) / 41048480.0;
        }
    }
}