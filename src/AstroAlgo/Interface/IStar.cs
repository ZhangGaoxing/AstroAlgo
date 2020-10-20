using AstroAlgo.Models;
using System;

namespace AstroAlgo.SolarSystem
{
    /// <summary>
    /// Interface of star.
    /// </summary>
    public interface IStar
    {
        /// <summary>
        /// Current equator coordinates.
        /// </summary>
        Equator Equator { get; }

        /// <summary>
        /// Current geocentric ecliptic coordinates.
        /// </summary>
        Ecliptic Ecliptic { get; }

        /// <summary>
        /// Rising time of the day.
        /// </summary>
        TimeSpan Rise { get; }

        /// <summary>
        /// Culmination time of the day.
        /// </summary>
        TimeSpan Culmination { get; }

        /// <summary>
        /// Setting time of the day.
        /// </summary>
        TimeSpan Down { get; }

        /// <summary>
        /// Current distance from Earth.
        /// </summary>
        double ToEarth { get; }

        /// <summary>
        /// Current elevation angle.
        /// </summary>
        double ElevationAngle { get; }

        /// <summary>
        /// Current azimuth.
        /// </summary>
        double Azimuth { get; }
    }
}
