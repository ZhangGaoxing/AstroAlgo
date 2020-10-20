using System;

namespace AstroAlgo.Basic
{
    /// <summary>
    /// Sidereal time.
    /// </summary>
    public class SiderealTime
    {
        /// <summary>
        /// Calculate Greenwich mean sidereal time.
        /// </summary>
        /// <param name="date">Universal(Greenwich) time.</param>
        /// <returns>Greenwich mean sidereal time in degree.</returns>
        public static double UtMeanSiderealTime(DateTime date)
        {
            double julianDay = Julian.ToJulianDay(date);

            double T = (julianDay - 2451545.0) / 36525.0;
            double theta = 280.46061837 + 360.98564736629 * (julianDay - 2451545.0) + 0.000387933 * T * T - T * T * T / 38710000.0;

            theta = BasicTools.AngleSimplification(theta);

            return theta;
        }

        /// <summary>
        /// Calculate local mean sidereal time.
        /// </summary>
        /// <param name="localTime">Local time.</param>
        /// <param name="localZone">Local time zone.</param>
        /// <param name="longitude">Local longitude.</param>
        /// <returns>Local mean sidereal time in degree.</returns>
        public static double LocalMeanSiderealTime(DateTime localTime, TimeZoneInfo localZone, double longitude)
        {
            double t0 = UtMeanSiderealTime(new DateTime(localTime.Year, localTime.Month, localTime.Day));

            double t = t0 + ((localTime.Hour + ((localTime.Minute + (localTime.Second / 60.0)) / 60.0)) * 15 - (localZone.BaseUtcOffset.Hours + ((localZone.BaseUtcOffset.Minutes + (localZone.BaseUtcOffset.Seconds / 60.0)) / 60.0)) * 15) * (1 + 1 / 365.2422) + longitude;

            t = BasicTools.AngleSimplification(t);

            return t;
        }

        /// <summary>
        /// Convert local mean sidereal time to zone time.
        /// </summary>
        /// <param name="localSiderealTime">Local mean sidereal in degree.</param>
        /// <param name="date">Local date.</param>
        /// <param name="localZone">Local time zone.</param>
        /// <param name="longitude">Local longitude</param>
        /// <returns>Local time in degree.</returns>
        public static double SiderealTime2ZoneTime(double localSiderealTime, DateTime date, TimeZoneInfo localZone, double longitude)
        {
            double t0 = UtMeanSiderealTime(new DateTime(date.Year, date.Month, date.Day));

            double zoneTime = ((localSiderealTime - longitude - t0) / (1.0 + 1.0 / 365.2422)) + (localZone.BaseUtcOffset.Hours + ((localZone.BaseUtcOffset.Minutes + (localZone.BaseUtcOffset.Seconds / 60.0)) / 60.0)) * 15 + 0.06527778;

            zoneTime = BasicTools.AngleSimplification(zoneTime);

            return zoneTime - 1.04166667;
        }

        /// <summary>
        /// Calculate the difference between dynamical time and universal time.
        /// </summary>
        /// <param name="time"><see cref="DateTime"/></param>
        /// <returns>The difference.</returns>
        public static double DifferenceDtUt(DateTime time)
        {
            return -15 + Math.Pow(Julian.ToJulianDay(time) - 2382148, 2) / 41048480.0;
        }
    }
}