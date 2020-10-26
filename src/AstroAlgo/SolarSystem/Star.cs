using AstroAlgo.Basic;
using AstroAlgo.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
        /// Time zone of observation site.
        /// </summary>
        public virtual TimeZoneInfo LocalTimeZone { get; set; }

        /// <summary>
        /// Current apparent equator coordinates.
        /// </summary>
        public virtual Equator Equator
        {
            get
            {
                return GetEquatorialCoordinate(DateTime.Now, true);
            }
        }

        /// <summary>
        /// Current apparent ecliptic coordinates.
        /// </summary>
        public virtual Ecliptic Ecliptic
        {
            get
            {
                return CoordinateSystem.Equator2Ecliptic(Equator, DateTime.Now, true);
            }
        }

        /// <summary>
        /// Rising time of the day.
        /// </summary>
        public virtual TimeSpan Rise
        {
            get
            {
                var e = GetEquatorialCoordinate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
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
                var e = GetEquatorialCoordinate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.833, Latitude, Longitude, DateTime.Now, LocalTimeZone);

                var span = TimeSpan.FromHours((time[0] + time[1]) / 30.0);
                if (time[0] > time[1])
                {
                    if (span.Hours > 12)
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
        public virtual TimeSpan Down
        {
            get
            {
                var e = GetEquatorialCoordinate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.833, Latitude, Longitude, DateTime.Now, LocalTimeZone);

                var span = TimeSpan.FromHours(time[1] / 15.0);

                return new TimeSpan(span.Hours, span.Minutes, span.Seconds);
            }
        }

        /// <summary>
        /// Current distance from Earth in AU.
        /// </summary>
        public virtual double ToEarth
        {
            get
            {
                return GetToEarthDistance(DateTime.Now);
            }
        }

        /// <summary>
        /// Current elevation angle.
        /// </summary>
        public virtual double ElevationAngle
        {
            get
            {
                return CoordinateSystem.GetElevationAngle(DateTime.Now, Equator, Latitude, Longitude);
            }
        }

        /// <summary>
        /// Current azimuth.
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

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Star"/>.
        /// </summary>
        /// <param name="latitude">Latitude of observation site.</param>
        /// <param name="longitude">Longitude of observation site.</param>
        /// <param name="localTimeZone">Time zone of observation site.</param>
        public Star(double latitude, double longitude, TimeZoneInfo localTimeZone)
        {
            Latitude = latitude;
            Longitude = longitude;
            LocalTimeZone = localTimeZone;
        }

        /// <summary>
        /// Calculates the equator coordinates of the star at a specified time.
        /// </summary>
        /// <param name="time">Local time.</param>
        /// <param name="isApparent">Is it the apparent equator coordinates.</param>
        /// <returns>Equator coordinates.</returns>
        public abstract Equator GetEquatorialCoordinate(DateTime time, bool isApparent = false);

        /// <summary>
        /// Calculates the star earth distance at a specified time.
        /// </summary>
        /// <param name="time">Local time.</param>
        /// <returns>Distance in AU.</returns>
        public abstract double GetToEarthDistance(DateTime time);
    }
}
