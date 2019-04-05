namespace AstroAlgo.Model
{
    /// <summary>
    /// 黄道坐标
    /// </summary>
    public class Ecliptic
    {
        /// <summary>
        /// 黄经（0-360）
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 黄纬（-90-90）
        /// </summary>
        public double Latitude { get; set; }
    }
}
