using AstroAlgo.Basic;
using AstroAlgo.Models;
using System;

namespace AstroAlgo.SolarSystem
{
    /// <summary>
    /// Base class for planets.
    /// </summary>
    public abstract class Planet
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
                return GetEquatorCoordinate(DateTime, true);
            }
        }

        /// <summary>
        /// Apparent geocentric ecliptic coordinates at <see cref="DateTime"/>.
        /// </summary>
        public virtual Ecliptic Ecliptic
        {
            get
            {
                return GetEclipticCoordinate(DateTime, true);
            }
        }

        /// <summary>
        /// Rising time of the day.
        /// </summary>
        public virtual TimeSpan Rising
        {
            get
            {
                var e = GetEquatorCoordinate(new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.5667, Latitude, Longitude, DateTime, LocalTimeZone);

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
                var e = GetEquatorCoordinate(new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.5667, Latitude, Longitude, DateTime, LocalTimeZone);

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
        public virtual TimeSpan Setting
        {
            get
            {
                var e = GetEquatorCoordinate(new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.5667, Latitude, Longitude, DateTime, LocalTimeZone);

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
                return GetToEarthDistance(DateTime);
            }
        }

        /// <summary>
        /// Distance from Sun in AU at <see cref="DateTime"/>.
        /// </summary>
        public virtual double ToSun
        {
            get
            {
                return GetToSunDistance(DateTime);
            }
        }

        /// <summary>
        /// Elevation angle at <see cref="DateTime"/>.
        /// </summary>
        public virtual double ElevationAngle
        {
            get
            {
                return CoordinateSystem.GetElevationAngle(DateTime, Equator, Latitude, Longitude);
            }
        }

        /// <summary>
        /// Azimuth at <see cref="DateTime"/>.
        /// </summary>
        public virtual double Azimuth
        {
            get
            {
                return CoordinateSystem.GetAzimuth(DateTime, Equator, Latitude, Longitude);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Planet"/>.
        /// </summary>
        public Planet()
        {
            Latitude = 0;
            Longitude = 0;
            DateTime = DateTime.Now;
            LocalTimeZone = TimeZoneInfo.Local;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Planet"/>.
        /// </summary>
        /// <param name="latitude">Latitude of observation site.</param>
        /// <param name="longitude">Longitude of observation site.</param>
        public Planet(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            DateTime = DateTime.Now;
            LocalTimeZone = TimeZoneInfo.Local;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Planet"/>.
        /// </summary>
        /// <param name="latitude">Latitude of observation site.</param>
        /// <param name="longitude">Longitude of observation site.</param>
        /// <param name="localTime">Time of observation site.</param>
        /// <param name="localTimeZone">Time zone of observation site.</param>
        public Planet(double latitude, double longitude, DateTime localTime, TimeZoneInfo localTimeZone)
        {
            Latitude = latitude;
            Longitude = longitude;
            DateTime = localTime;
            LocalTimeZone = localTimeZone;
        }

        #region Methods

        /// <summary>
        /// Calculates the equator coordinates of the star at a specified time.
        /// </summary>
        /// <param name="time">Local time.</param>
        /// <param name="isApparent">Is it the apparent equator coordinates.</param>
        /// <returns>Equator coordinates.</returns>
        public virtual Equator GetEquatorCoordinate(DateTime time, bool isApparent = false)
        {
            var e = GetEclipticCoordinate(time, isApparent);
            return CoordinateSystem.Ecliptic2Equator(e, time, isApparent);
        }

        /// <summary>
        /// Calculates the geocentric ecliptic coordinates of the planet at a specified time.
        /// </summary>
        /// <param name="time">Local time.</param>
        /// <param name="isApparent">Is it the apparent ecliptic coordinates.</param>
        /// <returns>Geocentric ecliptic coordinates.</returns>
        public virtual Ecliptic GetEclipticCoordinate(DateTime time, bool isApparent = false)
        {
            Earth e = new Earth();

            var earth = e.GetHeliocentricEclipticCoordinate(time);
            double r0 = e.GetToSunDistance(time);
            var e1 = GetHeliocentricEclipticCoordinate(time);
            double r1 = GetToSunDistance(time);

            double x1 = r1 * Math.Cos(e1.Latitude * (Math.PI / 180.0)) * Math.Cos(e1.Longitude * (Math.PI / 180.0)) - r0 * Math.Cos(earth.Latitude * (Math.PI / 180.0)) * Math.Cos(earth.Longitude * (Math.PI / 180.0));
            double y1 = r1 * Math.Cos(e1.Latitude * (Math.PI / 180.0)) * Math.Sin(e1.Longitude * (Math.PI / 180.0)) - r0 * Math.Cos(earth.Latitude * (Math.PI / 180.0)) * Math.Sin(earth.Longitude * (Math.PI / 180.0));
            double z1 = r1 * Math.Sin(e1.Latitude * (Math.PI / 180.0)) - r0 * Math.Sin(earth.Latitude * (Math.PI / 180.0));

            if (isApparent)
            {
                double t = 0.0057755183 * Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);

                var e2 = GetHeliocentricEclipticCoordinate(time, t);
                double r2 = GetToSunDistance(time, t);

                double x2 = r2 * Math.Cos(e2.Latitude * (Math.PI / 180.0)) * Math.Cos(e2.Longitude * (Math.PI / 180.0)) - r0 * Math.Cos(earth.Latitude * (Math.PI / 180.0)) * Math.Cos(earth.Longitude * (Math.PI / 180.0));
                double y2 = r2 * Math.Cos(e2.Latitude * (Math.PI / 180.0)) * Math.Sin(e2.Longitude * (Math.PI / 180.0)) - r0 * Math.Cos(earth.Latitude * (Math.PI / 180.0)) * Math.Sin(earth.Longitude * (Math.PI / 180.0));
                double z2 = r2 * Math.Sin(e2.Latitude * (Math.PI / 180.0)) - r0 * Math.Sin(earth.Latitude * (Math.PI / 180.0));
                var nutation = CoordinateSystem.GetNutation(time);
                // Uncorrected annual aberration
                // Corrected the light time and nutation of ecliptic longitude.
                double latitude = Math.Atan2(z2, Math.Sqrt(x2 * x2 + y2 * y2)) * (180 / Math.PI);
                double longitude = Math.Atan2(y2, x2) * (180 / Math.PI) + nutation.Longitude;

                if (longitude < 0)
                {
                    return new Ecliptic
                    {
                        Latitude = latitude,
                        Longitude = longitude + 360
                    };
                }
                else
                {
                    return new Ecliptic
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    };
                }
            }
            else
            {
                double latitude = Math.Atan2(z1, Math.Sqrt(x1 * x1 + y1 * y1)) * (180 / Math.PI);
                double longitude = Math.Atan2(y1, x1) * (180 / Math.PI);

                if (longitude < 0)
                {
                    return new Ecliptic
                    {
                        Latitude = latitude,
                        Longitude = longitude + 360
                    };
                }
                else
                {
                    return new Ecliptic
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    };
                }
            }
        }

        /// <summary>
        /// Calculates the heliocentric ecliptic coordinates of the planet at a specified time.
        /// </summary>
        /// <param name="time">Local time.</param>
        /// <param name="julianDay">Light time correction.</param>
        /// <returns>Heliocentric ecliptic coordinates.</returns>
        public abstract Ecliptic GetHeliocentricEclipticCoordinate(DateTime time, double julianDay = 0);

        /// <summary>
        /// Calculates the orbital element of the planet at a specified time.
        /// </summary>
        /// <param name="time">Local time.</param>
        /// <returns>Orbital element.</returns>
        public abstract OrbitalElement GetOrbitalElement(DateTime time);

        /// <summary>
        /// Calculates the <see cref="Sun"/> planet distance at a specified time.
        /// </summary>
        /// <param name="time">Local time.</param>
        /// <param name="julianDay">Light time correction.</param>
        /// <returns>Distance in AU.</returns>
        public abstract double GetToSunDistance(DateTime time, double julianDay = 0);

        /// <summary>
        /// Calculates the planet <see cref="Earth"/> distance at a specified time.
        /// </summary>
        /// <param name="time">Local time.</param>
        /// <returns>Distance in AU.</returns>
        public virtual double GetToEarthDistance(DateTime time)
        {
            Earth e = new Earth();

            var earth = e.GetHeliocentricEclipticCoordinate(time);
            double r0 = e.GetToSunDistance(time);
            var e1 = GetHeliocentricEclipticCoordinate(time);
            double r1 = GetToSunDistance(time);

            double x1 = r1 * Math.Cos(e1.Latitude * (Math.PI / 180.0)) * Math.Cos(e1.Longitude * (Math.PI / 180.0)) - r0 * Math.Cos(earth.Latitude * (Math.PI / 180.0)) * Math.Cos(earth.Longitude * (Math.PI / 180.0));
            double y1 = r1 * Math.Cos(e1.Latitude * (Math.PI / 180.0)) * Math.Sin(e1.Longitude * (Math.PI / 180.0)) - r0 * Math.Cos(earth.Latitude * (Math.PI / 180.0)) * Math.Sin(earth.Longitude * (Math.PI / 180.0));
            double z1 = r1 * Math.Sin(e1.Latitude * (Math.PI / 180.0)) - r0 * Math.Sin(earth.Latitude * (Math.PI / 180.0));

            //double t = 0.0057755183 * Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);

            //var e2 = HeliocentricEclipticCoordinate(time, t);
            //double r2 = ToSunDistance(time, t);

            //double x2 = r2 * Math.Cos(e2.Latitude * (Math.PI / 180.0)) * Math.Cos(e2.Longitude * (Math.PI / 180.0)) - r0 * Math.Cos(earth.Latitude * (Math.PI / 180.0)) * Math.Cos(earth.Longitude * (Math.PI / 180.0));
            //double y2 = r2 * Math.Cos(e2.Latitude * (Math.PI / 180.0)) * Math.Sin(e2.Longitude * (Math.PI / 180.0)) - r0 * Math.Cos(earth.Latitude * (Math.PI / 180.0)) * Math.Sin(earth.Longitude * (Math.PI / 180.0));
            //double z2 = r2 * Math.Sin(e2.Latitude * (Math.PI / 180.0)) - r0 * Math.Sin(earth.Latitude * (Math.PI / 180.0));

            //return Math.Sqrt(x2 * x2 + y2 * y2 + z2 * z2);

            return Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);
        }

        #endregion
    }
}
