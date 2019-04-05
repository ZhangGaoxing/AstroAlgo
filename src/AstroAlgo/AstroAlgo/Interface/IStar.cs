using AstroAlgo.Model;

using System;

namespace AstroAlgo.SolarSystem
{
    /// <summary>
    /// 恒星接口
    /// </summary>
    public interface IStar
    {
        /// <summary>
        /// 当前赤道坐标
        /// </summary>
        Equator Equator { get; }

        /// <summary>
        /// 当前地心黄道坐标
        /// </summary>
        Ecliptic Ecliptic { get; }

        /// <summary>
        /// 当日升时间
        /// </summary>
        TimeSpan Rise { get; }

        /// <summary>
        /// 当日中天时间
        /// </summary>
        TimeSpan Culmination { get; }

        /// <summary>
        /// 当日落时间
        /// </summary>
        TimeSpan Down { get; }

        /// <summary>
        /// 当前与地球距离
        /// </summary>
        double ToEarth { get; }

        /// <summary>
        /// 当前高度角
        /// </summary>
        double ElevationAngle { get; }

        /// <summary>
        /// 当前方位角
        /// </summary>
        double Azimuth { get; }
    }
}
