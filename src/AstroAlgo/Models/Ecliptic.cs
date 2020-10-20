namespace AstroAlgo.Models
{
    /// <summary>
    /// Ecliptic coordinates.
    /// </summary>
    public class Ecliptic
    {
        /// <summary>
        /// Longitude of the ecliptic.
        /// </summary>
        /// <remarks>
        /// Range from 0 to 360.
        /// </remarks>
        public double Longitude { get; set; }

        /// <summary>
        /// Latitude of the ecliptic
        /// </summary>
        /// <remarks>
        /// Range from -90 to 90.
        /// </remarks>
        public double Latitude { get; set; }
    }
}
