namespace AstroAlgo.Models
{
    /// <summary>
    /// Orbital Element
    /// </summary>
    public class OrbitalElement
    {
        /// <summary>
        /// Planetary mean longitude.
        /// </summary>
        public double MeanLongitude { get; set; }

        /// <summary>
        /// Length of orbital radius (AU).
        /// </summary>
        public double SemimajorAxis { get; set; }

        /// <summary>
        /// Orbital eccentricity.
        /// </summary>
        public double Eccentricity { get; set; }

        /// <summary>
        /// Angle between orbit and ecliptic.
        /// </summary>
        public double EclipticInclination { get; set; }

        /// <summary>
        /// Ecliptic longitude of ascending node.
        /// </summary>
        public double AscendingNodeLongitude { get; set; }

        /// <summary>
        /// Ecliptic longitude of perihelion.
        /// </summary>
        public double PerihelionLongitude { get; set; }
    }
}
