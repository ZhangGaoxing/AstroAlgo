namespace AstroAlgo.Model
{
    /// <summary>
    /// 轨道要素
    /// </summary>
    public class OrbitalElement
    {
        /// <summary>
        /// 行星平黄经
        /// </summary>
        public double MeanLongitude { get; set; }

        /// <summary>
        /// 轨道半径长(AU)
        /// </summary>
        public double SemimajorAxis { get; set; }

        /// <summary>
        /// 轨道离心率
        /// </summary>
        public double Eccentricity { get; set; }

        /// <summary>
        /// 轨道与黄道夹角
        /// </summary>
        public double EclipticInclination { get; set; }

        /// <summary>
        /// 升交点黄经
        /// </summary>
        public double AscendingNodeLongitude { get; set; }

        /// <summary>
        /// 近日点黄经
        /// </summary>
        public double PerihelionLongitude { get; set; }
    }
}
