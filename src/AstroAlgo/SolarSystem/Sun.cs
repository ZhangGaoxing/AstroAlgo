using AstroAlgo.Basic;
using AstroAlgo.Models;

using System;

namespace AstroAlgo.SolarSystem
{
    /// <summary>
    /// 太阳
    /// </summary>
    public class Sun : IStar
    {
        private double latitude;
        private double longitude;
        private TimeZoneInfo localZone;

        #region Property

        /// <summary>
        /// 当前视赤道坐标
        /// </summary>
        public Equator Equator
        { 
            get 
            { 
                return EquatorialCoordinate(DateTime.Now, true); 
            } 
        }

        /// <summary>
        /// 当前视黄道坐标
        /// </summary>
        public Ecliptic Ecliptic
        {
            get
            {
                return CoordinateSystem.Equatorial2Ecliptic(Equator, DateTime.Now, true);
            }
        }

        /// <summary>
        /// 当日日出时间
        /// </summary>
        public TimeSpan Rise
        {
            get
            {
                var e = EquatorialCoordinate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.833, latitude, longitude, DateTime.Now, localZone);

                var span = TimeSpan.FromHours(time[0] / 15.0);

                return new TimeSpan(span.Hours, span.Minutes, span.Seconds);
            }
        }

        /// <summary>
        /// 当日中天时间
        /// </summary>
        public TimeSpan Culmination
        {
            get
            {
                var e = EquatorialCoordinate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.833, latitude, longitude, DateTime.Now, localZone);

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
        /// 当日日落时间
        /// </summary>
        public TimeSpan Down
        {
            get
            {
                var e = EquatorialCoordinate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.833, latitude, longitude, DateTime.Now, localZone);

                var span = TimeSpan.FromHours(time[1] / 15.0);

                return new TimeSpan(span.Hours, span.Minutes, span.Seconds);
            }
        }

        /// <summary>
        /// 日地距离(AU)
        /// </summary>
        public double ToEarth
        {
            get
            {
                return ToEarthDistance(DateTime.Now);
            }
        }

        /// <summary>
        /// 当前高度角
        /// </summary>
        public double ElevationAngle
        {
            get
            {
                return CoordinateSystem.GetElevationAngle(DateTime.Now, Equator, latitude, longitude);
            }
        }

        /// <summary>
        /// 当前方位角
        /// </summary>
        public double Azimuth
        {
            get
            {
                return CoordinateSystem.GetAzimuth(DateTime.Now, Equator, latitude, longitude);
            }
        }

        #endregion

