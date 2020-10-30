using AstroAlgo.Basic;
using AstroAlgo.Models;
using System;

namespace AstroAlgo.SolarSystem
{
    /// <summary>
    /// Base class for stars.
    /// </summary>
    public abstract class Star
    {
        #region Properties

        /// <summary>
        /// Latitude of observation site.
        /// </summary>
        public virtual double Latitude { get; set; }

        /// <summary>
        /// Longitude of observation site.
        /// </summary>
        public virtual double Longitude { get; set; }

        /// <summary>
        /// Time of observation site.
        /// </summary>
        public virtual DateTime DateTime { get; set; }

        /// <summary>
        /// Time zone of observation site.
        /// </summary>
        public virtual TimeZoneInfo LocalTimeZone { get; set; }

        /// <summary>
        /// Apparent equator coordinates at <see cref="DateTime"/>.
        /// </summary>
        public virtual Equator Equator
        {
            get
            {
                return GetEquatorCoordinate(DateTime.Now, true);
            }
        }

        /// <summary>
        /// Apparent geocentric ecliptic coordinates at <see cref="DateTime"/>.
        /// </summary>
        public virtual Ecliptic Ecliptic
        {
            get
            {
                return GetEclipticCoordinate(DateTime.Now, true);
            }
        }

        /// <summary>
        /// Rising time of the day.
        /// </summary>
        public virtual TimeSpan Rising
        {
            get
            {
                var e = GetEquatorCoordinate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.833, Latitude, Longitude, DateTime.Now, LocalTimeZone);

                var span = TimeSpan.FromHours(time[0] / 15.0);

                return new TimeSpan(span.Hours, span.Minutes, span.Seconds);
            }
        }

        /// <summary>
        /// Culmination time of the day.
        /// </summary>
        public virtual TimeSpan Culmination
        {
            get
            {
                var e = GetEquatorCoordinate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.833, Latitude, Longitude, DateTime.Now, LocalTimeZone);

                var span = TimeSpan.FromHours((time[0] + time[1]) / 30.0);
                if (time[0] > time[1])
                {
                    if (span.Hours >= 12)
                    {
                        return new TimeSpan(span.Hours - 12, span.Minutes, span.Seconds);
                    }
                    else
                    {
                        return new TimeSpan(span.Hours + 12, span.Minutes, span.Seconds);
                    }
                }
                else
                {
                    return new TimeSpan(span.Hours, span.Minutes, span.Seconds);
                }
            }
        }

        /// <summary>
        /// Setting time of the day.
        /// </summary>
        public virtual TimeSpan Setting
        {
            get
            {
                var e = GetEquatorCoordinate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.833, Latitude, Longitude, DateTime.Now, LocalTimeZone);

                var span = TimeSpan.FromHours(time[1] / 15.0);

                return new TimeSpan(span.Hours, span.Minutes, span.Seconds);
            }
        }

        /// <summary>
        /// Distance from Earth in AU at <see cref="DateTime"/>.
        /// </summary>
        public virtual double ToEarth
        {
            get
            {
                return GetToEarthDistance(DateTime.Now);
            }
        }

        /// <summary>
        /// Elevation angle at <see cref="DateTime"/>.
        /// </summary>
        public virtual double ElevationAngle
        {
            get
            {
                return CoordinateSystem.GetElevationAngle(DateTime.Now, Equator, Latitude, Longitude);
            }
        }

        /// <summary>
        /// Azimuth at <see cref="DateTime"/>.
        /// </summary>
        public virtual double Azimuth
        {
            get
            {
                return CoordinateSystem.GetAzimuth(DateTime.Now, Equator, Latitude, Longitude);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Star"/>.
        /// </summary>
        public Star()
        {
            Latitude = 0;
            Longitude = 0;
            DateTime = DateTime.Now;
            LocalTimeZone = TimeZoneInfo.Local;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Star"/>.
        /// </summary>
        /// <param name="latitude">Latitude of observation site.</param>
        /// <param name="longitude">Longitude of observation site.</param>
        public Star(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            DateTime = DateTime.Now;
            LocalTimeZone = TimeZoneInfo.Local;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Star"/>.
        /// </summary>
        /// <param name="latitude">Latitude of observation site.</param>
        /// <param name="longitude">Longitude of observation site.</param>
        /// <param name="dateTime">Time of observation site.</param>
        /// <param name="localTimeZone">Time zone of observation site.</param>
        public Star(double latitude, double longitude, DateTime dateTime, TimeZoneInfo localTimeZone)
        {
            Latitude = latitude;
            Longitude = longitude;
            DateTime = dateTime;
            LocalTimeZone = localTimeZone;
        }

        /// <summary>
        /// Calculates the equator coordinates of the star at a specified time.
        /// </summary>
        /// <param name="time">Local time.</param>
        /// <param name="isApparent">Is it the apparent equator coordinates.</param>
        /// <returns>Equator coordinates.</returns>
        public abstract Equator GetEquatorCoordinate(DateTime time, bool isApparent = false);

        /// <summary>
        /// Calculates the geocentric ecliptic coordinates of the planet at a specified time.
        /// </summary>
        /// <param name="time">Local time.</param>
        /// <param name="isApparent">Is it the apparent ecliptic coordinates.</param>
        /// <returns>Geocentric ecliptic coordinates.</returns>
        public abstract Ecliptic GetEclipticCoordinate(DateTime time, bool isApparent = false);

        /// <summary>
        /// Calculates the star <see cref="Earth"/> distance at a specified time.
        /// </summary>
        /// <param name="time">Local time.</param>
        /// <returns>Distance in AU.</returns>
        public abstract double GetToEarthDistance(DateTime time);
    }
}
