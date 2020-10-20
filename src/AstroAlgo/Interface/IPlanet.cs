using AstroAlgo.Models;

using System;

namespace AstroAlgo.SolarSystem
{
    /// <summary>
    /// Interface of planet.
    /// </summary>
    public interface IPlanet : IStar
    {
        /// <summary>
        /// Current distance from the sun.
        /// </summary>
        double ToSun { get; }
    }
}
