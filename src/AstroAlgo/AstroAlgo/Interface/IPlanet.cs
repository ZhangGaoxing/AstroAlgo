using AstroAlgo.Model;

using System;

namespace AstroAlgo.SolarSystem
{
    /// <summary>
    /// 行星接口
    /// </summary>
    public interface IPlanet : IStar
    {
        /// <summary>
        /// 当前与太阳距离
        /// </summary>
        double ToSun { get; }
    }
}