        /// <summary>
        /// 初始化一个太阳实例
        /// </summary>
        /// <param name="latitude">观测地纬度</param>
        /// <param name="longitude">观测地经度</param>
        /// <param name="localZone">时区</param>
        public Sun(double latitude, double longitude, TimeZoneInfo localZone)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.localZone = localZone;
        }

        #region Method

        /// <summary>
        /// 计算指定时间的太阳赤经与赤纬
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="isApparent">是否为视赤道坐标</param>
        /// <returns>赤经赤纬 (0°-360°)</returns>
        public static Equator EquatorialCoordinate(DateTime time, bool isApparent = false)
        {
            double T = (Julian.ToJulianDay(time) - 2451545.0) / 36525.0;

            double L0 = 280.46645 + 36000.76983 * T + 0.0003030 * T * T;
            double M = 357.52910 + 35999.05030 * T - 0.0001559 * T * T - 0.00000048 * T * T * T;
            double e = 0.016708617 - 0.000042037 * T - 0.0000001236 * T * T;

            L0 = BasicTools.AngleSimplification(L0);
            M = BasicTools.AngleSimplification(M);
            e = BasicTools.AngleSimplification(e);

            double C = Math.Abs((1.914600 - 0.004817 * T - 0.000014 * T * T)) * Math.Sin(M * (Math.PI / 180.0)) + (0.019993 - 0.000101 * T) * Math.Sin(2 * M * (Math.PI / 180.0)) + 0.000290 * Math.Sin(3 * M * (Math.PI / 180.0));
            C = BasicTools.AngleSimplification(C);

            double theta = L0 + C;

            double theta2000 = theta - 0.01397 * (time.Year - 2000);

            double omega = 125.04 - 1934.136 * T;
            omega = BasicTools.AngleSimplification(omega);
            double lambda = theta - 0.00569 - 0.00478 * Math.Sin(omega * (Math.PI / 180.0));

            double sinDelta, delta, alpha;

            if (isApparent)
            {
                sinDelta = Math.Sin((CoordinateSystem.GetEclipticObliquity(time, false) + 0.00256 * Math.Cos(omega) * (Math.PI / 180.0)) * (Math.PI / 180.0)) * Math.Sin(lambda * (Math.PI / 180.0));
                delta = Math.Asin(sinDelta) * (180.0 / Math.PI);

                alpha = Math.Atan2(Math.Cos((CoordinateSystem.GetEclipticObliquity(time, false) + 0.00256 * Math.Cos(omega) * (Math.PI / 180.0)) * (Math.PI / 180.0)) * Math.Sin(lambda * (Math.PI / 180.0)), Math.Cos(lambda * (Math.PI / 180.0))) * (180.0 / Math.PI);
            }
            else
            {
                sinDelta = Math.Sin(CoordinateSystem.GetEclipticObliquity(time, false) * (Math.PI / 180.0)) * Math.Sin(theta2000 * (Math.PI / 180.0));
                delta = Math.Asin(sinDelta) * (180.0 / Math.PI);

                alpha = Math.Atan2(Math.Cos(CoordinateSystem.GetEclipticObliquity(time, false) * (Math.PI / 180.0)) * Math.Sin(theta2000 * (Math.PI / 180.0)), Math.Cos(theta2000 * (Math.PI / 180.0))) * (180.0 / Math.PI);
            }            

            if (alpha <= 0)
            {
                while (!(alpha >= 0 && alpha <= 360))
                {
                    alpha += 360;
                }
            }

            Equator c = new Equator
            {
                RA = alpha,
                Dec = delta
            };

            return c;
        }

        /// <summary>
        /// 日地距离
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>距离，单位为“天文单位”</returns>
        public static double ToEarthDistance(DateTime time)
        {
            double T = (Julian.ToJulianDay(time) - 2451545.0) / 36525.0;

            double M = 357.52910 + 35999.05030 * T - 0.0001559 * T * T - 0.00000048 * T * T * T;
            double e = 0.016708617 - 0.000042037 * T - 0.0000001236 * T * T;

            M = BasicTools.AngleSimplification(M);
            e = BasicTools.AngleSimplification(e);

            double C = Math.Abs((1.914600 - 0.004817 * T - 0.000014 * T * T)) * Math.Sin(M * (Math.PI / 180.0)) + (0.019993 - 0.000101 * T) * Math.Sin(2 * M * (Math.PI / 180.0)) + 0.000290 * Math.Sin(3 * M * (Math.PI / 180.0));
            C = BasicTools.AngleSimplification(C);

            double v = M + C;
            v = BasicTools.AngleSimplification(v);

            double distance = 1.0000011018 * (1 - e * e) / (1 + e * Math.Cos(v * (Math.PI / 180.0)));

            return distance;
        }

        /// <summary>
        /// 计算分至点
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="localZone">时区</param>
        /// <returns>DateTime 数组，长度为4，内容为春分、夏至、秋分、冬至时间</returns>
        public static DateTime[] EquinoxAndSolstice(int year, TimeZoneInfo localZone)
        {
            double Y = (year - 2000) / 1000.0;
            // Spring
            double JDE0 = 2451623.80984 + 365242.37404 * Y + 0.05169 * Y * Y - 0.00411 * Y * Y * Y - 0.00057 * Y * Y * Y * Y;
            // Summer
            double JDE1 = 2451716.56767 + 365241.62603 * Y + 0.00325 * Y * Y + 0.00888 * Y * Y * Y - 0.00030 * Y * Y * Y * Y;
            // Autumn
            double JDE2 = 2451810.21715 + 365242.01767 * Y - 0.11575 * Y * Y + 0.00337 * Y * Y * Y + 0.00078 * Y * Y * Y * Y;
            // Winter
            double JDE3 = 2451900.05952 + 365242.74049 * Y - 0.06223 * Y * Y - 0.00823 * Y * Y * Y + 0.00032 * Y * Y * Y * Y;

            double[] JDE = new double[] { JDE0, JDE1, JDE2, JDE3 };

            DateTime[] result = new DateTime[4];

            for (int i = 0; i < 4; i++)
            {
                double T = (JDE[i] - 2451545.0) / 36525.0;
                double W = 35999.373 * T - 2.47;
                double lambda = 1 + 0.0334 * Math.Cos(W * (Math.PI / 180)) + 0.0007 * Math.Cos(2 * W * (Math.PI / 180));
                double S = 485 * Math.Cos((324.96 + 1934.136 * T) * (Math.PI / 180)) +
                           203 * Math.Cos((337.23 + 32964.467 * T) * (Math.PI / 180)) +
                           199 * Math.Cos((342.08 + 20.186 * T) * (Math.PI / 180)) +
                           182 * Math.Cos((27.85 + 445267.112 * T) * (Math.PI / 180)) +
                           156 * Math.Cos((73.14 + 45036.886 * T) * (Math.PI / 180)) +
                           136 * Math.Cos((171.52 + 22518.443 * T) * (Math.PI / 180)) +
                           77 * Math.Cos((222.54 + 65928.934 * T) * (Math.PI / 180)) +
                           74 * Math.Cos((296.72 + 3034.906 * T) * (Math.PI / 180)) +
                           70 * Math.Cos((243.58 + 9037.513 * T) * (Math.PI / 180)) +
                           58 * Math.Cos((119.81 + 33718.147 * T) * (Math.PI / 180)) +
                           52 * Math.Cos((297.17 + 150.678 * T) * (Math.PI / 180)) +
                           50 * Math.Cos((21.02 + 2281.226 * T) * (Math.PI / 180)) +
                           45 * Math.Cos((247.54 + 29929.562 * T) * (Math.PI / 180)) +
                           44 * Math.Cos((325.15 + 31555.956 * T) * (Math.PI / 180)) +
                           29 * Math.Cos((60.93 + 4443.417 * T) * (Math.PI / 180)) +
                           18 * Math.Cos((155.12 + 67555.328 * T) * (Math.PI / 180)) +
                           17 * Math.Cos((288.79 + 4562.452 * T) * (Math.PI / 180)) +
                           16 * Math.Cos((198.04 + 62894.029 * T) * (Math.PI / 180)) +
                           14 * Math.Cos((199.76 + 31436.921 * T) * (Math.PI / 180)) +
                           12 * Math.Cos((95.39 + 14577.848 * T) * (Math.PI / 180)) +
                           12 * Math.Cos((287.11 + 31931.756 * T) * (Math.PI / 180)) +
                           12 * Math.Cos((320.81 + 34777.259 * T) * (Math.PI / 180)) +
                           9 * Math.Cos((227.73 + 1222.114 * T) * (Math.PI / 180)) +
                           8 * Math.Cos((15.45 + 16859.074 * T) * (Math.PI / 180));

                double JD = JDE[i] + 0.00001 * S / lambda;

                result[i] = TimeZoneInfo.ConvertTime(Julian.ToCalendarDay(JD), TimeZoneInfo.Utc, localZone);
            }

            return result;
        }

        /// <summary>
        /// 计算二十四节气
        /// 若计算分至点请使用方法 EquinoxAndSolstice()
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="term">节气</param>
        /// <param name="localZone">时区</param>
        /// <returns>节气时间</returns>
        public static DateTime SolarTerms(int year, SolarTerm term, TimeZoneInfo localZone)
        {
            EstimateSTtimeScope(year, term, out double lJD, out double rJD); /*估算迭代起始时间区间*/

            double solarTermsJD = 0.0;
            double longitude = 0.0;
            DateTime solarTermsTime = new DateTime();

            do
            {
                solarTermsJD = ((rJD - lJD) * 0.618) + lJD;
                solarTermsTime = Julian.ToCalendarDay(solarTermsJD);
                longitude = CoordinateSystem.Equatorial2Ecliptic(EquatorialCoordinate(solarTermsTime, true), solarTermsTime, true).Longitude;
                /*
                    对黄经0度迭代逼近时，由于角度360度圆周性，估算黄经值可能在(345,360]和[0,15)两个区间，
                    如果值落入前一个区间，需要进行修正
                */
                longitude = ((term == 0) && (longitude > 345.0)) ? longitude - 360.0 : longitude;
                if (longitude > (int)term)
                {
                    rJD = solarTermsJD;
                }
                else
                {
                    lJD = solarTermsJD;
                }
            } while ((rJD - lJD) > 0.0001);

            // 分钟+2修正
            return TimeZoneInfo.ConvertTime(solarTermsTime.AddMinutes(2), TimeZoneInfo.Utc, localZone);
        }

        internal static void EstimateSTtimeScope(int year, SolarTerm term, out double lJD, out double rJD)
        {
            switch (term)
            {
                case SolarTerm.BeginningOfSpring:
                    lJD = Julian.ToJulianDay(new DateTime(year, 2, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 2, 9));
                    break;
                case SolarTerm.RainWater:
                    lJD = Julian.ToJulianDay(new DateTime(year, 2, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 2, 24));
                    break;
                case SolarTerm.InsectsAwakening:
                    lJD = Julian.ToJulianDay(new DateTime(year, 3, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 3, 9));
                    break;
                case SolarTerm.SpringEquinox:
                    lJD = Julian.ToJulianDay(new DateTime(year, 3, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 3, 24));
                    break;
                case SolarTerm.FreshGreen:
                    lJD = Julian.ToJulianDay(new DateTime(year, 4, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 4, 9));
                    break;
                case SolarTerm.GrainRain:
                    lJD = Julian.ToJulianDay(new DateTime(year, 4, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 4, 24));
                    break;
                case SolarTerm.BeginningOfSummer:
                    lJD = Julian.ToJulianDay(new DateTime(year, 5, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 5, 9));
                    break;
                case SolarTerm.LesserFullness:
                    lJD = Julian.ToJulianDay(new DateTime(year, 5, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 5, 24));
                    break;
                case SolarTerm.GrainInEar:
                    lJD = Julian.ToJulianDay(new DateTime(year, 6, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 6, 9));
                    break;
                case SolarTerm.SummerSolstice:
                    lJD = Julian.ToJulianDay(new DateTime(year, 6, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 6, 24));
                    break;
                case SolarTerm.LesserHeat:
                    lJD = Julian.ToJulianDay(new DateTime(year, 7, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 7, 9));
                    break;
                case SolarTerm.GreaterHeat:
                    lJD = Julian.ToJulianDay(new DateTime(year, 7, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 7, 24));
                    break;
                case SolarTerm.BeginningOfAutumn:
                    lJD = Julian.ToJulianDay(new DateTime(year, 8, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 8, 9));
                    break;
                case SolarTerm.EndOfHeat:
                    lJD = Julian.ToJulianDay(new DateTime(year, 8, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 8, 24));
                    break;
                case SolarTerm.WhiteDew:
                    lJD = Julian.ToJulianDay(new DateTime(year, 9, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 9, 9));
                    break;
                case SolarTerm.AutumnalEquinox:
                    lJD = Julian.ToJulianDay(new DateTime(year, 9, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 9, 24));
                    break;
                case SolarTerm.ColdDew:
                    lJD = Julian.ToJulianDay(new DateTime(year, 10, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 10, 9));
                    break;
                case SolarTerm.FirstFrost:
                    lJD = Julian.ToJulianDay(new DateTime(year, 10, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 10, 24));
                    break;
                case SolarTerm.BeginningOfWinter:
                    lJD = Julian.ToJulianDay(new DateTime(year, 11, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 11, 9));
                    break;
                case SolarTerm.LightSnow:
                    lJD = Julian.ToJulianDay(new DateTime(year, 11, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 11, 24));
                    break;
                case SolarTerm.HeavySnow:
                    lJD = Julian.ToJulianDay(new DateTime(year, 12, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 12, 9));
                    break;
                case SolarTerm.WinterSolstice:
                    lJD = Julian.ToJulianDay(new DateTime(year, 12, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 12, 24));
                    break;
                case SolarTerm.LesserCold:
                    lJD = Julian.ToJulianDay(new DateTime(year, 1, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 1, 9));
                    break;
                case SolarTerm.GreaterCold:
                    lJD = Julian.ToJulianDay(new DateTime(year, 1, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 1, 24));
                    break;
                default:
                    lJD = 0;
                    rJD = 0;
                    break;
            }
        }

        #endregion

        #region OldMethod
        /*

        /// <summary>
        /// 计算指定时间与地点的太阳高度角
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <returns>太阳高度角(-90°-90°)</returns>
        public static double ElevationAngle(DateTime date, double latitude, double longitude)
        {
            var coord = EquatorialCoordinate(date);
            double ra = coord.RA;
            double dec = coord.Dec;
            double localSiderealTime = SiderealTime.LocalSiderealTime(date, TimeZoneInfo.Local, longitude);

            double sinH = Math.Sin(latitude * Math.PI / 180.0) * Math.Sin(dec * Math.PI / 180.0) + Math.Cos(latitude * Math.PI / 180.0) * Math.Cos(dec * Math.PI / 180.0) * Math.Cos((localSiderealTime - ra) * Math.PI / 180.0);
            double H = Math.Asin(sinH) * 180.0 / Math.PI;

            return H;
        }

        /// <summary>
        /// 计算指定时间与地点的太阳中天高度角
        /// </summary>
        /// <param name="date"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static double CulminationAngle(DateTime date, double latitude)
        {
            var coord = EquatorialCoordinate(date);
            double dec = coord.Dec;

            double sinH = Math.Sin(latitude * Math.PI / 180.0) * Math.Sin(dec * Math.PI / 180.0) + Math.Cos(latitude * Math.PI / 180.0) * Math.Cos(dec * Math.PI / 180.0);
            double H = Math.Asin(sinH) * 180.0 / Math.PI;

            return H;
        }

        /// <summary>
        /// 计算指定时间与地点的太阳方位角
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <returns>太阳方位角(0°-360°)</returns>
        public static double Azimuth(DateTime date, double latitude, double longitude)
        {
            double elevationAngle = ElevationAngle(date, latitude, longitude);

            var coord = EquatorialCoordinate(date);
            double ra = coord.RA;
            double dec = coord.Dec;
            double localSiderealTime = SiderealTime.LocalSiderealTime(date, TimeZoneInfo.Local, longitude);

            double omega = localSiderealTime - ra;

            double cosA = (Math.Sin(dec * (Math.PI / 180.0)) - Math.Sin(elevationAngle * (Math.PI / 180.0)) * Math.Sin(latitude * (Math.PI / 180.0))) / (Math.Cos(elevationAngle * (Math.PI / 180.0)) * Math.Cos(latitude * (Math.PI / 180.0)));
            
            double A;
            if (omega < 0)
            {
                A = Math.Acos(cosA) * 180.0 / Math.PI;
            }
            else
            {
                A = 360 - (Math.Acos(cosA) * 180.0 / Math.PI);
            }

            return A;
        }

        /// <summary>
        /// 将太阳高度角转换为区时
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="angle">太阳高度角(-90°-90°)</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <returns>单位为“度”的区时</returns>
        public static double[] ElevationAngle2Time(DateTime date, double angle, double latitude, double longitude)
        {
            var coord = EquatorialCoordinate(date);
            double ra = coord.RA;
            double dec = coord.Dec;

            double sinH = Math.Sin(angle * (Math.PI / 180.0));
            double cosT = (sinH - Math.Sin(latitude * Math.PI / 180.0) * Math.Sin(dec * Math.PI / 180.0)) / (Math.Cos(latitude * Math.PI / 180.0) * Math.Cos(dec * Math.PI / 180.0));
            double T = Math.Acos(cosT) * 180.0 / Math.PI;

            double t1 = 360 - T;
            double t2 = T;

            double localSiderealTime1 = t1 + ra;
            double zoneTime1 = SiderealTime.LocalSiderealTime2ZoneTime(localSiderealTime1, date, TimeZoneInfo.Local, longitude);

            double localSiderealTime2 = t2 + ra;
            double zoneTime2 = SiderealTime.LocalSiderealTime2ZoneTime(localSiderealTime2, date, TimeZoneInfo.Local, longitude);

            return new double[] { zoneTime1, zoneTime2 };
        }
        */

        #endregion
    }
}