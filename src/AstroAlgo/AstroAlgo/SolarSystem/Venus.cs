﻿using AstroAlgo.Base;
using AstroAlgo.Model;

using System;

namespace AstroAlgo.SolarSystem
{
    /// <summary>
    /// 金星
    /// </summary>
    public class Venus : IPlanet
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
                var e = EclipticalCoordinate(DateTime.Now, true);
                return CoordinateSystem.Ecliptic2Equatorial(e, DateTime.Now, true);
            }
        }

        /// <summary>
        /// 当前视黄道坐标
        /// </summary>
        public Ecliptic Ecliptic
        {
            get
            {
                return EclipticalCoordinate(DateTime.Now, true);
            }
        }

        /// <summary>
        /// 当日升时间
        /// </summary>
        public TimeSpan Rise
        {
            get
            {
                var e0 = EclipticalCoordinate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var e = CoordinateSystem.Ecliptic2Equatorial(e0, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.5667, latitude, longitude, DateTime.Now, localZone);

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
                var e0 = EclipticalCoordinate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var e = CoordinateSystem.Ecliptic2Equatorial(e0, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.5667, latitude, longitude, DateTime.Now, localZone);

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
        /// 当日落时间
        /// </summary>
        public TimeSpan Down
        {
            get
            {
                var e0 = EclipticalCoordinate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var e = CoordinateSystem.Ecliptic2Equatorial(e0, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, -0.5667, latitude, longitude, DateTime.Now, localZone);

                var span = TimeSpan.FromHours(time[1] / 15.0);

                return new TimeSpan(span.Hours, span.Minutes, span.Seconds);
            }
        }

        /// <summary>
        /// 当前与地球距离(AU)
        /// </summary>
        public double ToEarth
        {
            get
            {
                return ToEarthDistance(DateTime.Now);
            }
        }

        /// <summary>
        /// 当前与太阳距离(AU)
        /// </summary>
        public double ToSun
        {
            get
            {
                return ToSunDistance(DateTime.Now);
            }
        }

        /// <summary>
        /// 当前高度角
        /// </summary>
        public double ElevationAngle
        {
            get
            {
                return CoordinateSystem.ElevationAngle(DateTime.Now, Equator, latitude, longitude);
            }
        }

        /// <summary>
        /// 当前方位角
        /// </summary>
        public double Azimuth
        {
            get
            {
                return CoordinateSystem.Azimuth(DateTime.Now, Equator, latitude, longitude);
            }
        }

        #endregion

        /// <summary>
        /// 初始化一个金星实例
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <param name="localZone">时区</param>
        public Venus(double latitude, double longitude, TimeZoneInfo localZone)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.localZone = localZone;
        }

        #region Method

        /// <summary>
        /// 计算行星轨道要素
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>轨道要素</returns>
        public static OrbitalElement GetOrbitalElement(DateTime time)
        {
            double T = (Julian.ToJulianDay(time) - 2451545) / 36525.0;

            double L = 118.979801 + 58519.2130303 * T + 0.00031060 * T * T + 0.000000015 * T * T * T;
            double a = 0.723329820;
            double e = 0.00677188 - 0.000047766 * T + 0.0000000975 * T * T + 0.00000000044 * T * T * T;
            double i = 3.394662 + 0.0010037 * T - 0.00000088 * T * T - 0.000000007 * T * T * T;
            double Omega = 76.679920 + 0.9011190 * T + 0.00040664 * T * T - 0.000000080 * T * T * T;
            double pi = 131.563707 + 1.4022188 * T - 0.00107337 * T * T - 0.000005315 * T * T * T;

            L = BasicTools.AngleSimplification(L);
            i = BasicTools.AngleSimplification(i);
            Omega = BasicTools.AngleSimplification(Omega);
            pi = BasicTools.AngleSimplification(pi);

            return new OrbitalElement()
            {
                MeanLongitude = L,
                SemimajorAxis = a,
                Eccentricity = e,
                EclipticInclination = i,
                AscendingNodeLongitude = Omega,
                PerihelionLongitude = pi
            };
        }

        /// <summary>
        /// 计算日心黄道坐标
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>日心黄道坐标</returns>
        public static Ecliptic HeliocentricEclipticCoordinate(DateTime time)
        {
            double t = (Julian.ToJulianDay(time) - 2451545) / 365250.0;

            return new Ecliptic
            {
                Longitude = BasicTools.AngleSimplification((Venus_L0(t) + Venus_L1(t) + Venus_L2(t) + Venus_L3(t) + Venus_L4(t) + Venus_L5(t)) * (180 / Math.PI)),
                Latitude = (Venus_B0(t) + Venus_B1(t) + Venus_B2(t) + Venus_B3(t) + Venus_B4(t) + Venus_B5(t)) * (180 / Math.PI)
            };
        }

        /// <summary>
        /// 计算到太阳的距离
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>距离（天文单位）</returns>
        public static double ToSunDistance(DateTime time)
        {
            double t = (Julian.ToJulianDay(time) - 2451545) / 365250.0;

            return Venus_R0(t) + Venus_R1(t) + Venus_R2(t) + Venus_R3(t) + Venus_R4(t) + Venus_R5(t);
        }

        /// <summary>
        /// 计算到地球的距离
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>距离（天文单位）</returns>
        public static double ToEarthDistance(DateTime time)
        {
            var earth = Earth.HeliocentricEclipticCoordinate(time);
            double r0 = Earth.ToSunDistance(time);
            var e1 = HeliocentricEclipticCoordinate(time);
            double r1 = ToSunDistance(time);

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

        /// <summary>
        /// 计算地心黄道坐标
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="isApparent">是否为视黄道坐标</param>
        /// <returns>地心视黄道坐标</returns>
        public static Ecliptic EclipticalCoordinate(DateTime time, bool isApparent = false)
        {
            var earth = Earth.HeliocentricEclipticCoordinate(time);
            double r0 = Earth.ToSunDistance(time);
            var e1 = HeliocentricEclipticCoordinate(time);
            double r1 = ToSunDistance(time);

            double x1 = r1 * Math.Cos(e1.Latitude * (Math.PI / 180.0)) * Math.Cos(e1.Longitude * (Math.PI / 180.0)) - r0 * Math.Cos(earth.Latitude * (Math.PI / 180.0)) * Math.Cos(earth.Longitude * (Math.PI / 180.0));
            double y1 = r1 * Math.Cos(e1.Latitude * (Math.PI / 180.0)) * Math.Sin(e1.Longitude * (Math.PI / 180.0)) - r0 * Math.Cos(earth.Latitude * (Math.PI / 180.0)) * Math.Sin(earth.Longitude * (Math.PI / 180.0));
            double z1 = r1 * Math.Sin(e1.Latitude * (Math.PI / 180.0)) - r0 * Math.Sin(earth.Latitude * (Math.PI / 180.0));

            if (isApparent)
            {
                double t = 0.0057755183 * Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);

                var e2 = HeliocentricEclipticCoordinate(time, t);
                double r2 = ToSunDistance(time, t);

                double x2 = r2 * Math.Cos(e2.Latitude * (Math.PI / 180.0)) * Math.Cos(e2.Longitude * (Math.PI / 180.0)) - r0 * Math.Cos(earth.Latitude * (Math.PI / 180.0)) * Math.Cos(earth.Longitude * (Math.PI / 180.0));
                double y2 = r2 * Math.Cos(e2.Latitude * (Math.PI / 180.0)) * Math.Sin(e2.Longitude * (Math.PI / 180.0)) - r0 * Math.Cos(earth.Latitude * (Math.PI / 180.0)) * Math.Sin(earth.Longitude * (Math.PI / 180.0));
                double z2 = r2 * Math.Sin(e2.Latitude * (Math.PI / 180.0)) - r0 * Math.Sin(earth.Latitude * (Math.PI / 180.0));
                var nutation = CoordinateSystem.GetNutation(time);
                // 未修正周年光行差
                // 修正了光行时与黄经章动
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

        #endregion

        #region internal

        /// <summary>
        /// 计算日心黄道坐标
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="jd">光行时修正</param>
        /// <returns>日心黄道坐标</returns>
        internal static Ecliptic HeliocentricEclipticCoordinate(DateTime time, double jd)
        {
            double t = (Julian.ToJulianDay(time) - jd - 2451545) / 365250.0 ;

            return new Ecliptic
            {
                Longitude = BasicTools.AngleSimplification((Venus_L0(t) + Venus_L1(t) + Venus_L2(t) + Venus_L3(t) + Venus_L4(t) + Venus_L5(t)) * (180 / Math.PI)),
                Latitude = (Venus_B0(t) + Venus_B1(t) + Venus_B2(t) + Venus_B3(t) + Venus_B4(t) + Venus_B5(t)) * (180 / Math.PI)
            };
        }

        /// <summary>
        /// 计算到太阳的距离
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="jd">光行时修正</param>
        /// <returns>距离（天文单位）</returns>
        internal static double ToSunDistance(DateTime time, double jd)
        {
            double t = (Julian.ToJulianDay(time) - jd - 2451545) / 365250.0;

            return Venus_R0(t) + Venus_R1(t) + Venus_R2(t) + Venus_R3(t) + Venus_R4(t) + Venus_R5(t);
        }

        #endregion

        #region VSOP87

        /*
           VENUS - VSOP87 Series Version D
           HELIOCENTRIC DYNAMICAL ECLIPTIC AND EQUINOX OF THE DATE
           Spherical (L,B,R) Coordinates

           Series Validity Span: 2000 BC < Date < 6000 AD
           Theoretical accuracy over span: ±1 arc sec

           L = Longitude in radians
           B = Latitude in radians
           R = Radius vector in AU (Astronomical Units)

           t = (JD - 2451545) / 365250


           C++ Programming Language

           VSOP87 Functions Source Code
           Generated By The VSOP87 Source Code Generator Tool
           (c) Jay Tanner 2017

           Ref:
           Planetary Theories in Rectangular and Spherical Variables
           VSOP87 Solutions
           Pierre Bretagnon, Gerard Francou
           Journal of Astronomy & Astrophysics
           vol. 202, p309-p315
           1988

           Source code provided under the provisions of the
           GNU General Public License (GPL), version 3.
           http://www.gnu.org/licenses/gpl.html
        */

        internal static double Venus_L0(double t) // 367 terms of order 0
        {
            double L0 = 0;
            L0 += 3.17614666774;
            L0 += 0.01353968419 * Math.Cos(5.59313319619 + 10213.285546211 * t);
            L0 += 0.00089891645 * Math.Cos(5.30650048468 + 20426.571092422 * t);
            L0 += 0.00005477201 * Math.Cos(4.41630652531 + 7860.4193924392 * t);
            L0 += 0.00003455732 * Math.Cos(2.69964470778 + 11790.6290886588 * t);
            L0 += 0.00002372061 * Math.Cos(2.99377539568 + 3930.2096962196 * t);
            L0 += 0.00001317108 * Math.Cos(5.18668219093 + 26.2983197998 * t);
            L0 += 0.00001664069 * Math.Cos(4.25018935030 + 1577.3435424478 * t);
            L0 += 0.00001438322 * Math.Cos(4.15745043958 + 9683.5945811164 * t);
            L0 += 0.00001200521 * Math.Cos(6.15357115319 + 30639.856638633 * t);
            L0 += 0.00000761380 * Math.Cos(1.95014702120 + 529.6909650946 * t);
            L0 += 0.00000707676 * Math.Cos(1.06466707214 + 775.522611324 * t);
            L0 += 0.00000584836 * Math.Cos(3.99839884762 + 191.4482661116 * t);
            L0 += 0.00000769314 * Math.Cos(0.81629615911 + 9437.762934887 * t);
            L0 += 0.00000499915 * Math.Cos(4.12340210074 + 15720.8387848784 * t);
            L0 += 0.00000326221 * Math.Cos(4.59056473097 + 10404.7338123226 * t);
            L0 += 0.00000429498 * Math.Cos(3.58642859752 + 19367.1891622328 * t);
            L0 += 0.00000326967 * Math.Cos(5.67736583705 + 5507.5532386674 * t);
            L0 += 0.00000231937 * Math.Cos(3.16251057072 + 9153.9036160218 * t);
            L0 += 0.00000179695 * Math.Cos(4.65337915578 + 1109.3785520934 * t);
            L0 += 0.00000128263 * Math.Cos(4.22604493736 + 20.7753954924 * t);
            L0 += 0.00000155464 * Math.Cos(5.57043888948 + 19651.048481098 * t);
            L0 += 0.00000127907 * Math.Cos(0.96209822685 + 5661.3320491522 * t);
            L0 += 0.00000105547 * Math.Cos(1.53721191253 + 801.8209311238 * t);
            L0 += 0.00000085722 * Math.Cos(0.35589249966 + 3154.6870848956 * t);
            L0 += 0.00000099121 * Math.Cos(0.83288185132 + 213.299095438 * t);
            L0 += 0.00000098804 * Math.Cos(5.39389655503 + 13367.9726311066 * t);
            L0 += 0.00000082094 * Math.Cos(3.21596990826 + 18837.49819713819 * t);
            L0 += 0.00000088031 * Math.Cos(3.88868860307 + 9999.986450773 * t);
            L0 += 0.00000071577 * Math.Cos(0.11145739345 + 11015.1064773348 * t);
            L0 += 0.00000056122 * Math.Cos(4.24039855475 + 7.1135470008 * t);
            L0 += 0.00000070239 * Math.Cos(0.67458813282 + 23581.2581773176 * t);
            L0 += 0.00000050796 * Math.Cos(0.24531603049 + 11322.6640983044 * t);
            L0 += 0.00000046111 * Math.Cos(5.31576465717 + 18073.7049386502 * t);
            L0 += 0.00000044574 * Math.Cos(6.06282201966 + 40853.142184844 * t);
            L0 += 0.00000042594 * Math.Cos(5.32873337210 + 2352.8661537718 * t);
            L0 += 0.00000042635 * Math.Cos(1.79955421680 + 7084.8967811152 * t);
            L0 += 0.00000041177 * Math.Cos(0.36240972161 + 382.8965322232 * t);
            L0 += 0.00000035749 * Math.Cos(2.70448479296 + 10206.1719992102 * t);
            L0 += 0.00000033893 * Math.Cos(2.02347322198 + 6283.0758499914 * t);
            L0 += 0.00000029138 * Math.Cos(3.59230925768 + 22003.9146348698 * t);
            L0 += 0.00000028479 * Math.Cos(2.22375414002 + 1059.3819301892 * t);
            L0 += 0.00000029850 * Math.Cos(4.02176977477 + 10239.5838660108 * t);
            L0 += 0.00000033252 * Math.Cos(2.10025596509 + 27511.4678735372 * t);
            L0 += 0.00000030172 * Math.Cos(4.94191919890 + 13745.3462390224 * t);
            L0 += 0.00000029252 * Math.Cos(3.51392387787 + 283.8593188652 * t);
            L0 += 0.00000024424 * Math.Cos(2.70177493852 + 8624.2126509272 * t);
            L0 += 0.00000020274 * Math.Cos(3.79493637509 + 14143.4952424306 * t);
            L0 += 0.00000024322 * Math.Cos(4.27814493315 + 5.5229243074 * t);
            L0 += 0.00000026260 * Math.Cos(0.54067587552 + 17298.1823273262 * t);
            L0 += 0.00000020492 * Math.Cos(0.58547075036 + 38.0276726358 * t);
            L0 += 0.00000018988 * Math.Cos(4.13811500642 + 4551.9534970588 * t);
            L0 += 0.00000023739 * Math.Cos(4.82870797552 + 6872.6731195112 * t);
            L0 += 0.00000015885 * Math.Cos(1.50067222283 + 8635.9420037632 * t);
            L0 += 0.00000019069 * Math.Cos(6.12025580313 + 29050.7837433492 * t);
            L0 += 0.00000018269 * Math.Cos(3.04740408477 + 19999.97290154599 * t);
            L0 += 0.00000013656 * Math.Cos(4.41336292334 + 3532.0606928114 * t);
            L0 += 0.00000017094 * Math.Cos(3.52161526430 + 31441.6775697568 * t);
            L0 += 0.00000010955 * Math.Cos(2.84562790076 + 18307.8072320436 * t);
            L0 += 0.00000011048 * Math.Cos(2.58361219075 + 9786.687355335 * t);
            L0 += 0.00000009904 * Math.Cos(1.08737710389 + 7064.1213856228 * t);
            L0 += 0.00000010576 * Math.Cos(0.85419784436 + 10596.1820784342 * t);
            L0 += 0.00000009231 * Math.Cos(5.52471655579 + 12566.1516999828 * t);
            L0 += 0.00000011599 * Math.Cos(5.81007422699 + 19896.8801273274 * t);
            L0 += 0.00000011807 * Math.Cos(1.91250672543 + 21228.3920235458 * t);
            L0 += 0.00000010105 * Math.Cos(2.34270786693 + 10742.9765113056 * t);
            L0 += 0.00000008154 * Math.Cos(1.92331359797 + 15.252471185 * t);
            L0 += 0.00000008893 * Math.Cos(1.97291388515 + 10186.9872264112 * t);
            L0 += 0.00000009352 * Math.Cos(4.94508904764 + 35371.8872659764 * t);
            L0 += 0.00000006821 * Math.Cos(4.39733188968 + 8662.240323563 * t);
            L0 += 0.00000006688 * Math.Cos(1.55310437864 + 14945.3161735544 * t);
            L0 += 0.00000006413 * Math.Cos(2.17677652923 + 10988.808157535 * t);
            L0 += 0.00000005802 * Math.Cos(1.93462125906 + 3340.6124266998 * t);
            L0 += 0.00000005950 * Math.Cos(2.96578175391 + 4732.0306273434 * t);
            L0 += 0.00000005275 * Math.Cos(5.01877102496 + 28286.9904848612 * t);
            L0 += 0.00000007047 * Math.Cos(1.00111452053 + 632.7837393132 * t);
            L0 += 0.00000005048 * Math.Cos(4.27886209626 + 29580.4747084438 * t);
            L0 += 0.00000006305 * Math.Cos(0.35506331180 + 103.0927742186 * t);
            L0 += 0.00000005959 * Math.Cos(5.04792949464 + 245.8316462294 * t);
            L0 += 0.00000004651 * Math.Cos(0.85218058876 + 6770.7106012456 * t);
            L0 += 0.00000005580 * Math.Cos(0.48723384809 + 522.5774180938 * t);
            L0 += 0.00000005327 * Math.Cos(3.03115417024 + 10021.8372800994 * t);
            L0 += 0.00000005010 * Math.Cos(5.77375166500 + 28521.0927782546 * t);
            L0 += 0.00000004608 * Math.Cos(1.93302108394 + 4705.7323075436 * t);
            L0 += 0.00000005526 * Math.Cos(3.36797048901 + 25158.6017197654 * t);
            L0 += 0.00000003863 * Math.Cos(4.89351531412 + 25934.1243310894 * t);
            L0 += 0.00000005303 * Math.Cos(0.08161426841 + 39302.096962196 * t);
            L0 += 0.00000004254 * Math.Cos(5.36046113295 + 21535.9496445154 * t);
            L0 += 0.00000003763 * Math.Cos(1.05304597315 + 19.66976089979 * t);
            L0 += 0.00000004407 * Math.Cos(4.02575374517 + 74.7815985673 * t);
            L0 += 0.00000004145 * Math.Cos(1.14356412295 + 9676.4810341156 * t);
            L0 += 0.00000004318 * Math.Cos(4.38289970585 + 316.3918696566 * t);
            L0 += 0.00000003642 * Math.Cos(6.11733529325 + 3128.3887650958 * t);
            L0 += 0.00000003238 * Math.Cos(5.39551036769 + 419.4846438752 * t);
            L0 += 0.00000003909 * Math.Cos(4.05263635330 + 9690.7081281172 * t);
            L0 += 0.00000003152 * Math.Cos(0.72553551731 + 16496.3613962024 * t);
            L0 += 0.00000003496 * Math.Cos(0.72414615705 + 3723.508958923 * t);
            L0 += 0.00000003755 * Math.Cos(3.80208713127 + 19786.67380610799 * t);
            L0 += 0.00000002891 * Math.Cos(3.33782737770 + 32217.2001810808 * t);
            L0 += 0.00000003016 * Math.Cos(1.57249112496 + 17277.4069318338 * t);
            L0 += 0.00000003825 * Math.Cos(0.19612312903 + 426.598190876 * t);
            L0 += 0.00000003798 * Math.Cos(0.45524571743 + 10316.3783204296 * t);
            L0 += 0.00000002570 * Math.Cos(1.20813474107 + 13936.794505134 * t);
            L0 += 0.00000002796 * Math.Cos(3.65128969074 + 206.1855484372 * t);
            L0 += 0.00000002466 * Math.Cos(3.61988676373 + 1551.045222648 * t);
            L0 += 0.00000003108 * Math.Cos(1.50325806664 + 43232.3066584156 * t);
            L0 += 0.00000002976 * Math.Cos(4.79415001304 + 29088.811415985 * t);
            L0 += 0.00000002217 * Math.Cos(3.59623681714 + 24356.7807886416 * t);
            L0 += 0.00000002227 * Math.Cos(4.96059221940 + 536.8045120954 * t);
            L0 += 0.00000002397 * Math.Cos(3.45249688427 + 19374.3027092336 * t);
            L0 += 0.00000002462 * Math.Cos(0.53295178258 + 19360.07561523199 * t);
            L0 += 0.00000002205 * Math.Cos(2.70399309963 + 12592.4500197826 * t);
            L0 += 0.00000002230 * Math.Cos(3.01413465913 + 18875.525869774 * t);
            L0 += 0.00000001858 * Math.Cos(4.06129152783 + 2379.1644735716 * t);
            L0 += 0.00000001807 * Math.Cos(3.15086214479 + 9573.388259897 * t);
            L0 += 0.00000002238 * Math.Cos(5.52216925076 + 10138.5039476437 * t);
            L0 += 0.00000002195 * Math.Cos(2.32046770554 + 8094.5216858326 * t);
            L0 += 0.00000002101 * Math.Cos(2.90421302975 + 9967.4538999816 * t);
            L0 += 0.00000001916 * Math.Cos(4.56513949099 + 2218.7571041868 * t);
            L0 += 0.00000001467 * Math.Cos(2.42640162465 + 10234.0609417034 * t);
            L0 += 0.00000001726 * Math.Cos(5.59790693845 + 20452.8694122218 * t);
            L0 += 0.00000001455 * Math.Cos(2.44757248737 + 1589.0728952838 * t);
            L0 += 0.00000001991 * Math.Cos(4.04623390359 + 31749.2351907264 * t);
            L0 += 0.00000001406 * Math.Cos(2.71736996917 + 16983.9961474566 * t);
            L0 += 0.00000001658 * Math.Cos(0.11252373292 + 153.7788104848 * t);
            L0 += 0.00000001851 * Math.Cos(2.92898027939 + 47162.5163546352 * t);
            L0 += 0.00000001492 * Math.Cos(1.07513892753 + 9103.9069941176 * t);
            L0 += 0.00000001247 * Math.Cos(2.48433565896 + 17778.11626694899 * t);
            L0 += 0.00000001549 * Math.Cos(4.20553654300 + 3442.5749449654 * t);
            L0 += 0.00000001243 * Math.Cos(3.95452438599 + 170.6728706192 * t);
            L0 += 0.00000001694 * Math.Cos(6.20694480406 + 33019.0211122046 * t);
            L0 += 0.00000001221 * Math.Cos(4.77931820602 + 30110.1656735384 * t);
            L0 += 0.00000001206 * Math.Cos(0.30531303095 + 29864.334027309 * t);
            L0 += 0.00000001238 * Math.Cos(5.05581820608 + 20213.271996984 * t);
            L0 += 0.00000001152 * Math.Cos(3.26229919481 + 11.729352836 * t);
            L0 += 0.00000001179 * Math.Cos(1.69491074791 + 20400.2727726222 * t);
            L0 += 0.00000001165 * Math.Cos(2.88995128147 + 574.3447983348 * t);
            L0 += 0.00000001306 * Math.Cos(0.14519588607 + 9146.790069021 * t);
            L0 += 0.00000001113 * Math.Cos(1.52598846804 + 10426.584641649 * t);
            L0 += 0.00000001104 * Math.Cos(2.58791423813 + 18849.2275499742 * t);
            L0 += 0.00000001045 * Math.Cos(0.57539216420 + 15874.6175953632 * t);
            L0 += 0.00000001360 * Math.Cos(2.41976595457 + 38734.3783244656 * t);
            L0 += 0.00000000981 * Math.Cos(4.37930727798 + 110.2063212194 * t);
            L0 += 0.00000001095 * Math.Cos(0.49492867814 + 51066.427731055 * t);
            L0 += 0.00000001146 * Math.Cos(4.54241454215 + 10220.3990932118 * t);
            L0 += 0.00000000981 * Math.Cos(1.65915064733 + 10103.0792249916 * t);
            L0 += 0.00000001270 * Math.Cos(4.69374306132 + 9050.8108418032 * t);
            L0 += 0.00000001065 * Math.Cos(4.41645258887 + 22805.7355659936 * t);
            L0 += 0.00000000854 * Math.Cos(2.34437926957 + 6681.2248533996 * t);
            L0 += 0.00000001104 * Math.Cos(0.49781459714 + 1.4844727083 * t);
            L0 += 0.00000001075 * Math.Cos(1.09857593161 + 377.3736079158 * t);
            L0 += 0.00000001114 * Math.Cos(4.35024775806 + 51092.7260508548 * t);
            L0 += 0.00000000829 * Math.Cos(5.41196274578 + 27991.40181316 * t);
            L0 += 0.00000000900 * Math.Cos(2.74195379617 + 41962.5207369374 * t);
            L0 += 0.00000001010 * Math.Cos(2.96092073452 + 135.62532501 * t);
            L0 += 0.00000000768 * Math.Cos(3.98260860494 + 18844.61174413899 * t);
            L0 += 0.00000001018 * Math.Cos(1.36891050752 + 36949.2308084242 * t);
            L0 += 0.00000000726 * Math.Cos(1.67728773965 + 21202.093703746 * t);
            L0 += 0.00000000727 * Math.Cos(0.89048212541 + 467.9649903544 * t);
            L0 += 0.00000000869 * Math.Cos(2.93767679827 + 10192.5101507186 * t);
            L0 += 0.00000000696 * Math.Cos(5.35698039414 + 10063.7223490764 * t);
            L0 += 0.00000000920 * Math.Cos(4.17128923588 + 18734.4054229196 * t);
            L0 += 0.00000000691 * Math.Cos(1.50594097883 + 27197.2816936676 * t);
            L0 += 0.00000000835 * Math.Cos(0.48050621092 + 20618.0193585336 * t);
            L0 += 0.00000000711 * Math.Cos(0.19750098222 + 18830.38465013739 * t);
            L0 += 0.00000000811 * Math.Cos(0.16685071959 + 12432.0426503978 * t);
            L0 += 0.00000000756 * Math.Cos(3.79022623226 + 9161.0171630226 * t);
            L0 += 0.00000000622 * Math.Cos(5.33659507738 + 9411.4646150872 * t);
            L0 += 0.00000000862 * Math.Cos(5.72705356405 + 10175.1525105732 * t);
            L0 += 0.00000000853 * Math.Cos(0.10404188453 + 2107.0345075424 * t);
            L0 += 0.00000000742 * Math.Cos(3.96365892051 + 813.5502839598 * t);
            L0 += 0.00000000705 * Math.Cos(0.71229660616 + 220.4126424388 * t);
            L0 += 0.00000000584 * Math.Cos(1.71900692700 + 36.0278666774 * t);
            L0 += 0.00000000612 * Math.Cos(0.36418385449 + 949.1756089698 * t);
            L0 += 0.00000000587 * Math.Cos(1.58648949290 + 6.62855890001 * t);
            L0 += 0.00000000581 * Math.Cos(5.49288908804 + 6309.3741697912 * t);
            L0 += 0.00000000581 * Math.Cos(4.80353237853 + 24150.080051345 * t);
            L0 += 0.00000000516 * Math.Cos(6.07328802561 + 38.1330356378 * t);
            L0 += 0.00000000627 * Math.Cos(5.47281424954 + 9580.5018068978 * t);
            L0 += 0.00000000601 * Math.Cos(1.40500080774 + 1162.4747044078 * t);
            L0 += 0.00000000620 * Math.Cos(4.00681042667 + 9992.8729037722 * t);
            L0 += 0.00000000611 * Math.Cos(3.62010998629 + 7255.5696517344 * t);
            L0 += 0.00000000697 * Math.Cos(2.22359630727 + 348.924420448 * t);
            L0 += 0.00000000693 * Math.Cos(5.77432072851 + 55022.9357470744 * t);
            L0 += 0.00000000494 * Math.Cos(0.29761886866 + 7058.5984613154 * t);
            L0 += 0.00000000563 * Math.Cos(0.24172140474 + 37410.5672398786 * t);
            L0 += 0.00000000487 * Math.Cos(5.86917216517 + 10137.0194749354 * t);
            L0 += 0.00000000493 * Math.Cos(2.04534833854 + 735.8765135318 * t);
            L0 += 0.00000000636 * Math.Cos(2.79707392326 + 40879.4405046438 * t);
            L0 += 0.00000000519 * Math.Cos(4.13945657630 + 16522.6597160022 * t);
            L0 += 0.00000000535 * Math.Cos(4.60569597820 + 19573.37471066999 * t);
            L0 += 0.00000000555 * Math.Cos(5.88120421263 + 26735.9452622132 * t);
            L0 += 0.00000000440 * Math.Cos(5.61490649795 + 23958.6317852334 * t);
            L0 += 0.00000000541 * Math.Cos(0.62494922735 + 10007.0999977738 * t);
            L0 += 0.00000000427 * Math.Cos(4.02335620501 + 14.2270940016 * t);
            L0 += 0.00000000434 * Math.Cos(0.29028429049 + 9264.1099372412 * t);
            L0 += 0.00000000451 * Math.Cos(1.66320363626 + 26087.9031415742 * t);
            L0 += 0.00000000422 * Math.Cos(3.38413582674 + 10787.6303445458 * t);
            L0 += 0.00000000569 * Math.Cos(5.14001758731 + 27490.6924780448 * t);
            L0 += 0.00000000421 * Math.Cos(4.23407313457 + 39793.7602546548 * t);
            L0 += 0.00000000458 * Math.Cos(5.28786368820 + 49.7570254718 * t);
            L0 += 0.00000000418 * Math.Cos(5.69097790790 + 14765.2390432698 * t);
            L0 += 0.00000000475 * Math.Cos(0.97544690438 + 1052.2683831884 * t);
            L0 += 0.00000000387 * Math.Cos(4.41665162999 + 21.8508293264 * t);
            L0 += 0.00000000523 * Math.Cos(2.90512426870 + 20235.1228263104 * t);
            L0 += 0.00000000506 * Math.Cos(5.26999314618 + 29999.959352319 * t);
            L0 += 0.00000000438 * Math.Cos(2.49457071132 + 20956.2620575166 * t);
            L0 += 0.00000000530 * Math.Cos(0.71368442157 + 33794.5437235286 * t);
            L0 += 0.00000000382 * Math.Cos(1.92119365480 + 3.9321532631 * t);
            L0 += 0.00000000365 * Math.Cos(3.81715328784 + 20419.45754542119 * t);
            L0 += 0.00000000426 * Math.Cos(2.06384083608 + 38204.687359371 * t);
            L0 += 0.00000000496 * Math.Cos(0.44077356179 + 9835.9119382952 * t);
            L0 += 0.00000000410 * Math.Cos(4.93346326003 + 19264.0963880142 * t);
            L0 += 0.00000000380 * Math.Cos(3.79573358631 + 8521.1198767086 * t);
            L0 += 0.00000000334 * Math.Cos(5.51158557799 + 10251.3132188468 * t);
            L0 += 0.00000000412 * Math.Cos(2.56129670728 + 77.673770428 * t);
            L0 += 0.00000000418 * Math.Cos(2.37865963521 + 32.5325507914 * t);
            L0 += 0.00000000325 * Math.Cos(6.03020523465 + 18947.7045183576 * t);
            L0 += 0.00000000400 * Math.Cos(0.91999360201 + 227.476132789 * t);
            L0 += 0.00000000437 * Math.Cos(0.91420135162 + 58953.145443294 * t);
            L0 += 0.00000000360 * Math.Cos(0.82477639126 + 22.7752014508 * t);
            L0 += 0.00000000413 * Math.Cos(4.22381905655 + 44809.6502008634 * t);
            L0 += 0.00000000375 * Math.Cos(3.15657291896 + 19992.85935454519 * t);
            L0 += 0.00000000371 * Math.Cos(6.05370874275 + 20007.0864485468 * t);
            L0 += 0.00000000361 * Math.Cos(5.44371227904 + 19470.28193645139 * t);
            L0 += 0.00000000386 * Math.Cos(5.28120540405 + 47623.8527860896 * t);
            L0 += 0.00000000389 * Math.Cos(0.73216672240 + 19050.7972925762 * t);
            L0 += 0.00000000320 * Math.Cos(2.84811591194 + 10199.0584522094 * t);
            L0 += 0.00000000386 * Math.Cos(3.88754165531 + 1975.492545856 * t);
            L0 += 0.00000000276 * Math.Cos(4.33979180814 + 20809.4676246452 * t);
            L0 += 0.00000000276 * Math.Cos(0.50647429773 + 9830.3890139878 * t);
            L0 += 0.00000000309 * Math.Cos(3.79299100668 + 18204.71445782499 * t);
            L0 += 0.00000000377 * Math.Cos(0.73768791281 + 11506.7697697936 * t);
            L0 += 0.00000000322 * Math.Cos(0.96138442100 + 30666.1549584328 * t);
            L0 += 0.00000000363 * Math.Cos(1.30472406690 + 9367.2027114598 * t);
            L0 += 0.00000000366 * Math.Cos(2.79972786028 + 11272.6674764002 * t);
            L0 += 0.00000000271 * Math.Cos(4.66141338193 + 846.0828347512 * t);
            L0 += 0.00000000259 * Math.Cos(0.42031175750 + 39264.0692895602 * t);
            L0 += 0.00000000285 * Math.Cos(0.40546033634 + 30.914125635 * t);
            L0 += 0.00000000247 * Math.Cos(4.80676426942 + 36147.4098773004 * t);
            L0 += 0.00000000264 * Math.Cos(2.71608177583 + 11.0457002639 * t);
            L0 += 0.00000000233 * Math.Cos(2.76423842887 + 187.9251477626 * t);
            L0 += 0.00000000248 * Math.Cos(1.60765612338 + 10497.1448650762 * t);
            L0 += 0.00000000271 * Math.Cos(0.82348919630 + 19793.7873531088 * t);
            L0 += 0.00000000225 * Math.Cos(3.80080957016 + 8631.326197928 * t);
            L0 += 0.00000000263 * Math.Cos(1.92311689852 + 37724.7534197482 * t);
            L0 += 0.00000000214 * Math.Cos(5.01663795092 + 639.897286314 * t);
            L0 += 0.00000000289 * Math.Cos(0.12342601246 + 20277.0078952874 * t);
            L0 += 0.00000000210 * Math.Cos(0.12771800254 + 29.8214381488 * t);
            L0 += 0.00000000227 * Math.Cos(4.18036609801 + 17468.8551979454 * t);
            L0 += 0.00000000274 * Math.Cos(2.34929343000 + 62883.3551395136 * t);
            L0 += 0.00000000260 * Math.Cos(5.65254501655 + 48739.859897083 * t);
            L0 += 0.00000000271 * Math.Cos(4.95325404028 + 4214.0690150848 * t);
            L0 += 0.00000000219 * Math.Cos(2.08775228014 + 194.9713844606 * t);
            L0 += 0.00000000191 * Math.Cos(2.49267248333 + 568.8218740274 * t);
            L0 += 0.00000000250 * Math.Cos(1.52909737354 + 6037.244203762 * t);
            L0 += 0.00000000231 * Math.Cos(5.23674429498 + 491.6632924588 * t);
            L0 += 0.00000000182 * Math.Cos(4.98046042571 + 18418.01355326299 * t);
            L0 += 0.00000000188 * Math.Cos(2.82273639603 + 1385.8952763362 * t);
            L0 += 0.00000000204 * Math.Cos(4.09939796199 + 14919.0178537546 * t);
            L0 += 0.00000000176 * Math.Cos(3.82400982460 + 9360.089164459 * t);
            L0 += 0.00000000198 * Math.Cos(2.76491873243 + 10217.2176994741 * t);
            L0 += 0.00000000168 * Math.Cos(5.19268384202 + 1066.49547719 * t);
            L0 += 0.00000000199 * Math.Cos(1.95301487982 + 7564.830720738 * t);
            L0 += 0.00000000171 * Math.Cos(2.59623459612 + 20405.7956969296 * t);
            L0 += 0.00000000172 * Math.Cos(5.29332132623 + 11764.330768859 * t);
            L0 += 0.00000000165 * Math.Cos(2.88557908025 + 10207.7626219036 * t);
            L0 += 0.00000000164 * Math.Cos(3.25435371801 + 3914.9572250346 * t);
            L0 += 0.00000000200 * Math.Cos(3.82443218090 + 18314.9207790444 * t);
            L0 += 0.00000000169 * Math.Cos(1.78341902878 + 31022.7531708562 * t);
            L0 += 0.00000000179 * Math.Cos(0.90840065587 + 7880.08915333899 * t);
            L0 += 0.00000000163 * Math.Cos(2.79665037814 + 41.5507909848 * t);
            L0 += 0.00000000154 * Math.Cos(3.90796293476 + 30213.258447757 * t);
            L0 += 0.00000000153 * Math.Cos(0.07463240782 + 28528.2063252554 * t);
            L0 += 0.00000000194 * Math.Cos(5.95838706838 + 8617.0991039264 * t);
            L0 += 0.00000000171 * Math.Cos(4.58206324409 + 20447.3464879144 * t);
            L0 += 0.00000000150 * Math.Cos(2.11647586229 + 17248.4253018544 * t);
            L0 += 0.00000000149 * Math.Cos(2.17259986320 + 9929.4262273458 * t);
            L0 += 0.00000000191 * Math.Cos(0.82310353823 + 52670.0695933026 * t);
            L0 += 0.00000000148 * Math.Cos(2.94315921485 + 41654.9631159678 * t);
            L0 += 0.00000000149 * Math.Cos(4.49798039726 + 30831.3049047446 * t);
            L0 += 0.00000000184 * Math.Cos(2.46923348701 + 34596.3646546524 * t);
            L0 += 0.00000000146 * Math.Cos(2.69452930300 + 43071.8992890308 * t);
            L0 += 0.00000000159 * Math.Cos(2.11137713570 + 19317.1925403286 * t);
            L0 += 0.00000000154 * Math.Cos(2.76536164654 + 28513.97923125379 * t);
            L0 += 0.00000000140 * Math.Cos(4.94595038686 + 9256.9963902404 * t);
            L0 += 0.00000000141 * Math.Cos(2.57248458154 + 13553.8979729108 * t);
            L0 += 0.00000000137 * Math.Cos(1.66482327575 + 2636.725472637 * t);
            L0 += 0.00000000140 * Math.Cos(5.23039605990 + 22645.32819660879 * t);
            L0 += 0.00000000132 * Math.Cos(5.35690599728 + 19624.7501612982 * t);
            L0 += 0.00000000140 * Math.Cos(2.90637712614 + 48947.6638706766 * t);
            L0 += 0.00000000129 * Math.Cos(3.95303623681 + 32858.61374281979 * t);
            L0 += 0.00000000156 * Math.Cos(6.01143316387 + 29057.89729034999 * t);
            L0 += 0.00000000134 * Math.Cos(5.75241675118 + 68050.42387851159 * t);
            L0 += 0.00000000154 * Math.Cos(3.66827363753 + 276.7457718644 * t);
            L0 += 0.00000000176 * Math.Cos(3.77298381177 + 66813.5648357332 * t);
            L0 += 0.00000000126 * Math.Cos(5.00217740223 + 27461.7108480654 * t);
            L0 += 0.00000000135 * Math.Cos(1.34807013920 + 53285.1848352418 * t);
            L0 += 0.00000000150 * Math.Cos(0.25029475344 + 290.972865866 * t);
            L0 += 0.00000000152 * Math.Cos(3.13035670092 + 29043.67019634839 * t);
            L0 += 0.00000000169 * Math.Cos(5.04348109430 + 73.297125859 * t);
            L0 += 0.00000000166 * Math.Cos(5.39219948035 + 41236.0387170672 * t);
            L0 += 0.00000000163 * Math.Cos(5.59796070987 + 7576.560073574 * t);
            L0 += 0.00000000126 * Math.Cos(0.77391784606 + 49.9966219042 * t);
            L0 += 0.00000000163 * Math.Cos(0.44241846674 + 20350.3050211464 * t);
            L0 += 0.00000000136 * Math.Cos(3.09066368912 + 418.9243989006 * t);
            L0 += 0.00000000154 * Math.Cos(0.47086190960 + 28418.000004036 * t);
            L0 += 0.00000000120 * Math.Cos(0.88536981986 + 29573.361161443 * t);
            L0 += 0.00000000132 * Math.Cos(1.48009769040 + 17085.9586657222 * t);
            L0 += 0.00000000126 * Math.Cos(1.39497760964 + 966.9708774356 * t);
            L0 += 0.00000000143 * Math.Cos(3.84026797958 + 14128.2427712456 * t);
            L0 += 0.00000000147 * Math.Cos(2.11627427804 + 34363.365597556 * t);
            L0 += 0.00000000106 * Math.Cos(2.04696932293 + 37674.9963942764 * t);
            L0 += 0.00000000106 * Math.Cos(1.43873202489 + 27682.1407441564 * t);
            L0 += 0.00000000149 * Math.Cos(0.09286508794 + 8144.2787113044 * t);
            L0 += 0.00000000103 * Math.Cos(0.01992041470 + 18300.69368504279 * t);
            L0 += 0.00000000121 * Math.Cos(3.57602835443 + 45.1412196366 * t);
            L0 += 0.00000000125 * Math.Cos(0.11630302078 + 149.5631971346 * t);
            L0 += 0.00000000102 * Math.Cos(4.17947097730 + 2333.196392872 * t);
            L0 += 0.00000000099 * Math.Cos(1.51324741657 + 10419.4710946482 * t);
            L0 += 0.00000000133 * Math.Cos(3.02183293676 + 76251.32777062019 * t);
            L0 += 0.00000000136 * Math.Cos(4.17517197268 + 3646.3503773544 * t);
            L0 += 0.00000000123 * Math.Cos(0.44045588682 + 515.463871093 * t);
            L0 += 0.00000000113 * Math.Cos(5.69261397718 + 10110.1927719924 * t);
            L0 += 0.00000000098 * Math.Cos(6.23797900467 + 202.2533951741 * t);
            L0 += 0.00000000099 * Math.Cos(3.75627530197 + 59728.668054618 * t);
            L0 += 0.00000000101 * Math.Cos(4.62832557536 + 65236.2212932854 * t);
            L0 += 0.00000000111 * Math.Cos(1.25947267588 + 10846.0692855242 * t);
            L0 += 0.00000000110 * Math.Cos(5.87455577536 + 38500.2760310722 * t);
            L0 += 0.00000000128 * Math.Cos(6.01024562160 + 90394.82301305079 * t);
            L0 += 0.00000000091 * Math.Cos(1.77665981007 + 1539.315869812 * t);
            L0 += 0.00000000092 * Math.Cos(0.99804571578 + 95.9792272178 * t);
            L0 += 0.00000000120 * Math.Cos(3.93060866244 + 38526.574350872 * t);
            L0 += 0.00000000117 * Math.Cos(2.24143299549 + 56600.2792895222 * t);
            L0 += 0.00000000118 * Math.Cos(6.09121325940 + 29786.660256881 * t);
            L0 += 0.00000000098 * Math.Cos(4.60938156207 + 11787.1059703098 * t);
            L0 += 0.00000000097 * Math.Cos(3.92727733144 + 11794.1522070078 * t);
            L0 += 0.00000000093 * Math.Cos(5.23395435043 + 14169.7935622304 * t);
            L0 += 0.00000000096 * Math.Cos(5.27525709038 + 8734.4189721466 * t);
            L0 += 0.00000000094 * Math.Cos(0.18166654805 + 67589.08744705719 * t);
            L0 += 0.00000000110 * Math.Cos(4.96279287076 + 48417.97290558199 * t);
            L0 += 0.00000000086 * Math.Cos(0.39533409505 + 3956.5080160194 * t);
            L0 += 0.00000000085 * Math.Cos(5.69642646462 + 37703.9780242558 * t);
            L0 += 0.00000000114 * Math.Cos(5.19676285428 + 70743.77453195279 * t);
            L0 += 0.00000000081 * Math.Cos(5.51324815184 + 412.3710968744 * t);
            L0 += 0.00000000089 * Math.Cos(2.13409771828 + 44768.0994098786 * t);
            L0 += 0.00000000084 * Math.Cos(6.02475904578 + 10632.7701900862 * t);
            L0 += 0.00000000085 * Math.Cos(4.60912614442 + 45585.1728121874 * t);
            L0 += 0.00000000078 * Math.Cos(4.47358603432 + 114.43928868521 * t);
            L0 += 0.00000000097 * Math.Cos(4.02223363535 + 10218.8084705184 * t);
            L0 += 0.00000000081 * Math.Cos(1.03870237004 + 9793.8009023358 * t);
            L0 += 0.00000000092 * Math.Cos(0.80301220092 + 24383.0791084414 * t);
            L0 += 0.00000000087 * Math.Cos(2.15124790938 + 28313.288804661 * t);
            L0 += 0.00000000075 * Math.Cos(5.17868679355 + 63658.8777508376 * t);
            L0 += 0.00000000078 * Math.Cos(5.81927313665 + 567.7186377304 * t);
            L0 += 0.00000000075 * Math.Cos(1.72618192481 + 19580.4882576708 * t);
            L0 += 0.00000000071 * Math.Cos(0.10259261764 + 90695.75207512038 * t);
            L0 += 0.00000000077 * Math.Cos(6.16012067704 + 1573.8204240988 * t);
            L0 += 0.00000000076 * Math.Cos(5.12884307551 + 49515.382508407 * t);
            L0 += 0.00000000069 * Math.Cos(0.29569499484 + 10175.2578735752 * t);
            L0 += 0.00000000061 * Math.Cos(4.80385549281 + 19889.76658032659 * t);
            L0 += 0.00000000060 * Math.Cos(4.56685040226 + 30426.557543195 * t);
            L0 += 0.00000000062 * Math.Cos(4.16222812699 + 42430.4857272918 * t);
            L0 += 0.00000000050 * Math.Cos(6.17899839001 + 22779.4372461938 * t);
            L0 += 0.00000000048 * Math.Cos(1.52546758016 + 20639.87018786 * t);
            L0 += 0.00000000046 * Math.Cos(4.41738494249 + 34570.0663348526 * t);
            L0 += 0.00000000037 * Math.Cos(4.69675087759 + 44007.8292697396 * t);
            return L0;
        }

        internal static double Venus_L1(double t) // 215 terms of order 1
        {
            double L1 = 0;
            L1 += 10213.52943052898;
            L1 += 0.00095707712 * Math.Cos(2.46424448979 + 10213.285546211 * t);
            L1 += 0.00014444977 * Math.Cos(0.51624564679 + 20426.571092422 * t);
            L1 += 0.00000213374 * Math.Cos(1.79547929368 + 30639.856638633 * t);
            L1 += 0.00000151669 * Math.Cos(6.10635282369 + 1577.3435424478 * t);
            L1 += 0.00000173904 * Math.Cos(2.65535879443 + 26.2983197998 * t);
            L1 += 0.00000082233 * Math.Cos(5.70234133730 + 191.4482661116 * t);
            L1 += 0.00000069734 * Math.Cos(2.68136034979 + 9437.762934887 * t);
            L1 += 0.00000052408 * Math.Cos(3.60013087656 + 775.522611324 * t);
            L1 += 0.00000038318 * Math.Cos(1.03379038025 + 529.6909650946 * t);
            L1 += 0.00000029633 * Math.Cos(1.25056322354 + 5507.5532386674 * t);
            L1 += 0.00000025056 * Math.Cos(6.10664792855 + 10404.7338123226 * t);
            L1 += 0.00000017772 * Math.Cos(6.19369798901 + 1109.3785520934 * t);
            L1 += 0.00000016510 * Math.Cos(2.64330452640 + 7.1135470008 * t);
            L1 += 0.00000014230 * Math.Cos(5.45138233941 + 9153.9036160218 * t);
            L1 += 0.00000012607 * Math.Cos(1.24464400689 + 40853.142184844 * t);
            L1 += 0.00000011627 * Math.Cos(4.97604495371 + 213.299095438 * t);
            L1 += 0.00000012563 * Math.Cos(1.88122199199 + 382.8965322232 * t);
            L1 += 0.00000008869 * Math.Cos(0.95282732248 + 13367.9726311066 * t);
            L1 += 0.00000007374 * Math.Cos(4.39476760580 + 10206.1719992102 * t);
            L1 += 0.00000006552 * Math.Cos(2.28168808058 + 2352.8661537718 * t);
            L1 += 0.00000006255 * Math.Cos(4.08056644034 + 3154.6870848956 * t);
            L1 += 0.00000006697 * Math.Cos(5.05673427795 + 801.8209311238 * t);
            L1 += 0.00000004084 * Math.Cos(4.12103826030 + 18837.49819713819 * t);
            L1 += 0.00000004882 * Math.Cos(3.44515199115 + 11015.1064773348 * t);
            L1 += 0.00000003549 * Math.Cos(6.19934345402 + 5.5229243074 * t);
            L1 += 0.00000003448 * Math.Cos(1.77405651704 + 11322.6640983044 * t);
            L1 += 0.00000004290 * Math.Cos(0.08154809210 + 6283.0758499914 * t);
            L1 += 0.00000003694 * Math.Cos(2.48453945256 + 5661.3320491522 * t);
            L1 += 0.00000003555 * Math.Cos(1.48036949420 + 1059.3819301892 * t);
            L1 += 0.00000003023 * Math.Cos(2.24092938317 + 18073.7049386502 * t);
            L1 += 0.00000003000 * Math.Cos(0.39169917698 + 15.252471185 * t);
            L1 += 0.00000002563 * Math.Cos(0.35147506973 + 22003.9146348698 * t);
            L1 += 0.00000002774 * Math.Cos(1.45683830639 + 10239.5838660108 * t);
            L1 += 0.00000002951 * Math.Cos(5.34618097429 + 7084.8967811152 * t);
            L1 += 0.00000002344 * Math.Cos(2.36652432105 + 17298.1823273262 * t);
            L1 += 0.00000002405 * Math.Cos(2.36085282088 + 10596.1820784342 * t);
            L1 += 0.00000001720 * Math.Cos(4.72129626061 + 10186.9872264112 * t);
            L1 += 0.00000002209 * Math.Cos(2.07730338665 + 8635.9420037632 * t);
            L1 += 0.00000002122 * Math.Cos(4.47091605309 + 8624.2126509272 * t);
            L1 += 0.00000001527 * Math.Cos(0.67146857292 + 14143.4952424306 * t);
            L1 += 0.00000001473 * Math.Cos(2.59350470099 + 7064.1213856228 * t);
            L1 += 0.00000001311 * Math.Cos(0.90408820221 + 12566.1516999828 * t);
            L1 += 0.00000001474 * Math.Cos(5.92236241437 + 9786.687355335 * t);
            L1 += 0.00000001237 * Math.Cos(2.59740787132 + 4551.9534970588 * t);
            L1 += 0.00000001219 * Math.Cos(2.83617320088 + 9676.4810341156 * t);
            L1 += 0.00000001116 * Math.Cos(3.83715584719 + 21228.3920235458 * t);
            L1 += 0.00000001006 * Math.Cos(4.26200749078 + 426.598190876 * t);
            L1 += 0.00000001150 * Math.Cos(2.35531987378 + 9690.7081281172 * t);
            L1 += 0.00000001219 * Math.Cos(2.27324315182 + 522.5774180938 * t);
            L1 += 0.00000001150 * Math.Cos(0.81088598778 + 10742.9765113056 * t);
            L1 += 0.00000001101 * Math.Cos(3.74248783564 + 18307.8072320436 * t);
            L1 += 0.00000001031 * Math.Cos(2.03889374176 + 38.0276726358 * t);
            L1 += 0.00000000971 * Math.Cos(6.10590045414 + 3532.0606928114 * t);
            L1 += 0.00000000844 * Math.Cos(4.75124127613 + 10988.808157535 * t);
            L1 += 0.00000000908 * Math.Cos(1.06613723738 + 10021.8372800994 * t);
            L1 += 0.00000000824 * Math.Cos(0.23090829723 + 28286.9904848612 * t);
            L1 += 0.00000000821 * Math.Cos(2.60456032773 + 19.66976089979 * t);
            L1 += 0.00000000728 * Math.Cos(0.10716917942 + 4705.7323075436 * t);
            L1 += 0.00000000744 * Math.Cos(3.33129778857 + 536.8045120954 * t);
            L1 += 0.00000000816 * Math.Cos(1.27303930175 + 19896.8801273274 * t);
            L1 += 0.00000000929 * Math.Cos(1.08024621325 + 11790.6290886588 * t);
            L1 += 0.00000000797 * Math.Cos(2.23891816523 + 3723.508958923 * t);
            L1 += 0.00000000704 * Math.Cos(5.95307260017 + 20.7753954924 * t);
            L1 += 0.00000000665 * Math.Cos(0.21346689192 + 7860.4193924392 * t);
            L1 += 0.00000000733 * Math.Cos(2.22147883292 + 19360.07561523199 * t);
            L1 += 0.00000000702 * Math.Cos(1.76206343944 + 19374.3027092336 * t);
            L1 += 0.00000000575 * Math.Cos(2.38792087791 + 6770.7106012456 * t);
            L1 += 0.00000000538 * Math.Cos(1.52023264138 + 25934.1243310894 * t);
            L1 += 0.00000000690 * Math.Cos(4.01873754171 + 19651.048481098 * t);
            L1 += 0.00000000532 * Math.Cos(4.41576130890 + 574.3447983348 * t);
            L1 += 0.00000000540 * Math.Cos(2.15936134728 + 16496.3613962024 * t);
            L1 += 0.00000000576 * Math.Cos(5.41170044566 + 206.1855484372 * t);
            L1 += 0.00000000482 * Math.Cos(0.40815793538 + 3340.6124266998 * t);
            L1 += 0.00000000501 * Math.Cos(3.08578363577 + 245.8316462294 * t);
            L1 += 0.00000000488 * Math.Cos(5.22311611589 + 25158.6017197654 * t);
            L1 += 0.00000000450 * Math.Cos(0.21279844600 + 11.729352836 * t);
            L1 += 0.00000000432 * Math.Cos(1.32004964493 + 103.0927742186 * t);
            L1 += 0.00000000434 * Math.Cos(5.91094755233 + 19786.67380610799 * t);
            L1 += 0.00000000564 * Math.Cos(0.38776462529 + 19367.1891622328 * t);
            L1 += 0.00000000421 * Math.Cos(2.71057839701 + 13936.794505134 * t);
            L1 += 0.00000000549 * Math.Cos(6.08792865644 + 3930.2096962196 * t);
            L1 += 0.00000000478 * Math.Cos(4.70234715828 + 14945.3161735544 * t);
            L1 += 0.00000000408 * Math.Cos(4.80890663927 + 32217.2001810808 * t);
            L1 += 0.00000000404 * Math.Cos(2.85003595942 + 29864.334027309 * t);
            L1 += 0.00000000407 * Math.Cos(2.94002049006 + 10220.3990932118 * t);
            L1 += 0.00000000359 * Math.Cos(0.72354778897 + 419.4846438752 * t);
            L1 += 0.00000000449 * Math.Cos(1.44520508753 + 8662.240323563 * t);
            L1 += 0.00000000353 * Math.Cos(2.22195492336 + 51066.427731055 * t);
            L1 += 0.00000000324 * Math.Cos(1.40308439067 + 29580.4747084438 * t);
            L1 += 0.00000000443 * Math.Cos(1.93864353398 + 9146.790069021 * t);
            L1 += 0.00000000314 * Math.Cos(0.96837035284 + 20618.0193585336 * t);
            L1 += 0.00000000324 * Math.Cos(5.10759068171 + 24356.7807886416 * t);
            L1 += 0.00000000324 * Math.Cos(1.80146948625 + 18830.38465013739 * t);
            L1 += 0.00000000370 * Math.Cos(6.16895004656 + 2218.7571041868 * t);
            L1 += 0.00000000278 * Math.Cos(2.20429108375 + 18844.61174413899 * t);
            L1 += 0.00000000286 * Math.Cos(3.08459438435 + 17277.4069318338 * t);
            L1 += 0.00000000383 * Math.Cos(0.13890934755 + 4732.0306273434 * t);
            L1 += 0.00000000292 * Math.Cos(0.43528982259 + 29088.811415985 * t);
            L1 += 0.00000000273 * Math.Cos(5.84415407168 + 9573.388259897 * t);
            L1 += 0.00000000324 * Math.Cos(2.14144542781 + 9999.986450773 * t);
            L1 += 0.00000000264 * Math.Cos(5.20407029554 + 220.4126424388 * t);
            L1 += 0.00000000254 * Math.Cos(0.34411959301 + 28521.0927782546 * t);
            L1 += 0.00000000300 * Math.Cos(3.76014360906 + 8094.5216858326 * t);
            L1 += 0.00000000301 * Math.Cos(3.64457981649 + 20400.2727726222 * t);
            L1 += 0.00000000287 * Math.Cos(1.84003536598 + 1589.0728952838 * t);
            L1 += 0.00000000206 * Math.Cos(0.97167234723 + 10234.0609417034 * t);
            L1 += 0.00000000212 * Math.Cos(0.24173677600 + 36.0278666774 * t);
            L1 += 0.00000000216 * Math.Cos(5.88618923030 + 18875.525869774 * t);
            L1 += 0.00000000198 * Math.Cos(1.89506914939 + 20452.8694122218 * t);
            L1 += 0.00000000258 * Math.Cos(6.27611355094 + 1551.045222648 * t);
            L1 += 0.00000000197 * Math.Cos(2.09222675324 + 9683.5945811164 * t);
            L1 += 0.00000000217 * Math.Cos(5.79472589364 + 9103.9069941176 * t);
            L1 += 0.00000000188 * Math.Cos(0.39123199129 + 19573.37471066999 * t);
            L1 += 0.00000000195 * Math.Cos(6.23142464829 + 30110.1656735384 * t);
            L1 += 0.00000000187 * Math.Cos(5.49670351645 + 170.6728706192 * t);
            L1 += 0.00000000178 * Math.Cos(4.90042854659 + 10787.6303445458 * t);
            L1 += 0.00000000188 * Math.Cos(1.62614804098 + 9161.0171630226 * t);
            L1 += 0.00000000211 * Math.Cos(2.71884568392 + 15720.8387848784 * t);
            L1 += 0.00000000177 * Math.Cos(1.88170417337 + 33019.0211122046 * t);
            L1 += 0.00000000209 * Math.Cos(2.66033422116 + 3442.5749449654 * t);
            L1 += 0.00000000164 * Math.Cos(4.92240093026 + 10426.584641649 * t);
            L1 += 0.00000000186 * Math.Cos(5.13678812068 + 7255.5696517344 * t);
            L1 += 0.00000000177 * Math.Cos(5.70206821967 + 9992.8729037722 * t);
            L1 += 0.00000000214 * Math.Cos(2.70027196648 + 3128.3887650958 * t);
            L1 += 0.00000000208 * Math.Cos(3.38876526854 + 17778.11626694899 * t);
            L1 += 0.00000000147 * Math.Cos(4.25008782855 + 16983.9961474566 * t);
            L1 += 0.00000000148 * Math.Cos(3.46404418130 + 21202.093703746 * t);
            L1 += 0.00000000189 * Math.Cos(1.43553862242 + 2379.1644735716 * t);
            L1 += 0.00000000139 * Math.Cos(2.99154379541 + 110.2063212194 * t);
            L1 += 0.00000000159 * Math.Cos(5.23851679605 + 10007.0999977738 * t);
            L1 += 0.00000000136 * Math.Cos(0.88942869764 + 22805.7355659936 * t);
            L1 += 0.00000000155 * Math.Cos(5.90500835975 + 12592.4500197826 * t);
            L1 += 0.00000000151 * Math.Cos(0.03422618975 + 27991.40181316 * t);
            L1 += 0.00000000153 * Math.Cos(4.01743770323 + 33794.5437235286 * t);
            L1 += 0.00000000121 * Math.Cos(0.51392111799 + 21535.9496445154 * t);
            L1 += 0.00000000109 * Math.Cos(2.25388616761 + 26735.9452622132 * t);
            L1 += 0.00000000109 * Math.Cos(0.78612823474 + 6681.2248533996 * t);
            L1 += 0.00000000122 * Math.Cos(4.84805105466 + 19992.85935454519 * t);
            L1 += 0.00000000112 * Math.Cos(3.31796669604 + 36949.2308084242 * t);
            L1 += 0.00000000106 * Math.Cos(3.34507236765 + 10103.0792249916 * t);
            L1 += 0.00000000114 * Math.Cos(4.36384000196 + 20007.0864485468 * t);
            L1 += 0.00000000098 * Math.Cos(5.07711736751 + 135.62532501 * t);
            L1 += 0.00000000120 * Math.Cos(5.41870615047 + 37724.7534197482 * t);
            L1 += 0.00000000103 * Math.Cos(2.62610244425 + 20213.271996984 * t);
            L1 += 0.00000000085 * Math.Cos(5.04808202087 + 9830.3890139878 * t);
            L1 += 0.00000000103 * Math.Cos(2.01549383816 + 45585.1728121874 * t);
            L1 += 0.00000000088 * Math.Cos(2.62613816931 + 21.8508293264 * t);
            L1 += 0.00000000084 * Math.Cos(3.50355880173 + 639.897286314 * t);
            L1 += 0.00000000099 * Math.Cos(0.61079620895 + 41654.9631159678 * t);
            L1 += 0.00000000088 * Math.Cos(3.63836700262 + 49515.382508407 * t);
            L1 += 0.00000000098 * Math.Cos(2.42401801881 + 23581.2581773176 * t);
            L1 += 0.00000000081 * Math.Cos(0.46468679835 + 77.673770428 * t);
            L1 += 0.00000000092 * Math.Cos(4.82530051729 + 29043.67019634839 * t);
            L1 += 0.00000000102 * Math.Cos(4.27051236894 + 15874.6175953632 * t);
            L1 += 0.00000000090 * Math.Cos(4.34075776744 + 29057.89729034999 * t);
            L1 += 0.00000000081 * Math.Cos(0.01896422336 + 24150.080051345 * t);
            L1 += 0.00000000093 * Math.Cos(1.79250830018 + 12432.0426503978 * t);
            L1 += 0.00000000087 * Math.Cos(5.25157021446 + 14128.2427712456 * t);
            L1 += 0.00000000089 * Math.Cos(5.65756996753 + 377.3736079158 * t);
            L1 += 0.00000000097 * Math.Cos(5.67942873241 + 227.476132789 * t);
            L1 += 0.00000000076 * Math.Cos(2.93363913259 + 38204.687359371 * t);
            L1 += 0.00000000091 * Math.Cos(2.60544242067 + 1052.2683831884 * t);
            L1 += 0.00000000087 * Math.Cos(3.82284200928 + 27511.4678735372 * t);
            L1 += 0.00000000073 * Math.Cos(4.75280755154 + 40879.4405046438 * t);
            L1 += 0.00000000067 * Math.Cos(3.54815262526 + 30666.1549584328 * t);
            L1 += 0.00000000067 * Math.Cos(5.81350818057 + 20809.4676246452 * t);
            L1 += 0.00000000064 * Math.Cos(4.24772678145 + 153.7788104848 * t);
            L1 += 0.00000000064 * Math.Cos(2.99454749109 + 27197.2816936676 * t);
            L1 += 0.00000000070 * Math.Cos(4.03868009742 + 56600.2792895222 * t);
            L1 += 0.00000000071 * Math.Cos(4.33628806850 + 39264.0692895602 * t);
            L1 += 0.00000000069 * Math.Cos(1.73648747605 + 37410.5672398786 * t);
            L1 += 0.00000000065 * Math.Cos(1.08206062736 + 68050.42387851159 * t);
            L1 += 0.00000000062 * Math.Cos(4.77698454650 + 3914.9572250346 * t);
            L1 += 0.00000000061 * Math.Cos(4.96121014691 + 34596.3646546524 * t);
            L1 += 0.00000000063 * Math.Cos(5.04865067599 + 53445.5922046266 * t);
            L1 += 0.00000000058 * Math.Cos(3.74010494151 + 1066.49547719 * t);
            L1 += 0.00000000057 * Math.Cos(5.39355890141 + 20419.45754542119 * t);
            L1 += 0.00000000057 * Math.Cos(3.59399518494 + 735.8765135318 * t);
            L1 += 0.00000000065 * Math.Cos(2.10322000074 + 74.7815985673 * t);
            L1 += 0.00000000073 * Math.Cos(1.31083648835 + 11272.6674764002 * t);
            L1 += 0.00000000055 * Math.Cos(1.33161298098 + 18300.69368504279 * t);
            L1 += 0.00000000065 * Math.Cos(4.21150522641 + 49.7570254718 * t);
            L1 += 0.00000000061 * Math.Cos(5.66161679402 + 17468.8551979454 * t);
            L1 += 0.00000000053 * Math.Cos(4.30231233835 + 18849.2275499742 * t);
            L1 += 0.00000000055 * Math.Cos(2.63906959481 + 52670.0695933026 * t);
            L1 += 0.00000000050 * Math.Cos(5.69803054279 + 39793.7602546548 * t);
            L1 += 0.00000000049 * Math.Cos(0.77345264124 + 35371.8872659764 * t);
            L1 += 0.00000000048 * Math.Cos(6.00565977593 + 283.8593188652 * t);
            L1 += 0.00000000047 * Math.Cos(2.63299859494 + 51868.2486621788 * t);
            L1 += 0.00000000046 * Math.Cos(0.05105081843 + 38526.574350872 * t);
            L1 += 0.00000000050 * Math.Cos(4.37549274002 + 28513.97923125379 * t);
            L1 += 0.00000000046 * Math.Cos(2.93422086586 + 27682.1407441564 * t);
            L1 += 0.00000000051 * Math.Cos(5.45979584751 + 60530.4889857418 * t);
            L1 += 0.00000000045 * Math.Cos(5.59492908223 + 467.9649903544 * t);
            L1 += 0.00000000045 * Math.Cos(2.34680401001 + 9411.4646150872 * t);
            L1 += 0.00000000045 * Math.Cos(0.02999265111 + 44809.6502008634 * t);
            L1 += 0.00000000043 * Math.Cos(5.62725673544 + 14.2270940016 * t);
            L1 += 0.00000000047 * Math.Cos(3.73567275749 + 64460.6986819614 * t);
            L1 += 0.00000000046 * Math.Cos(0.12586526756 + 57375.8019008462 * t);
            L1 += 0.00000000044 * Math.Cos(2.03114426076 + 18314.9207790444 * t);
            L1 += 0.00000000039 * Math.Cos(0.99375127466 + 94138.32702008578 * t);
            L1 += 0.00000000053 * Math.Cos(0.41974404621 + 30831.3049047446 * t);
            L1 += 0.00000000055 * Math.Cos(1.38351566741 + 38500.2760310722 * t);
            L1 += 0.00000000041 * Math.Cos(4.47012768909 + 40077.61957352 * t);
            L1 += 0.00000000041 * Math.Cos(0.36665992484 + 19999.97290154599 * t);
            L1 += 0.00000000040 * Math.Cos(3.06358586355 + 813.5502839598 * t);
            L1 += 0.00000000040 * Math.Cos(2.16802870803 + 59728.668054618 * t);
            L1 += 0.00000000037 * Math.Cos(1.08739100421 + 17085.9586657222 * t);
            L1 += 0.00000000039 * Math.Cos(1.31040309875 + 48739.859897083 * t);
            L1 += 0.00000000036 * Math.Cos(1.43280677914 + 42456.7840470916 * t);
            L1 += 0.00000000037 * Math.Cos(0.14190533464 + 29050.7837433492 * t);
            L1 += 0.00000000037 * Math.Cos(3.66792179278 + 20956.2620575166 * t);
            L1 += 0.00000000025 * Math.Cos(3.38876180652 + 7058.5984613154 * t);
            L1 += 0.00000000031 * Math.Cos(6.16829805337 + 10192.5101507186 * t);
            return L1 * t;
        }

        internal static double Venus_L2(double t) // 70 terms of order 2
        {
            double L2 = 0;
            L2 += 0.00054127076;
            L2 += 0.00003891460 * Math.Cos(0.34514360047 + 10213.285546211 * t);
            L2 += 0.00001337880 * Math.Cos(2.02011286082 + 20426.571092422 * t);
            L2 += 0.00000023836 * Math.Cos(2.04592119012 + 26.2983197998 * t);
            L2 += 0.00000019331 * Math.Cos(3.53527371458 + 30639.856638633 * t);
            L2 += 0.00000009984 * Math.Cos(3.97130221102 + 775.522611324 * t);
            L2 += 0.00000007046 * Math.Cos(1.51962593409 + 1577.3435424478 * t);
            L2 += 0.00000006014 * Math.Cos(0.99926757893 + 191.4482661116 * t);
            L2 += 0.00000003163 * Math.Cos(4.36095475762 + 9437.762934887 * t);
            L2 += 0.00000002125 * Math.Cos(2.65810625752 + 40853.142184844 * t);
            L2 += 0.00000001934 * Math.Cos(3.39287946981 + 382.8965322232 * t);
            L2 += 0.00000001460 * Math.Cos(6.04899046273 + 529.6909650946 * t);
            L2 += 0.00000001346 * Math.Cos(2.94633106219 + 5507.5532386674 * t);
            L2 += 0.00000001025 * Math.Cos(1.40598904981 + 10404.7338123226 * t);
            L2 += 0.00000001221 * Math.Cos(3.73339139385 + 3154.6870848956 * t);
            L2 += 0.00000001033 * Math.Cos(3.52858472904 + 11015.1064773348 * t);
            L2 += 0.00000000955 * Math.Cos(5.11133878923 + 801.8209311238 * t);
            L2 += 0.00000000742 * Math.Cos(1.49198584483 + 1109.3785520934 * t);
            L2 += 0.00000000525 * Math.Cos(3.32087042103 + 213.299095438 * t);
            L2 += 0.00000000578 * Math.Cos(0.92614279843 + 10239.5838660108 * t);
            L2 += 0.00000000602 * Math.Cos(5.19220099775 + 7084.8967811152 * t);
            L2 += 0.00000000431 * Math.Cos(2.67159914364 + 13367.9726311066 * t);
            L2 += 0.00000000389 * Math.Cos(4.14116341739 + 8635.9420037632 * t);
            L2 += 0.00000000355 * Math.Cos(1.12061570874 + 9153.9036160218 * t);
            L2 += 0.00000000301 * Math.Cos(3.90047984197 + 10596.1820784342 * t);
            L2 += 0.00000000212 * Math.Cos(5.32697688872 + 18837.49819713819 * t);
            L2 += 0.00000000260 * Math.Cos(0.22761369281 + 2352.8661537718 * t);
            L2 += 0.00000000243 * Math.Cos(4.70747902991 + 6283.0758499914 * t);
            L2 += 0.00000000196 * Math.Cos(4.10467294392 + 11790.6290886588 * t);
            L2 += 0.00000000194 * Math.Cos(6.01197759470 + 7860.4193924392 * t);
            L2 += 0.00000000140 * Math.Cos(4.97015671653 + 14143.4952424306 * t);
            L2 += 0.00000000134 * Math.Cos(4.10529011674 + 17298.1823273262 * t);
            L2 += 0.00000000119 * Math.Cos(3.39375528828 + 11322.6640983044 * t);
            L2 += 0.00000000126 * Math.Cos(0.09854516140 + 18073.7049386502 * t);
            L2 += 0.00000000122 * Math.Cos(5.92478855457 + 574.3447983348 * t);
            L2 += 0.00000000107 * Math.Cos(0.35660030184 + 1059.3819301892 * t);
            L2 += 0.00000000108 * Math.Cos(2.25352052666 + 12566.1516999828 * t);
            L2 += 0.00000000093 * Math.Cos(5.48716819776 + 10021.8372800994 * t);
            L2 += 0.00000000084 * Math.Cos(4.89744332968 + 18307.8072320436 * t);
            L2 += 0.00000000074 * Math.Cos(2.35354025573 + 426.598190876 * t);
            L2 += 0.00000000093 * Math.Cos(4.99316908815 + 14945.3161735544 * t);
            L2 += 0.00000000069 * Math.Cos(3.86409065860 + 51066.427731055 * t);
            L2 += 0.00000000082 * Math.Cos(5.36280178643 + 10186.9872264112 * t);
            L2 += 0.00000000077 * Math.Cos(3.75728548372 + 3723.508958923 * t);
            L2 += 0.00000000063 * Math.Cos(5.39882267787 + 21228.3920235458 * t);
            L2 += 0.00000000056 * Math.Cos(4.11564786973 + 7064.1213856228 * t);
            L2 += 0.00000000056 * Math.Cos(6.26920407723 + 32217.2001810808 * t);
            L2 += 0.00000000060 * Math.Cos(5.02186497542 + 19367.1891622328 * t);
            L2 += 0.00000000058 * Math.Cos(5.13263709670 + 20400.2727726222 * t);
            L2 += 0.00000000051 * Math.Cos(4.52870390511 + 22003.9146348698 * t);
            L2 += 0.00000000041 * Math.Cos(3.83822107919 + 16496.3613962024 * t);
            L2 += 0.00000000041 * Math.Cos(3.36020411807 + 4705.7323075436 * t);
            L2 += 0.00000000043 * Math.Cos(5.98371820588 + 15720.8387848784 * t);
            L2 += 0.00000000047 * Math.Cos(0.18498155367 + 18875.525869774 * t);
            L2 += 0.00000000038 * Math.Cos(0.52232581277 + 1551.045222648 * t);
            L2 += 0.00000000039 * Math.Cos(5.05391675878 + 10742.9765113056 * t);
            L2 += 0.00000000036 * Math.Cos(3.16242472203 + 20452.8694122218 * t);
            L2 += 0.00000000035 * Math.Cos(5.17462577483 + 29088.811415985 * t);
            L2 += 0.00000000035 * Math.Cos(3.47325394141 + 24356.7807886416 * t);
            L2 += 0.00000000031 * Math.Cos(4.74511706231 + 28521.0927782546 * t);
            L2 += 0.00000000029 * Math.Cos(0.19383091192 + 19896.8801273274 * t);
            L2 += 0.00000000033 * Math.Cos(1.80059867635 + 20618.0193585336 * t);
            L2 += 0.00000000024 * Math.Cos(0.14022912457 + 21202.093703746 * t);
            L2 += 0.00000000022 * Math.Cos(4.73565067573 + 10988.808157535 * t);
            L2 += 0.00000000018 * Math.Cos(0.74039763161 + 25158.6017197654 * t);
            L2 += 0.00000000019 * Math.Cos(1.53770232218 + 28286.9904848612 * t);
            L2 += 0.00000000014 * Math.Cos(1.49084059765 + 30110.1656735384 * t);
            L2 += 0.00000000013 * Math.Cos(4.72171283479 + 29864.334027309 * t);
            L2 += 0.00000000013 * Math.Cos(5.79700427846 + 29580.4747084438 * t);
            L2 += 0.00000000014 * Math.Cos(3.69205225010 + 27511.4678735372 * t);
            return L2 * t * t;
        }

        internal static double Venus_L3(double t) // 9 terms of order 3
        {
            double L3 = 0;
            L3 += 0.00000135742 * Math.Cos(4.80389020993 + 10213.285546211 * t);
            L3 += 0.00000077846 * Math.Cos(3.66876371591 + 20426.571092422 * t);
            L3 += 0.00000026023;
            L3 += 0.00000001214 * Math.Cos(5.31970006917 + 30639.856638633 * t);
            L3 += 0.00000000254 * Math.Cos(4.15021671822 + 40853.142184844 * t);
            L3 += 0.00000000008 * Math.Cos(5.55523563261 + 51066.427731055 * t);
            L3 += 0.00000000008 * Math.Cos(1.40501229148 + 1577.3435424478 * t);
            L3 += 0.00000000006 * Math.Cos(1.27791479726 + 10404.7338123226 * t);
            L3 += 0.00000000006 * Math.Cos(5.76447068962 + 10239.5838660108 * t);
            return L3 * t * t * t;
        }

        internal static double Venus_L4(double t) // 5 terms of order 4
        {
            double L4 = 0;
            L4 -= 0.00000114016;
            L4 += 0.00000003209 * Math.Cos(5.20514170164 + 20426.571092422 * t);
            L4 += 0.00000001714 * Math.Cos(2.51099591706 + 10213.285546211 * t);
            L4 += 0.00000000050 * Math.Cos(0.71356059861 + 30639.856638633 * t);
            L4 += 0.00000000023 * Math.Cos(5.68127607034 + 40853.142184844 * t);
            return L4 * t * t * t * t;
        }

        internal static double Venus_L5(double t) // 5 terms of order 5
        {
            double L5 = 0;
            L5 -= 0.00000000874;
            L5 += 0.00000000117 * Math.Cos(0.54643013000 + 20426.571092422 * t);
            L5 += 0.00000000118 * Math.Cos(1.90548541922 + 10213.285546211 * t);
            L5 += 0.00000000002 * Math.Cos(1.07734277826 + 40853.142184844 * t);
            L5 += 0.00000000002 * Math.Cos(1.89460223529 + 30639.856638633 * t);
            return L5 * t * t * t * t * t;
        }

        internal static double Venus_B0(double t) // 210 terms of order 0
        {
            double B0 = 0;
            B0 += 0.05923638472 * Math.Cos(0.26702775813 + 10213.285546211 * t);
            B0 += 0.00040107978 * Math.Cos(1.14737178106 + 20426.571092422 * t);
            B0 -= 0.00032814918;
            B0 += 0.00001011392 * Math.Cos(1.08946123021 + 30639.856638633 * t);
            B0 += 0.00000149458 * Math.Cos(6.25390296069 + 18073.7049386502 * t);
            B0 += 0.00000137788 * Math.Cos(0.86020146523 + 1577.3435424478 * t);
            B0 += 0.00000129973 * Math.Cos(3.67152483651 + 9437.762934887 * t);
            B0 += 0.00000119507 * Math.Cos(3.70468812804 + 2352.8661537718 * t);
            B0 += 0.00000107971 * Math.Cos(4.53903677647 + 22003.9146348698 * t);
            B0 += 0.00000092029 * Math.Cos(1.53954562706 + 9153.9036160218 * t);
            B0 += 0.00000052982 * Math.Cos(2.28138172277 + 5507.5532386674 * t);
            B0 += 0.00000045617 * Math.Cos(0.72319641722 + 10239.5838660108 * t);
            B0 += 0.00000038855 * Math.Cos(2.93437865147 + 10186.9872264112 * t);
            B0 += 0.00000043491 * Math.Cos(6.14015776699 + 11790.6290886588 * t);
            B0 += 0.00000041700 * Math.Cos(5.99126845246 + 19896.8801273274 * t);
            B0 += 0.00000039644 * Math.Cos(3.86842095901 + 8635.9420037632 * t);
            B0 += 0.00000039175 * Math.Cos(3.94960351174 + 529.6909650946 * t);
            B0 += 0.00000033320 * Math.Cos(4.83194909595 + 14143.4952424306 * t);
            B0 += 0.00000023711 * Math.Cos(2.90646621218 + 10988.808157535 * t);
            B0 += 0.00000023500 * Math.Cos(2.00770618322 + 13367.9726311066 * t);
            B0 += 0.00000021809 * Math.Cos(2.69701424951 + 19651.048481098 * t);
            B0 += 0.00000020653 * Math.Cos(0.98666685459 + 775.522611324 * t);
            B0 += 0.00000016976 * Math.Cos(4.13711782135 + 10021.8372800994 * t);
            B0 += 0.00000017835 * Math.Cos(5.96268643102 + 25934.1243310894 * t);
            B0 += 0.00000014949 * Math.Cos(5.61075168206 + 10404.7338123226 * t);
            B0 += 0.00000018579 * Math.Cos(1.80529277514 + 40853.142184844 * t);
            B0 += 0.00000015407 * Math.Cos(3.29563855296 + 11015.1064773348 * t);
            B0 += 0.00000012936 * Math.Cos(5.42651448496 + 29580.4747084438 * t);
            B0 += 0.00000011962 * Math.Cos(3.57604253827 + 10742.9765113056 * t);
            B0 += 0.00000011827 * Math.Cos(1.19070919600 + 8624.2126509272 * t);
            B0 += 0.00000011466 * Math.Cos(5.12780364967 + 6283.0758499914 * t);
            B0 += 0.00000009484 * Math.Cos(2.75167834335 + 191.4482661116 * t);
            B0 += 0.00000013129 * Math.Cos(5.70735942511 + 9683.5945811164 * t);
            B0 += 0.00000008583 * Math.Cos(0.43182249199 + 9786.687355335 * t);
            B0 += 0.00000009763 * Math.Cos(0.14614896296 + 20618.0193585336 * t);
            B0 += 0.00000008148 * Math.Cos(1.30548515603 + 15720.8387848784 * t);
            B0 += 0.00000006050 * Math.Cos(6.26541665966 + 11322.6640983044 * t);
            B0 += 0.00000005955 * Math.Cos(4.92235372433 + 1059.3819301892 * t);
            B0 += 0.00000006983 * Math.Cos(3.44920932146 + 17298.1823273262 * t);
            B0 += 0.00000006228 * Math.Cos(1.13312070908 + 29864.334027309 * t);
            B0 += 0.00000006186 * Math.Cos(4.92498052443 + 19367.1891622328 * t);
            B0 += 0.00000006155 * Math.Cos(2.42413946900 + 4705.7323075436 * t);
            B0 += 0.00000005204 * Math.Cos(3.42528906628 + 9103.9069941176 * t);
            B0 += 0.00000006000 * Math.Cos(3.57639095526 + 3154.6870848956 * t);
            B0 += 0.00000004796 * Math.Cos(3.86676184909 + 7860.4193924392 * t);
            B0 += 0.00000005289 * Math.Cos(4.99182712443 + 7084.8967811152 * t);
            B0 += 0.00000004070 * Math.Cos(5.58798144740 + 12566.1516999828 * t);
            B0 += 0.00000003942 * Math.Cos(5.68758787835 + 10206.1719992102 * t);
            B0 += 0.00000003797 * Math.Cos(3.89520601076 + 10192.5101507186 * t);
            B0 += 0.00000003798 * Math.Cos(6.06410995916 + 10234.0609417034 * t);
            B0 += 0.00000003579 * Math.Cos(0.73789669235 + 4551.9534970588 * t);
            B0 += 0.00000003641 * Math.Cos(2.61501257205 + 15874.6175953632 * t);
            B0 += 0.00000003266 * Math.Cos(0.97517223854 + 23581.2581773176 * t);
            B0 += 0.00000002813 * Math.Cos(0.29951755546 + 9411.4646150872 * t);
            B0 += 0.00000003048 * Math.Cos(2.51085146990 + 33794.5437235286 * t);
            B0 += 0.00000002559 * Math.Cos(4.58043833032 + 801.8209311238 * t);
            B0 += 0.00000002462 * Math.Cos(5.05790874754 + 29050.7837433492 * t);
            B0 += 0.00000002593 * Math.Cos(5.73113176751 + 20213.271996984 * t);
            B0 += 0.00000002625 * Math.Cos(4.24272906574 + 213.299095438 * t);
            B0 += 0.00000002246 * Math.Cos(0.82112963936 + 28286.9904848612 * t);
            B0 += 0.00000002229 * Math.Cos(2.22457598233 + 10426.584641649 * t);
            B0 += 0.00000001742 * Math.Cos(1.48394229233 + 7058.5984613154 * t);
            B0 += 0.00000001660 * Math.Cos(5.42775825275 + 32217.2001810808 * t);
            B0 += 0.00000001491 * Math.Cos(4.64883377941 + 1109.3785520934 * t);
            B0 += 0.00000002010 * Math.Cos(0.75702888128 + 9999.986450773 * t);
            B0 += 0.00000001562 * Math.Cos(3.93962080463 + 37724.7534197482 * t);
            B0 += 0.00000001538 * Math.Cos(2.17309577080 + 21535.9496445154 * t);
            B0 += 0.00000001546 * Math.Cos(4.70759186462 + 14945.3161735544 * t);
            B0 += 0.00000001200 * Math.Cos(1.48282382657 + 9830.3890139878 * t);
            B0 += 0.00000001224 * Math.Cos(5.55090394449 + 5661.3320491522 * t);
            B0 += 0.00000001111 * Math.Cos(1.20276209213 + 9573.388259897 * t);
            B0 += 0.00000001064 * Math.Cos(1.98891375536 + 26.2983197998 * t);
            B0 += 0.00000001041 * Math.Cos(5.38535116069 + 7.1135470008 * t);
            B0 += 0.00000001036 * Math.Cos(1.16719443387 + 8662.240323563 * t);
            B0 += 0.00000001143 * Math.Cos(3.20596958337 + 3532.0606928114 * t);
            B0 += 0.00000001201 * Math.Cos(0.81913312536 + 8094.5216858326 * t);
            B0 += 0.00000001005 * Math.Cos(2.38429892132 + 27511.4678735372 * t);
            B0 += 0.00000001047 * Math.Cos(4.56525030769 + 20419.45754542119 * t);
            B0 += 0.00000000968 * Math.Cos(6.18496721871 + 25158.6017197654 * t);
            B0 += 0.00000001044 * Math.Cos(1.98055689074 + 10596.1820784342 * t);
            B0 += 0.00000000962 * Math.Cos(0.48573513747 + 23958.6317852334 * t);
            B0 += 0.00000000846 * Math.Cos(0.01566400887 + 3128.3887650958 * t);
            B0 += 0.00000000792 * Math.Cos(5.39686899735 + 24356.7807886416 * t);
            B0 += 0.00000000858 * Math.Cos(5.34692750735 + 41654.9631159678 * t);
            B0 += 0.00000000757 * Math.Cos(6.25904553773 + 20452.8694122218 * t);
            B0 += 0.00000000801 * Math.Cos(4.62406152514 + 9929.4262273458 * t);
            B0 += 0.00000000802 * Math.Cos(5.37234892520 + 10497.1448650762 * t);
            B0 += 0.00000000750 * Math.Cos(3.85219782842 + 21228.3920235458 * t);
            B0 += 0.00000000700 * Math.Cos(1.98097957188 + 3930.2096962196 * t);
            B0 += 0.00000000719 * Math.Cos(6.11596800207 + 10218.8084705184 * t);
            B0 += 0.00000000672 * Math.Cos(6.23429601219 + 14765.2390432698 * t);
            B0 += 0.00000000639 * Math.Cos(5.37566437358 + 1589.0728952838 * t);
            B0 += 0.00000000605 * Math.Cos(2.42330391120 + 10251.3132188468 * t);
            B0 += 0.00000000726 * Math.Cos(6.16683781802 + 18875.525869774 * t);
            B0 += 0.00000000613 * Math.Cos(5.99731180690 + 4732.0306273434 * t);
            B0 += 0.00000000720 * Math.Cos(3.84286345199 + 10207.7626219036 * t);
            B0 += 0.00000000637 * Math.Cos(6.17053891156 + 10220.3990932118 * t);
            B0 += 0.00000000515 * Math.Cos(1.03001478293 + 22779.4372461938 * t);
            B0 += 0.00000000574 * Math.Cos(0.43813688572 + 17085.9586657222 * t);
            B0 += 0.00000000510 * Math.Cos(1.41065159851 + 9161.0171630226 * t);
            B0 += 0.00000000569 * Math.Cos(3.34601425125 + 3340.6124266998 * t);
            B0 += 0.00000000608 * Math.Cos(1.25236241968 + 10175.2578735752 * t);
            B0 += 0.00000000524 * Math.Cos(2.39794248670 + 26087.9031415742 * t);
            B0 += 0.00000000542 * Math.Cos(1.34665646732 + 29088.811415985 * t);
            B0 += 0.00000000527 * Math.Cos(4.01994270827 + 18849.2275499742 * t);
            B0 += 0.00000000569 * Math.Cos(1.65498800378 + 39264.0692895602 * t);
            B0 += 0.00000000518 * Math.Cos(4.96996115446 + 30213.258447757 * t);
            B0 += 0.00000000514 * Math.Cos(5.78413007838 + 12592.4500197826 * t);
            B0 += 0.00000000538 * Math.Cos(4.56198493922 + 10063.7223490764 * t);
            B0 += 0.00000000484 * Math.Cos(4.18538027381 + 14919.0178537546 * t);
            B0 += 0.00000000493 * Math.Cos(4.79939382739 + 9146.790069021 * t);
            B0 += 0.00000000427 * Math.Cos(3.76876868949 + 11272.6674764002 * t);
            B0 += 0.00000000495 * Math.Cos(0.49175293655 + 45585.1728121874 * t);
            B0 += 0.00000000494 * Math.Cos(3.74345863918 + 31441.6775697568 * t);
            B0 += 0.00000000524 * Math.Cos(0.97991794166 + 30110.1656735384 * t);
            B0 += 0.00000000483 * Math.Cos(1.87898057316 + 51066.427731055 * t);
            B0 += 0.00000000505 * Math.Cos(3.70047474212 + 20400.2727726222 * t);
            B0 += 0.00000000351 * Math.Cos(4.34026574490 + 10137.0194749354 * t);
            B0 += 0.00000000355 * Math.Cos(5.56672554631 + 18837.49819713819 * t);
            B0 += 0.00000000328 * Math.Cos(3.78427378910 + 6681.2248533996 * t);
            B0 += 0.00000000349 * Math.Cos(4.20550749672 + 20956.2620575166 * t);
            B0 += 0.00000000333 * Math.Cos(4.44969281739 + 28521.0927782546 * t);
            B0 += 0.00000000296 * Math.Cos(2.83205515646 + 17277.4069318338 * t);
            B0 += 0.00000000311 * Math.Cos(2.57334132897 + 20809.4676246452 * t);
            B0 += 0.00000000294 * Math.Cos(0.75089224483 + 3149.1641605882 * t);
            B0 += 0.00000000377 * Math.Cos(3.98143308775 + 21202.093703746 * t);
            B0 += 0.00000000272 * Math.Cos(5.56183082489 + 16496.3613962024 * t);
            B0 += 0.00000000314 * Math.Cos(0.02584607093 + 13745.3462390224 * t);
            B0 += 0.00000000263 * Math.Cos(0.55328410985 + 36147.4098773004 * t);
            B0 += 0.00000000286 * Math.Cos(5.16408902215 + 426.598190876 * t);
            B0 += 0.00000000279 * Math.Cos(4.29871615943 + 19999.97290154599 * t);
            B0 += 0.00000000280 * Math.Cos(1.92925047377 + 49515.382508407 * t);
            B0 += 0.00000000265 * Math.Cos(4.81168402147 + 20235.1228263104 * t);
            B0 += 0.00000000273 * Math.Cos(5.12740051559 + 35371.8872659764 * t);
            B0 += 0.00000000306 * Math.Cos(5.28903794869 + 382.8965322232 * t);
            B0 += 0.00000000223 * Math.Cos(2.50591724074 + 26709.6469424134 * t);
            B0 += 0.00000000235 * Math.Cos(5.96522395118 + 10198.033075026 * t);
            B0 += 0.00000000234 * Math.Cos(3.52866583267 + 10228.538017396 * t);
            B0 += 0.00000000224 * Math.Cos(6.24561979789 + 7064.1213856228 * t);
            B0 += 0.00000000251 * Math.Cos(2.84739274245 + 33019.0211122046 * t);
            B0 += 0.00000000196 * Math.Cos(1.50610393790 + 31749.2351907264 * t);
            B0 += 0.00000000192 * Math.Cos(1.69321442572 + 13341.6743113068 * t);
            B0 += 0.00000000180 * Math.Cos(6.19353087076 + 39793.7602546548 * t);
            B0 += 0.00000000199 * Math.Cos(1.16433321880 + 22805.7355659936 * t);
            B0 += 0.00000000180 * Math.Cos(3.72646417141 + 1551.045222648 * t);
            B0 += 0.00000000173 * Math.Cos(3.35235705827 + 53445.5922046266 * t);
            B0 += 0.00000000195 * Math.Cos(1.51901264131 + 43232.3066584156 * t);
            B0 += 0.00000000174 * Math.Cos(2.84049662693 + 9967.4538999816 * t);
            B0 += 0.00000000163 * Math.Cos(4.29160537719 + 36949.2308084242 * t);
            B0 += 0.00000000169 * Math.Cos(0.37000676558 + 10459.1171924404 * t);
            B0 += 0.00000000137 * Math.Cos(5.61149803116 + 10529.6774158676 * t);
            B0 += 0.00000000139 * Math.Cos(0.87847805052 + 16522.6597160022 * t);
            B0 += 0.00000000139 * Math.Cos(4.12576475427 + 36301.18868778519 * t);
            B0 += 0.00000000127 * Math.Cos(5.14447758616 + 5481.2549188676 * t);
            B0 += 0.00000000131 * Math.Cos(3.11317801589 + 9896.8936765544 * t);
            B0 += 0.00000000131 * Math.Cos(0.89697384735 + 3442.5749449654 * t);
            B0 += 0.00000000121 * Math.Cos(1.32802112907 + 38734.3783244656 * t);
            B0 += 0.00000000122 * Math.Cos(1.59017183044 + 10110.1927719924 * t);
            B0 += 0.00000000123 * Math.Cos(2.33714216061 + 10316.3783204296 * t);
            B0 += 0.00000000133 * Math.Cos(2.90682399304 + 9793.8009023358 * t);
            B0 += 0.00000000111 * Math.Cos(2.52077634760 + 13936.794505134 * t);
            B0 += 0.00000000120 * Math.Cos(0.36076947165 + 536.8045120954 * t);
            B0 += 0.00000000115 * Math.Cos(2.53355582059 + 26735.9452622132 * t);
            B0 += 0.00000000108 * Math.Cos(2.65839634325 + 10232.95530711079 * t);
            B0 += 0.00000000108 * Math.Cos(0.55230439694 + 10193.61578531121 * t);
            B0 += 0.00000000138 * Math.Cos(1.06919239240 + 65236.2212932854 * t);
            B0 += 0.00000000101 * Math.Cos(3.17012502017 + 19317.1925403286 * t);
            B0 += 0.00000000127 * Math.Cos(5.63110477712 + 10288.0671447783 * t);
            B0 += 0.00000000127 * Math.Cos(3.86278127025 + 10138.5039476437 * t);
            B0 += 0.00000000137 * Math.Cos(2.93350659460 + 47162.5163546352 * t);
            B0 += 0.00000000095 * Math.Cos(5.03917884334 + 52175.8062831484 * t);
            B0 += 0.00000000094 * Math.Cos(0.71308489207 + 38500.2760310722 * t);
            B0 += 0.00000000092 * Math.Cos(5.46204624886 + 11764.330768859 * t);
            B0 += 0.00000000096 * Math.Cos(1.52914774412 + 9690.7081281172 * t);
            B0 += 0.00000000101 * Math.Cos(0.83318284426 + 6489.776587288 * t);
            B0 += 0.00000000115 * Math.Cos(3.76443612245 + 522.5774180938 * t);
            B0 += 0.00000000089 * Math.Cos(2.53312656681 + 10735.8629643048 * t);
            B0 += 0.00000000082 * Math.Cos(0.85628515615 + 2379.1644735716 * t);
            B0 += 0.00000000103 * Math.Cos(5.22683237620 + 103.0927742186 * t);
            B0 += 0.00000000090 * Math.Cos(2.12423586627 + 28313.288804661 * t);
            B0 += 0.00000000090 * Math.Cos(0.39668501735 + 9580.5018068978 * t);
            B0 += 0.00000000074 * Math.Cos(6.02680095550 + 3723.508958923 * t);
            B0 += 0.00000000081 * Math.Cos(5.25045057985 + 10419.4710946482 * t);
            B0 += 0.00000000080 * Math.Cos(4.23724598221 + 10007.0999977738 * t);
            B0 += 0.00000000091 * Math.Cos(2.48874147947 + 10846.0692855242 * t);
            B0 += 0.00000000085 * Math.Cos(3.82784790321 + 51868.2486621788 * t);
            B0 += 0.00000000081 * Math.Cos(2.26235214191 + 3903.9113764198 * t);
            B0 += 0.00000000097 * Math.Cos(0.77295091600 + 18307.8072320436 * t);
            B0 += 0.00000000094 * Math.Cos(0.17063414792 + 6872.6731195112 * t);
            B0 += 0.00000000080 * Math.Cos(5.62254102739 + 29999.959352319 * t);
            B0 += 0.00000000068 * Math.Cos(2.71762936670 + 16983.9961474566 * t);
            B0 += 0.00000000066 * Math.Cos(0.76731351736 + 20.7753954924 * t);
            B0 += 0.00000000075 * Math.Cos(0.36155638007 + 39302.096962196 * t);
            B0 += 0.00000000075 * Math.Cos(2.27327165974 + 8521.1198767086 * t);
            B0 += 0.00000000058 * Math.Cos(2.14482855875 + 8631.326197928 * t);
            B0 += 0.00000000064 * Math.Cos(5.83569051301 + 2118.7638603784 * t);
            B0 += 0.00000000058 * Math.Cos(2.98524209824 + 19889.76658032659 * t);
            B0 += 0.00000000054 * Math.Cos(1.78260668333 + 40077.61957352 * t);
            B0 += 0.00000000055 * Math.Cos(4.70485939861 + 639.897286314 * t);
            B0 += 0.00000000060 * Math.Cos(5.89661892920 + 41962.5207369374 * t);
            B0 += 0.00000000066 * Math.Cos(2.24746237999 + 74.7815985673 * t);
            B0 += 0.00000000061 * Math.Cos(3.40726181591 + 27490.6924780448 * t);
            B0 += 0.00000000051 * Math.Cos(3.07811180039 + 24150.080051345 * t);
            B0 += 0.00000000057 * Math.Cos(2.30081371235 + 20529.66386664059 * t);
            B0 += 0.00000000052 * Math.Cos(2.37192464233 + 29573.361161443 * t);
            B0 += 0.00000000052 * Math.Cos(4.76610409132 + 57375.8019008462 * t);
            B0 += 0.00000000047 * Math.Cos(1.61630288856 + 30831.3049047446 * t);
            B0 += 0.00000000054 * Math.Cos(5.89684197257 + 19903.99367432819 * t);
            B0 += 0.00000000040 * Math.Cos(5.32101847424 + 42430.4857272918 * t);
            B0 += 0.00000000051 * Math.Cos(5.29186795569 + 29587.5882554446 * t);
            return B0;
        }

        internal static double Venus_B1(double t) // 133 terms of order 1
        {
            double B1 = 0;
            B1 += 0.00513347602 * Math.Cos(1.80364310797 + 10213.285546211 * t);
            B1 += 0.00004380100 * Math.Cos(3.38615711591 + 20426.571092422 * t);
            B1 += 0.00000196586 * Math.Cos(2.53001197486 + 30639.856638633 * t);
            B1 += 0.00000199162;
            B1 += 0.00000014031 * Math.Cos(2.27087044687 + 9437.762934887 * t);
            B1 += 0.00000012958 * Math.Cos(1.50735622957 + 18073.7049386502 * t);
            B1 += 0.00000011941 * Math.Cos(5.60462450426 + 1577.3435424478 * t);
            B1 += 0.00000010324 * Math.Cos(5.24224313355 + 2352.8661537718 * t);
            B1 += 0.00000009294 * Math.Cos(6.07545631303 + 22003.9146348698 * t);
            B1 += 0.00000007441 * Math.Cos(1.50257909439 + 11790.6290886588 * t);
            B1 += 0.00000008031 * Math.Cos(0.29371105198 + 9153.9036160218 * t);
            B1 += 0.00000007514 * Math.Cos(5.08081885990 + 10186.9872264112 * t);
            B1 += 0.00000004669 * Math.Cos(3.87801635015 + 10239.5838660108 * t);
            B1 += 0.00000004399 * Math.Cos(3.58872736593 + 40853.142184844 * t);
            B1 += 0.00000003975 * Math.Cos(1.28397121206 + 10404.7338123226 * t);
            B1 += 0.00000004657 * Math.Cos(0.75073886819 + 5507.5532386674 * t);
            B1 += 0.00000003783 * Math.Cos(4.33004753984 + 19651.048481098 * t);
            B1 += 0.00000003390 * Math.Cos(4.88976070903 + 10988.808157535 * t);
            B1 += 0.00000003555 * Math.Cos(1.25927550356 + 19896.8801273274 * t);
            B1 += 0.00000003479 * Math.Cos(5.50797002160 + 529.6909650946 * t);
            B1 += 0.00000002884 * Math.Cos(0.08549582037 + 14143.4952424306 * t);
            B1 += 0.00000001786 * Math.Cos(0.37134513186 + 13367.9726311066 * t);
            B1 += 0.00000001600 * Math.Cos(1.68378002982 + 20618.0193585336 * t);
            B1 += 0.00000001539 * Math.Cos(1.21683853657 + 25934.1243310894 * t);
            B1 += 0.00000001341 * Math.Cos(2.90077139758 + 15720.8387848784 * t);
            B1 += 0.00000000993 * Math.Cos(1.74681248965 + 11322.6640983044 * t);
            B1 += 0.00000001165 * Math.Cos(6.13437155401 + 7860.4193924392 * t);
            B1 += 0.00000001115 * Math.Cos(0.66743690380 + 29580.4747084438 * t);
            B1 += 0.00000000923 * Math.Cos(2.25384969096 + 10021.8372800994 * t);
            B1 += 0.00000000965 * Math.Cos(1.36425494833 + 9683.5945811164 * t);
            B1 += 0.00000000973 * Math.Cos(0.39071758442 + 6283.0758499914 * t);
            B1 += 0.00000000805 * Math.Cos(0.53331923557 + 8624.2126509272 * t);
            B1 += 0.00000000913 * Math.Cos(0.76046003719 + 8635.9420037632 * t);
            B1 += 0.00000000991 * Math.Cos(0.55319879330 + 19367.1891622328 * t);
            B1 += 0.00000000609 * Math.Cos(2.62364470139 + 23581.2581773176 * t);
            B1 += 0.00000000532 * Math.Cos(5.10925676528 + 9786.687355335 * t);
            B1 += 0.00000000476 * Math.Cos(6.17672999981 + 11015.1064773348 * t);
            B1 += 0.00000000472 * Math.Cos(1.69672629200 + 17298.1823273262 * t);
            B1 += 0.00000000503 * Math.Cos(2.65840772485 + 29864.334027309 * t);
            B1 += 0.00000000456 * Math.Cos(5.01205315518 + 10742.9765113056 * t);
            B1 += 0.00000000478 * Math.Cos(3.94100005156 + 775.522611324 * t);
            B1 += 0.00000000477 * Math.Cos(3.71554345922 + 10596.1820784342 * t);
            B1 += 0.00000000347 * Math.Cos(2.34551062680 + 9411.4646150872 * t);
            B1 += 0.00000000458 * Math.Cos(2.31894399069 + 9999.986450773 * t);
            B1 += 0.00000000374 * Math.Cos(3.76878356974 + 21228.3920235458 * t);
            B1 += 0.00000000440 * Math.Cos(4.33400244581 + 15874.6175953632 * t);
            B1 += 0.00000000349 * Math.Cos(1.31468836511 + 10234.0609417034 * t);
            B1 += 0.00000000310 * Math.Cos(5.45422332781 + 10192.5101507186 * t);
            B1 += 0.00000000346 * Math.Cos(0.94242286364 + 1059.3819301892 * t);
            B1 += 0.00000000308 * Math.Cos(4.90145899142 + 3930.2096962196 * t);
            B1 += 0.00000000331 * Math.Cos(4.89498986674 + 10206.1719992102 * t);
            B1 += 0.00000000269 * Math.Cos(2.39650266204 + 801.8209311238 * t);
            B1 += 0.00000000269 * Math.Cos(0.00589873499 + 9830.3890139878 * t);
            B1 += 0.00000000261 * Math.Cos(3.48196147279 + 7058.5984613154 * t);
            B1 += 0.00000000290 * Math.Cos(0.10953964861 + 29050.7837433492 * t);
            B1 += 0.00000000283 * Math.Cos(6.12133736787 + 20419.45754542119 * t);
            B1 += 0.00000000232 * Math.Cos(3.07845850030 + 12566.1516999828 * t);
            B1 += 0.00000000265 * Math.Cos(4.02431894973 + 33794.5437235286 * t);
            B1 += 0.00000000220 * Math.Cos(2.37315851889 + 4551.9534970588 * t);
            B1 += 0.00000000247 * Math.Cos(3.07626728158 + 28286.9904848612 * t);
            B1 += 0.00000000202 * Math.Cos(3.56872121409 + 21535.9496445154 * t);
            B1 += 0.00000000225 * Math.Cos(5.76888896320 + 213.299095438 * t);
            B1 += 0.00000000217 * Math.Cos(0.88382111135 + 20213.271996984 * t);
            B1 += 0.00000000172 * Math.Cos(6.12653050186 + 9161.0171630226 * t);
            B1 += 0.00000000195 * Math.Cos(5.47240855400 + 37724.7534197482 * t);
            B1 += 0.00000000153 * Math.Cos(4.07656151671 + 27511.4678735372 * t);
            B1 += 0.00000000174 * Math.Cos(1.33676849359 + 32217.2001810808 * t);
            B1 += 0.00000000157 * Math.Cos(5.98474214437 + 26.2983197998 * t);
            B1 += 0.00000000163 * Math.Cos(5.45519134760 + 10426.584641649 * t);
            B1 += 0.00000000129 * Math.Cos(2.08748660996 + 3128.3887650958 * t);
            B1 += 0.00000000131 * Math.Cos(1.51959002513 + 10218.8084705184 * t);
            B1 += 0.00000000139 * Math.Cos(4.42330401713 + 10220.3990932118 * t);
            B1 += 0.00000000126 * Math.Cos(2.62296638037 + 22779.4372461938 * t);
            B1 += 0.00000000146 * Math.Cos(4.69869606856 + 25158.6017197654 * t);
            B1 += 0.00000000172 * Math.Cos(6.13435208788 + 18837.49819713819 * t);
            B1 += 0.00000000157 * Math.Cos(5.44507403858 + 4705.7323075436 * t);
            B1 += 0.00000000117 * Math.Cos(6.18296175153 + 20400.2727726222 * t);
            B1 += 0.00000000164 * Math.Cos(3.30849473132 + 51066.427731055 * t);
            B1 += 0.00000000113 * Math.Cos(3.64412860654 + 7.1135470008 * t);
            B1 += 0.00000000109 * Math.Cos(5.21220660788 + 8662.240323563 * t);
            B1 += 0.00000000133 * Math.Cos(1.78047296245 + 191.4482661116 * t);
            B1 += 0.00000000117 * Math.Cos(0.14681677884 + 9146.790069021 * t);
            B1 += 0.00000000116 * Math.Cos(0.61940521198 + 41654.9631159678 * t);
            B1 += 0.00000000096 * Math.Cos(1.49631428731 + 7084.8967811152 * t);
            B1 += 0.00000000096 * Math.Cos(1.21744230443 + 10198.033075026 * t);
            B1 += 0.00000000082 * Math.Cos(1.45863866349 + 10207.7626219036 * t);
            B1 += 0.00000000085 * Math.Cos(6.04057728058 + 21202.093703746 * t);
            B1 += 0.00000000083 * Math.Cos(0.19985600927 + 14919.0178537546 * t);
            B1 += 0.00000000077 * Math.Cos(5.50132310610 + 5661.3320491522 * t);
            B1 += 0.00000000077 * Math.Cos(2.00173927326 + 10228.538017396 * t);
            B1 += 0.00000000093 * Math.Cos(1.85466268819 + 45585.1728121874 * t);
            B1 += 0.00000000066 * Math.Cos(3.25826124156 + 1109.3785520934 * t);
            B1 += 0.00000000089 * Math.Cos(0.64100435648 + 3154.6870848956 * t);
            B1 += 0.00000000061 * Math.Cos(3.80043027736 + 11272.6674764002 * t);
            B1 += 0.00000000077 * Math.Cos(1.85516358950 + 3532.0606928114 * t);
            B1 += 0.00000000062 * Math.Cos(0.81341290651 + 382.8965322232 * t);
            B1 += 0.00000000072 * Math.Cos(2.35312965005 + 9103.9069941176 * t);
            B1 += 0.00000000053 * Math.Cos(3.21969389511 + 20452.8694122218 * t);
            B1 += 0.00000000067 * Math.Cos(1.42090542131 + 24356.7807886416 * t);
            B1 += 0.00000000056 * Math.Cos(2.97733070198 + 30110.1656735384 * t);
            B1 += 0.00000000051 * Math.Cos(4.22406663447 + 20809.4676246452 * t);
            B1 += 0.00000000058 * Math.Cos(6.20761936031 + 29088.811415985 * t);
            B1 += 0.00000000061 * Math.Cos(3.27309494322 + 49515.382508407 * t);
            B1 += 0.00000000046 * Math.Cos(5.49443476235 + 31441.6775697568 * t);
            B1 += 0.00000000050 * Math.Cos(4.16651052942 + 13341.6743113068 * t);
            B1 += 0.00000000047 * Math.Cos(1.25473247769 + 33019.0211122046 * t);
            B1 += 0.00000000047 * Math.Cos(2.03402044389 + 23958.6317852334 * t);
            B1 += 0.00000000036 * Math.Cos(5.24409311105 + 3149.1641605882 * t);
            B1 += 0.00000000038 * Math.Cos(4.15337829669 + 18849.2275499742 * t);
            B1 += 0.00000000042 * Math.Cos(0.43005959574 + 1589.0728952838 * t);
            B1 += 0.00000000041 * Math.Cos(1.21289342964 + 12592.4500197826 * t);
            B1 += 0.00000000038 * Math.Cos(5.91928287144 + 28521.0927782546 * t);
            B1 += 0.00000000033 * Math.Cos(3.98241699279 + 4732.0306273434 * t);
            B1 += 0.00000000035 * Math.Cos(2.24417218267 + 16496.3613962024 * t);
            B1 += 0.00000000040 * Math.Cos(6.13293942728 + 26087.9031415742 * t);
            B1 += 0.00000000044 * Math.Cos(1.78123294860 + 426.598190876 * t);
            B1 += 0.00000000041 * Math.Cos(3.16744909855 + 39264.0692895602 * t);
            B1 += 0.00000000033 * Math.Cos(4.96183427323 + 536.8045120954 * t);
            B1 += 0.00000000034 * Math.Cos(0.12963030501 + 30213.258447757 * t);
            B1 += 0.00000000036 * Math.Cos(5.41167321573 + 522.5774180938 * t);
            B1 += 0.00000000027 * Math.Cos(4.44250239485 + 17277.4069318338 * t);
            B1 += 0.00000000034 * Math.Cos(5.94541303751 + 9929.4262273458 * t);
            B1 += 0.00000000033 * Math.Cos(0.40689057274 + 10497.1448650762 * t);
            B1 += 0.00000000023 * Math.Cos(2.59067946967 + 10175.2578735752 * t);
            B1 += 0.00000000022 * Math.Cos(0.69625017371 + 19999.97290154599 * t);
            B1 += 0.00000000023 * Math.Cos(3.76162101633 + 10251.3132188468 * t);
            B1 += 0.00000000023 * Math.Cos(0.62711494266 + 35371.8872659764 * t);
            B1 += 0.00000000022 * Math.Cos(4.64142978776 + 19889.76658032659 * t);
            B1 += 0.00000000020 * Math.Cos(4.01315480107 + 26709.6469424134 * t);
            B1 += 0.00000000020 * Math.Cos(4.03344400680 + 29573.361161443 * t);
            B1 += 0.00000000023 * Math.Cos(0.90416640595 + 8094.5216858326 * t);
            B1 += 0.00000000022 * Math.Cos(1.92092469688 + 17085.9586657222 * t);
            B1 += 0.00000000019 * Math.Cos(5.04938942644 + 6681.2248533996 * t);
            return B1 * t;
        }

        internal static double Venus_B2(double t) // 59 terms of order 2
        {
            double B2 = 0;
            B2 += 0.00022377665 * Math.Cos(3.38509143877 + 10213.285546211 * t);
            B2 += 0.00000281739;
            B2 += 0.00000173164 * Math.Cos(5.25563766915 + 20426.571092422 * t);
            B2 += 0.00000026945 * Math.Cos(3.87040891568 + 30639.856638633 * t);
            B2 += 0.00000001174 * Math.Cos(0.09768632072 + 10186.9872264112 * t);
            B2 += 0.00000000685 * Math.Cos(3.19139067811 + 11790.6290886588 * t);
            B2 += 0.00000000788 * Math.Cos(4.36515965295 + 10239.5838660108 * t);
            B2 += 0.00000000592 * Math.Cos(5.22270440328 + 40853.142184844 * t);
            B2 += 0.00000000515 * Math.Cos(6.12821215207 + 10988.808157535 * t);
            B2 += 0.00000000538 * Math.Cos(0.57550272342 + 2352.8661537718 * t);
            B2 += 0.00000000540 * Math.Cos(3.11657836329 + 18073.7049386502 * t);
            B2 += 0.00000000454 * Math.Cos(2.79306867629 + 10404.7338123226 * t);
            B2 += 0.00000000374 * Math.Cos(6.10468482446 + 9437.762934887 * t);
            B2 += 0.00000000431 * Math.Cos(4.00778431184 + 1577.3435424478 * t);
            B2 += 0.00000000360 * Math.Cos(6.01747842320 + 19651.048481098 * t);
            B2 += 0.00000000375 * Math.Cos(1.31319959789 + 22003.9146348698 * t);
            B2 += 0.00000000354 * Math.Cos(5.12509281266 + 9153.9036160218 * t);
            B2 += 0.00000000150 * Math.Cos(4.58623687118 + 15720.8387848784 * t);
            B2 += 0.00000000164 * Math.Cos(5.41790158607 + 5507.5532386674 * t);
            B2 += 0.00000000159 * Math.Cos(2.78191550878 + 19896.8801273274 * t);
            B2 += 0.00000000157 * Math.Cos(0.65774905071 + 529.6909650946 * t);
            B2 += 0.00000000155 * Math.Cos(2.54824315372 + 9683.5945811164 * t);
            B2 += 0.00000000109 * Math.Cos(2.01866665583 + 14143.4952424306 * t);
            B2 += 0.00000000106 * Math.Cos(2.28289033017 + 6283.0758499914 * t);
            B2 += 0.00000000115 * Math.Cos(3.23636374193 + 20618.0193585336 * t);
            B2 += 0.00000000128 * Math.Cos(5.32400510939 + 13367.9726311066 * t);
            B2 += 0.00000000087 * Math.Cos(3.28265082435 + 11322.6640983044 * t);
            B2 += 0.00000000090 * Math.Cos(5.23585072275 + 10596.1820784342 * t);
            B2 += 0.00000000055 * Math.Cos(4.82369879741 + 7058.5984613154 * t);
            B2 += 0.00000000044 * Math.Cos(0.58444963462 + 10206.1719992102 * t);
            B2 += 0.00000000044 * Math.Cos(2.34401612969 + 19367.1891622328 * t);
            B2 += 0.00000000038 * Math.Cos(4.55053233088 + 9999.986450773 * t);
            B2 += 0.00000000039 * Math.Cos(5.84340580032 + 10220.3990932118 * t);
            B2 += 0.00000000036 * Math.Cos(4.41006216127 + 51066.427731055 * t);
            B2 += 0.00000000039 * Math.Cos(3.14348236386 + 9411.4646150872 * t);
            B2 += 0.00000000033 * Math.Cos(4.55748660340 + 10742.9765113056 * t);
            B2 += 0.00000000037 * Math.Cos(2.79630938717 + 25934.1243310894 * t);
            B2 += 0.00000000034 * Math.Cos(0.55287110072 + 11015.1064773348 * t);
            B2 += 0.00000000034 * Math.Cos(2.25809144959 + 29580.4747084438 * t);
            B2 += 0.00000000038 * Math.Cos(1.88638747393 + 801.8209311238 * t);
            B2 += 0.00000000034 * Math.Cos(1.22706917271 + 10021.8372800994 * t);
            B2 += 0.00000000027 * Math.Cos(4.83867137637 + 9830.3890139878 * t);
            B2 += 0.00000000027 * Math.Cos(4.31140179350 + 23581.2581773176 * t);
            B2 += 0.00000000027 * Math.Cos(2.17187621336 + 8635.9420037632 * t);
            B2 += 0.00000000020 * Math.Cos(5.66581696952 + 21228.3920235458 * t);
            B2 += 0.00000000024 * Math.Cos(2.17208107850 + 18849.2275499742 * t);
            B2 += 0.00000000020 * Math.Cos(5.29318634138 + 775.522611324 * t);
            B2 += 0.00000000019 * Math.Cos(2.73486845601 + 3128.3887650958 * t);
            B2 += 0.00000000013 * Math.Cos(3.40362915274 + 1059.3819301892 * t);
            B2 += 0.00000000014 * Math.Cos(0.05074160195 + 7860.4193924392 * t);
            B2 += 0.00000000014 * Math.Cos(5.43035907265 + 26.2983197998 * t);
            B2 += 0.00000000012 * Math.Cos(3.24834347355 + 9103.9069941176 * t);
            B2 += 0.00000000013 * Math.Cos(5.04826725887 + 7.1135470008 * t);
            B2 += 0.00000000015 * Math.Cos(1.42027402522 + 29050.7837433492 * t);
            B2 += 0.00000000010 * Math.Cos(4.98138067490 + 10426.584641649 * t);
            B2 += 0.00000000011 * Math.Cos(0.85773045784 + 17298.1823273262 * t);
            B2 += 0.00000000011 * Math.Cos(4.23048200054 + 29864.334027309 * t);
            B2 += 0.00000000010 * Math.Cos(0.26447399758 + 3930.2096962196 * t);
            B2 += 0.00000000011 * Math.Cos(1.46728576671 + 20419.45754542119 * t);
            return B2 * t * t;
        }

        internal static double Venus_B3(double t) // 15 terms of order 3
        {
            double B3 = 0;
            B3 += 0.00000646671 * Math.Cos(4.99166565277 + 10213.285546211 * t);
            B3 -= 0.00000019952;
            B3 += 0.00000005540 * Math.Cos(0.77376923951 + 20426.571092422 * t);
            B3 += 0.00000002526 * Math.Cos(5.44493763020 + 30639.856638633 * t);
            B3 += 0.00000000079 * Math.Cos(1.51447613604 + 10186.9872264112 * t);
            B3 += 0.00000000056 * Math.Cos(0.63647808442 + 40853.142184844 * t);
            B3 += 0.00000000058 * Math.Cos(5.70731176550 + 10239.5838660108 * t);
            B3 += 0.00000000031 * Math.Cos(4.72523061067 + 11790.6290886588 * t);
            B3 += 0.00000000026 * Math.Cos(1.02068113372 + 10988.808157535 * t);
            B3 += 0.00000000025 * Math.Cos(5.60599130442 + 9437.762934887 * t);
            B3 += 0.00000000017 * Math.Cos(2.05293621864 + 2352.8661537718 * t);
            B3 += 0.00000000011 * Math.Cos(4.33056892256 + 10404.7338123226 * t);
            B3 += 0.00000000009 * Math.Cos(1.36283915068 + 19651.048481098 * t);
            B3 += 0.00000000007 * Math.Cos(4.69592781899 + 18073.7049386502 * t);
            B3 += 0.00000000006 * Math.Cos(2.97926526705 + 22003.9146348698 * t);
            return B3 * t * t * t;
        }

        internal static double Venus_B4(double t) // 5 terms of order 4
        {
            double B4 = 0;
            B4 += 0.00000014102 * Math.Cos(0.31537190181 + 10213.285546211 * t);
            B4 += 0.00000000190 * Math.Cos(2.35466404492 + 20426.571092422 * t);
            B4 += 0.00000000164 * Math.Cos(0.74476215141 + 30639.856638633 * t);
            B4 -= 0.00000000214;
            B4 += 0.00000000004 * Math.Cos(2.34190883009 + 40853.142184844 * t);
            return B4 * t * t * t * t;
        }

        internal static double Venus_B5(double t) // 4 terms of order 5
        {
            double B5 = 0;
            B5 += 0.00000000239 * Math.Cos(2.05201727566 + 10213.285546211 * t);
            B5 += 0.00000000039;
            B5 += 0.00000000011 * Math.Cos(3.82500275251 + 20426.571092422 * t);
            B5 += 0.00000000009 * Math.Cos(2.32953116868 + 30639.856638633 * t);
            return B5 * t * t * t * t * t;
        }

        internal static double Venus_R0(double t) // 330 terms of order 0
        {
            double R0 = 0;
            R0 += 0.72334820905;
            R0 += 0.00489824185 * Math.Cos(4.02151832268 + 10213.285546211 * t);
            R0 += 0.00001658058 * Math.Cos(4.90206728012 + 20426.571092422 * t);
            R0 += 0.00001632093 * Math.Cos(2.84548851892 + 7860.4193924392 * t);
            R0 += 0.00001378048 * Math.Cos(1.12846590600 + 11790.6290886588 * t);
            R0 += 0.00000498399 * Math.Cos(2.58682187717 + 9683.5945811164 * t);
            R0 += 0.00000373958 * Math.Cos(1.42314837063 + 3930.2096962196 * t);
            R0 += 0.00000263616 * Math.Cos(5.52938185920 + 9437.762934887 * t);
            R0 += 0.00000237455 * Math.Cos(2.55135903978 + 15720.8387848784 * t);
            R0 += 0.00000221983 * Math.Cos(2.01346776772 + 19367.1891622328 * t);
            R0 += 0.00000119467 * Math.Cos(3.01975365264 + 10404.7338123226 * t);
            R0 += 0.00000125896 * Math.Cos(2.72769833559 + 1577.3435424478 * t);
            R0 += 0.00000076178 * Math.Cos(1.59577224486 + 9153.9036160218 * t);
            R0 += 0.00000085336 * Math.Cos(3.98607953754 + 19651.048481098 * t);
            R0 += 0.00000074347 * Math.Cos(4.11957854039 + 5507.5532386674 * t);
            R0 += 0.00000041904 * Math.Cos(1.64273363458 + 18837.49819713819 * t);
            R0 += 0.00000042493 * Math.Cos(3.81864530735 + 13367.9726311066 * t);
            R0 += 0.00000039430 * Math.Cos(5.39019422358 + 23581.2581773176 * t);
            R0 += 0.00000029042 * Math.Cos(5.67739528728 + 5661.3320491522 * t);
            R0 += 0.00000027555 * Math.Cos(5.72392407794 + 775.522611324 * t);
            R0 += 0.00000027283 * Math.Cos(4.82151812709 + 11015.1064773348 * t);
            R0 += 0.00000031274 * Math.Cos(2.31806719544 + 9999.986450773 * t);
            R0 += 0.00000019700 * Math.Cos(4.96157560245 + 11322.6640983044 * t);
            R0 += 0.00000019809 * Math.Cos(0.53189326492 + 27511.4678735372 * t);
            R0 += 0.00000013567 * Math.Cos(3.75530870628 + 18073.7049386502 * t);
            R0 += 0.00000012921 * Math.Cos(1.13381083556 + 10206.1719992102 * t);
            R0 += 0.00000016215 * Math.Cos(0.56453834290 + 529.6909650946 * t);
            R0 += 0.00000011821 * Math.Cos(5.09025877427 + 3154.6870848956 * t);
            R0 += 0.00000011728 * Math.Cos(0.23432298744 + 7084.8967811152 * t);
            R0 += 0.00000013079 * Math.Cos(5.24353197586 + 17298.1823273262 * t);
            R0 += 0.00000013180 * Math.Cos(3.37207825651 + 13745.3462390224 * t);
            R0 += 0.00000009097 * Math.Cos(3.07004895769 + 1109.3785520934 * t);
            R0 += 0.00000010818 * Math.Cos(2.45024712908 + 10239.5838660108 * t);
            R0 += 0.00000011438 * Math.Cos(4.56838894696 + 29050.7837433492 * t);
            R0 += 0.00000008377 * Math.Cos(5.78327612352 + 30639.856638633 * t);
            R0 += 0.00000008193 * Math.Cos(1.95023111860 + 22003.9146348698 * t);
            R0 += 0.00000009308 * Math.Cos(1.61615909286 + 2352.8661537718 * t);
            R0 += 0.00000010652 * Math.Cos(1.95528396140 + 31441.6775697568 * t);
            R0 += 0.00000010357 * Math.Cos(1.20234990061 + 15874.6175953632 * t);
            R0 += 0.00000009585 * Math.Cos(1.46639856228 + 19999.97290154599 * t);
            R0 += 0.00000006506 * Math.Cos(2.17390732263 + 14143.4952424306 * t);
            R0 += 0.00000007562 * Math.Cos(1.13789564977 + 8624.2126509272 * t);
            R0 += 0.00000006434 * Math.Cos(0.84419623033 + 6283.0758499914 * t);
            R0 += 0.00000005898 * Math.Cos(0.01093731110 + 8635.9420037632 * t);
            R0 += 0.00000005632 * Math.Cos(3.94956548631 + 12566.1516999828 * t);
            R0 += 0.00000005523 * Math.Cos(1.27394296557 + 18307.8072320436 * t);
            R0 += 0.00000004488 * Math.Cos(2.47835729057 + 191.4482661116 * t);
            R0 += 0.00000004529 * Math.Cos(4.73027770400 + 19896.8801273274 * t);
            R0 += 0.00000006193 * Math.Cos(3.25881250939 + 6872.6731195112 * t);
            R0 += 0.00000006070 * Math.Cos(0.35337419942 + 21228.3920235458 * t);
            R0 += 0.00000004315 * Math.Cos(2.59737099519 + 4551.9534970588 * t);
            R0 += 0.00000006005 * Math.Cos(3.37874723475 + 35371.8872659764 * t);
            R0 += 0.00000003852 * Math.Cos(1.01162850357 + 9786.687355335 * t);
            R0 += 0.00000004033 * Math.Cos(0.00050855580 + 801.8209311238 * t);
            R0 += 0.00000003920 * Math.Cos(5.56542869407 + 10596.1820784342 * t);
            R0 += 0.00000002709 * Math.Cos(5.80195530112 + 7064.1213856228 * t);
            R0 += 0.00000003216 * Math.Cos(0.39767254848 + 10186.9872264112 * t);
            R0 += 0.00000003089 * Math.Cos(6.26174762876 + 14945.3161735544 * t);
            R0 += 0.00000002982 * Math.Cos(4.21196716354 + 28521.0927782546 * t);
            R0 += 0.00000003284 * Math.Cos(0.70709821006 + 10742.9765113056 * t);
            R0 += 0.00000003484 * Math.Cos(4.79878191875 + 39302.096962196 * t);
            R0 += 0.00000003172 * Math.Cos(1.80518954174 + 25158.6017197654 * t);
            R0 += 0.00000002463 * Math.Cos(0.68708153678 + 10988.808157535 * t);
            R0 += 0.00000002374 * Math.Cos(3.77948685343 + 21535.9496445154 * t);
            R0 += 0.00000002198 * Math.Cos(2.82996372521 + 8662.240323563 * t);
            R0 += 0.00000001958 * Math.Cos(5.41763804167 + 16496.3613962024 * t);
            R0 += 0.00000001876 * Math.Cos(2.63426768393 + 29580.4747084438 * t);
            R0 += 0.00000001902 * Math.Cos(2.85782199133 + 3532.0606928114 * t);
            R0 += 0.00000001706 * Math.Cos(3.67573010379 + 26.2983197998 * t);
            R0 += 0.00000001817 * Math.Cos(0.41611036449 + 4705.7323075436 * t);
            R0 += 0.00000001858 * Math.Cos(1.50368318296 + 10021.8372800994 * t);
            R0 += 0.00000002087 * Math.Cos(6.22112874639 + 43232.3066584156 * t);
            R0 += 0.00000001950 * Math.Cos(2.21447019683 + 19786.67380610799 * t);
            R0 += 0.00000001497 * Math.Cos(0.00134773824 + 17277.4069318338 * t);
            R0 += 0.00000001819 * Math.Cos(3.23144993268 + 29088.811415985 * t);
            R0 += 0.00000001423 * Math.Cos(5.85979618707 + 9676.4810341156 * t);
            R0 += 0.00000001223 * Math.Cos(5.55818994329 + 6770.7106012456 * t);
            R0 += 0.00000001140 * Math.Cos(5.92088900094 + 13936.794505134 * t);
            R0 += 0.00000001484 * Math.Cos(2.47665429253 + 31749.2351907264 * t);
            R0 += 0.00000001185 * Math.Cos(1.42087628351 + 4732.0306273434 * t);
            R0 += 0.00000001323 * Math.Cos(2.48821075422 + 9690.7081281172 * t);
            R0 += 0.00000001249 * Math.Cos(1.88323673734 + 19374.3027092336 * t);
            R0 += 0.00000001270 * Math.Cos(5.24647873116 + 19360.07561523199 * t);
            R0 += 0.00000001402 * Math.Cos(5.17536780118 + 10316.3783204296 * t);
            R0 += 0.00000001042 * Math.Cos(3.05454698508 + 25934.1243310894 * t);
            R0 += 0.00000001174 * Math.Cos(1.42913732999 + 18875.525869774 * t);
            R0 += 0.00000001278 * Math.Cos(1.35747287297 + 47162.5163546352 * t);
            R0 += 0.00000000917 * Math.Cos(6.26337648765 + 20618.0193585336 * t);
            R0 += 0.00000000905 * Math.Cos(1.12740203561 + 12592.4500197826 * t);
            R0 += 0.00000001093 * Math.Cos(4.64451720605 + 33019.0211122046 * t);
            R0 += 0.00000001014 * Math.Cos(1.09259406433 + 1059.3819301892 * t);
            R0 += 0.00000000783 * Math.Cos(2.02118183873 + 24356.7807886416 * t);
            R0 += 0.00000000779 * Math.Cos(0.41585274010 + 3340.6124266998 * t);
            R0 += 0.00000000700 * Math.Cos(1.14936815714 + 16983.9961474566 * t);
            R0 += 0.00000000878 * Math.Cos(0.87852464964 + 38734.3783244656 * t);
            R0 += 0.00000000623 * Math.Cos(0.89976912165 + 17778.11626694899 * t);
            R0 += 0.00000000608 * Math.Cos(1.58476225197 + 9573.388259897 * t);
            R0 += 0.00000000800 * Math.Cos(3.94213003073 + 10138.5039476437 * t);
            R0 += 0.00000000760 * Math.Cos(1.31851313748 + 9967.4538999816 * t);
            R0 += 0.00000000802 * Math.Cos(2.78173370208 + 51092.7260508548 * t);
            R0 += 0.00000000664 * Math.Cos(4.45864682400 + 3128.3887650958 * t);
            R0 += 0.00000000674 * Math.Cos(5.11214939998 + 382.8965322232 * t);
            R0 += 0.00000000530 * Math.Cos(0.85392938403 + 10234.0609417034 * t);
            R0 += 0.00000000509 * Math.Cos(3.56809374595 + 28286.9904848612 * t);
            R0 += 0.00000000600 * Math.Cos(4.25927726907 + 41962.5207369374 * t);
            R0 += 0.00000000601 * Math.Cos(5.78144137895 + 213.299095438 * t);
            R0 += 0.00000000595 * Math.Cos(2.83045104588 + 22805.7355659936 * t);
            R0 += 0.00000000673 * Math.Cos(6.06079908421 + 36949.2308084242 * t);
            R0 += 0.00000000535 * Math.Cos(5.85422519711 + 9103.9069941176 * t);
            R0 += 0.00000000544 * Math.Cos(5.44806074800 + 3723.508958923 * t);
            R0 += 0.00000000492 * Math.Cos(3.83802404893 + 27991.40181316 * t);
            R0 += 0.00000000635 * Math.Cos(0.76494024849 + 8094.5216858326 * t);
            R0 += 0.00000000434 * Math.Cos(6.22214487735 + 27197.2816936676 * t);
            R0 += 0.00000000459 * Math.Cos(3.55062885479 + 20213.271996984 * t);
            R0 += 0.00000000398 * Math.Cos(6.16269975784 + 10426.584641649 * t);
            R0 += 0.00000000378 * Math.Cos(2.41665947591 + 18844.61174413899 * t);
            R0 += 0.00000000421 * Math.Cos(4.86552697954 + 9146.790069021 * t);
            R0 += 0.00000000500 * Math.Cos(4.20351458644 + 55022.9357470744 * t);
            R0 += 0.00000000404 * Math.Cos(4.95834410782 + 37410.5672398786 * t);
            R0 += 0.00000000402 * Math.Cos(2.97963246945 + 10220.3990932118 * t);
            R0 += 0.00000000464 * Math.Cos(2.59869499733 + 18734.4054229196 * t);
            R0 += 0.00000000352 * Math.Cos(0.08963076359 + 10103.0792249916 * t);
            R0 += 0.00000000348 * Math.Cos(4.90260339364 + 18830.38465013739 * t);
            R0 += 0.00000000338 * Math.Cos(3.22520096478 + 24150.080051345 * t);
            R0 += 0.00000000375 * Math.Cos(6.17532088136 + 26087.9031415742 * t);
            R0 += 0.00000000425 * Math.Cos(1.20052578280 + 40879.4405046438 * t);
            R0 += 0.00000000408 * Math.Cos(3.12833060705 + 9050.8108418032 * t);
            R0 += 0.00000000385 * Math.Cos(1.94284690176 + 283.8593188652 * t);
            R0 += 0.00000000337 * Math.Cos(4.87838699272 + 12432.0426503978 * t);
            R0 += 0.00000000326 * Math.Cos(4.27369741426 + 26735.9452622132 * t);
            R0 += 0.00000000309 * Math.Cos(0.50597475053 + 38204.687359371 * t);
            R0 += 0.00000000329 * Math.Cos(3.88430599153 + 29864.334027309 * t);
            R0 += 0.00000000313 * Math.Cos(1.36138752543 + 10192.5101507186 * t);
            R0 += 0.00000000347 * Math.Cos(3.58439807209 + 27490.6924780448 * t);
            R0 += 0.00000000251 * Math.Cos(3.78618457047 + 10063.7223490764 * t);
            R0 += 0.00000000244 * Math.Cos(3.83523342668 + 9411.4646150872 * t);
            R0 += 0.00000000281 * Math.Cos(4.50895206233 + 32217.2001810808 * t);
            R0 += 0.00000000237 * Math.Cos(0.87748812245 + 6681.2248533996 * t);
            R0 += 0.00000000315 * Math.Cos(5.62657778233 + 58953.145443294 * t);
            R0 += 0.00000000311 * Math.Cos(4.15626121491 + 10175.1525105732 * t);
            R0 += 0.00000000247 * Math.Cos(2.53637594113 + 16522.6597160022 * t);
            R0 += 0.00000000219 * Math.Cos(5.08729383251 + 7058.5984613154 * t);
            R0 += 0.00000000291 * Math.Cos(3.72567217056 + 29999.959352319 * t);
            R0 += 0.00000000267 * Math.Cos(2.97685503991 + 19573.37471066999 * t);
            R0 += 0.00000000280 * Math.Cos(3.70200084294 + 47623.8527860896 * t);
            R0 += 0.00000000239 * Math.Cos(3.94545782067 + 9580.5018068978 * t);
            R0 += 0.00000000246 * Math.Cos(2.18244883930 + 9161.0171630226 * t);
            R0 += 0.00000000253 * Math.Cos(2.69506547016 + 3442.5749449654 * t);
            R0 += 0.00000000265 * Math.Cos(2.62811801237 + 44809.6502008634 * t);
            R0 += 0.00000000194 * Math.Cos(4.78926136175 + 33794.5437235286 * t);
            R0 += 0.00000000187 * Math.Cos(3.65620881095 + 20452.8694122218 * t);
            R0 += 0.00000000224 * Math.Cos(2.43601863127 + 9992.8729037722 * t);
            R0 += 0.00000000193 * Math.Cos(2.55112161845 + 2379.1644735716 * t);
            R0 += 0.00000000201 * Math.Cos(1.90356905733 + 1551.045222648 * t);
            R0 += 0.00000000176 * Math.Cos(4.29837616553 + 10137.0194749354 * t);
            R0 += 0.00000000184 * Math.Cos(6.16061560223 + 36147.4098773004 * t);
            R0 += 0.00000000175 * Math.Cos(2.71984797040 + 20809.4676246452 * t);
            R0 += 0.00000000186 * Math.Cos(2.55098927966 + 14919.0178537546 * t);
            R0 += 0.00000000161 * Math.Cos(4.13272567123 + 23958.6317852334 * t);
            R0 += 0.00000000221 * Math.Cos(4.83552377614 + 20277.0078952874 * t);
            R0 += 0.00000000160 * Math.Cos(1.81472642729 + 10787.6303445458 * t);
            R0 += 0.00000000199 * Math.Cos(5.74259798330 + 30666.1549584328 * t);
            R0 += 0.00000000160 * Math.Cos(4.46270605493 + 18947.7045183576 * t);
            R0 += 0.00000000187 * Math.Cos(2.98688597588 + 2218.7571041868 * t);
            R0 += 0.00000000189 * Math.Cos(5.34607810282 + 10007.0999977738 * t);
            R0 += 0.00000000198 * Math.Cos(0.77846666692 + 62883.3551395136 * t);
            R0 += 0.00000000144 * Math.Cos(5.00261963924 + 9264.1099372412 * t);
            R0 += 0.00000000171 * Math.Cos(2.05212624568 + 7255.5696517344 * t);
            R0 += 0.00000000188 * Math.Cos(4.08173534559 + 48739.859897083 * t);
            R0 += 0.00000000146 * Math.Cos(3.94191715702 + 6309.3741697912 * t);
            R0 += 0.00000000146 * Math.Cos(5.06313558118 + 39264.0692895602 * t);
            R0 += 0.00000000135 * Math.Cos(5.93689169614 + 37724.7534197482 * t);
            R0 += 0.00000000139 * Math.Cos(2.81266025896 + 20.7753954924 * t);
            R0 += 0.00000000177 * Math.Cos(5.16224804657 + 9835.9119382952 * t);
            R0 += 0.00000000119 * Math.Cos(1.37254262864 + 40077.61957352 * t);
            R0 += 0.00000000120 * Math.Cos(0.21443767468 + 31022.7531708562 * t);
            R0 += 0.00000000128 * Math.Cos(2.92458887798 + 7.1135470008 * t);
            R0 += 0.00000000150 * Math.Cos(5.73646272556 + 632.7837393132 * t);
            R0 += 0.00000000106 * Math.Cos(0.62224833817 + 11272.6674764002 * t);
            R0 += 0.00000000114 * Math.Cos(2.63301326520 + 17468.8551979454 * t);
            R0 += 0.00000000123 * Math.Cos(6.22518843711 + 53285.1848352418 * t);
            R0 += 0.00000000107 * Math.Cos(1.17258978900 + 43071.8992890308 * t);
            R0 += 0.00000000103 * Math.Cos(1.09613781581 + 41654.9631159678 * t);
            R0 += 0.00000000109 * Math.Cos(2.01412667085 + 20419.45754542119 * t);
            R0 += 0.00000000102 * Math.Cos(4.23406964348 + 10251.3132188468 * t);
            R0 += 0.00000000116 * Math.Cos(1.27731728606 + 10199.0584522094 * t);
            R0 += 0.00000000103 * Math.Cos(5.25887538465 + 9830.3890139878 * t);
            R0 += 0.00000000112 * Math.Cos(2.24436894064 + 18204.71445782499 * t);
            R0 += 0.00000000111 * Math.Cos(2.23547857955 + 8521.1198767086 * t);
            R0 += 0.00000000118 * Math.Cos(0.23754207200 + 10497.1448650762 * t);
            R0 += 0.00000000123 * Math.Cos(0.88054816668 + 34596.3646546524 * t);
            R0 += 0.00000000102 * Math.Cos(4.39438646620 + 18300.69368504279 * t);
            R0 += 0.00000000131 * Math.Cos(6.01711652115 + 9367.2027114598 * t);
            R0 += 0.00000000100 * Math.Cos(5.00532389609 + 10175.2578735752 * t);
            R0 += 0.00000000107 * Math.Cos(0.41270197502 + 40853.142184844 * t);
            R0 += 0.00000000132 * Math.Cos(5.45008342761 + 11506.7697697936 * t);
            R0 += 0.00000000098 * Math.Cos(1.07722950958 + 13553.8979729108 * t);
            R0 += 0.00000000094 * Math.Cos(2.91720097590 + 44007.8292697396 * t);
            R0 += 0.00000000097 * Math.Cos(1.04004223634 + 68050.42387851159 * t);
            R0 += 0.00000000127 * Math.Cos(2.20215372683 + 66813.5648357332 * t);
            R0 += 0.00000000111 * Math.Cos(1.57823839032 + 29043.67019634839 * t);
            R0 += 0.00000000118 * Math.Cos(2.33268176890 + 18314.9207790444 * t);
            R0 += 0.00000000090 * Math.Cos(2.42353056125 + 32858.61374281979 * t);
            R0 += 0.00000000109 * Math.Cos(3.82796787296 + 19470.28193645139 * t);
            R0 += 0.00000000111 * Math.Cos(4.47666957576 + 29057.89729034999 * t);
            R0 += 0.00000000101 * Math.Cos(3.41528493660 + 19264.0963880142 * t);
            R0 += 0.00000000092 * Math.Cos(3.66289799512 + 22645.32819660879 * t);
            R0 += 0.00000000094 * Math.Cos(6.07530805791 + 10846.0692855242 * t);
            R0 += 0.00000000114 * Math.Cos(4.02718653431 + 7576.560073574 * t);
            R0 += 0.00000000087 * Math.Cos(6.01842459303 + 17085.9586657222 * t);
            R0 += 0.00000000109 * Math.Cos(5.46886607309 + 52670.0695933026 * t);
            R0 += 0.00000000107 * Math.Cos(0.54805946713 + 34363.365597556 * t);
            R0 += 0.00000000108 * Math.Cos(5.44460610707 + 19050.7972925762 * t);
            R0 += 0.00000000076 * Math.Cos(6.15177368654 + 27682.1407441564 * t);
            R0 += 0.00000000107 * Math.Cos(4.80525404063 + 8144.2787113044 * t);
            R0 += 0.00000000073 * Math.Cos(1.60549217847 + 20956.2620575166 * t);
            R0 += 0.00000000097 * Math.Cos(5.13542051130 + 22779.4372461938 * t);
            R0 += 0.00000000068 * Math.Cos(2.31300447144 + 8631.326197928 * t);
            R0 += 0.00000000091 * Math.Cos(4.28652743953 + 10110.1927719924 * t);
            R0 += 0.00000000093 * Math.Cos(5.27290609264 + 522.5774180938 * t);
            R0 += 0.00000000071 * Math.Cos(3.65565961690 + 11764.330768859 * t);
            R0 += 0.00000000089 * Math.Cos(1.79712963206 + 45585.1728121874 * t);
            R0 += 0.00000000067 * Math.Cos(2.25900071584 + 9360.089164459 * t);
            R0 += 0.00000000085 * Math.Cos(0.67062144972 + 56600.2792895222 * t);
            R0 += 0.00000000080 * Math.Cos(1.58278081077 + 19992.85935454519 * t);
            R0 += 0.00000000065 * Math.Cos(6.23472325597 + 10419.4710946482 * t);
            R0 += 0.00000000064 * Math.Cos(0.53356325917 + 17248.4253018544 * t);
            R0 += 0.00000000085 * Math.Cos(4.52011215904 + 29786.660256881 * t);
            R0 += 0.00000000068 * Math.Cos(4.48235266554 + 10632.7701900862 * t);
            R0 += 0.00000000064 * Math.Cos(4.33495700921 + 47938.0389659592 * t);
            R0 += 0.00000000071 * Math.Cos(3.03858484137 + 11787.1059703098 * t);
            R0 += 0.00000000087 * Math.Cos(4.81823063172 + 2107.0345075424 * t);
            R0 += 0.00000000070 * Math.Cos(2.35648061034 + 11794.1522070078 * t);
            R0 += 0.00000000080 * Math.Cos(2.33248094128 + 38526.574350872 * t);
            R0 += 0.00000000070 * Math.Cos(3.70454061100 + 8734.4189721466 * t);
            R0 += 0.00000000077 * Math.Cos(4.49569185467 + 20007.0864485468 * t);
            R0 += 0.00000000072 * Math.Cos(1.19410424468 + 10217.2176994741 * t);
            R0 += 0.00000000068 * Math.Cos(2.01841060183 + 14128.2427712456 * t);
            R0 += 0.00000000064 * Math.Cos(5.39293951654 + 7880.08915333899 * t);
            R0 += 0.00000000066 * Math.Cos(3.20467071127 + 14765.2390432698 * t);
            R0 += 0.00000000080 * Math.Cos(3.41620457770 + 48417.97290558199 * t);
            R0 += 0.00000000080 * Math.Cos(3.39651161571 + 245.8316462294 * t);
            R0 += 0.00000000066 * Math.Cos(5.85414440204 + 9793.8009023358 * t);
            R0 += 0.00000000082 * Math.Cos(3.62592908644 + 70743.77453195279 * t);
            R0 += 0.00000000058 * Math.Cos(4.95174942212 + 30110.1656735384 * t);
            R0 += 0.00000000079 * Math.Cos(6.24161471033 + 6037.244203762 * t);
            R0 += 0.00000000069 * Math.Cos(5.50183658445 + 19793.7873531088 * t);
            R0 += 0.00000000056 * Math.Cos(1.24148350566 + 10207.7626219036 * t);
            R0 += 0.00000000070 * Math.Cos(2.45123308846 + 10218.8084705184 * t);
            R0 += 0.00000000064 * Math.Cos(5.53983104501 + 10735.8629643048 * t);
            R0 += 0.00000000054 * Math.Cos(3.62259713240 + 27461.7108480654 * t);
            R0 += 0.00000000073 * Math.Cos(1.75882480924 + 1589.0728952838 * t);
            R0 += 0.00000000075 * Math.Cos(3.38244819846 + 4214.0690150848 * t);
            R0 += 0.00000000054 * Math.Cos(0.64971567468 + 9929.4262273458 * t);
            R0 += 0.00000000054 * Math.Cos(3.40959637230 + 18418.01355326299 * t);
            R0 += 0.00000000056 * Math.Cos(3.65815006538 + 14169.7935622304 * t);
            R0 += 0.00000000056 * Math.Cos(0.71243223808 + 9896.8936765544 * t);
            R0 += 0.00000000052 * Math.Cos(1.33348131940 + 20400.2727726222 * t);
            R0 += 0.00000000067 * Math.Cos(3.12806595400 + 5481.2549188676 * t);
            R0 += 0.00000000058 * Math.Cos(0.54482893546 + 28313.288804661 * t);
            R0 += 0.00000000054 * Math.Cos(0.15603935681 + 19580.4882576708 * t);
            R0 += 0.00000000051 * Math.Cos(3.37515473510 + 9256.9963902404 * t);
            R0 += 0.00000000063 * Math.Cos(3.38848970950 + 49515.382508407 * t);
            R0 += 0.00000000069 * Math.Cos(4.90917651401 + 63498.47038145279 * t);
            R0 += 0.00000000057 * Math.Cos(5.07437742030 + 18521.1063274816 * t);
            R0 += 0.00000000050 * Math.Cos(1.59156823654 + 18631.31264870099 * t);
            R0 += 0.00000000054 * Math.Cos(6.25816208666 + 37674.9963942764 * t);
            R0 += 0.00000000057 * Math.Cos(5.48065460919 + 24383.0791084414 * t);
            R0 += 0.00000000045 * Math.Cos(1.10466490660 + 10408.2569306716 * t);
            R0 += 0.00000000051 * Math.Cos(3.61196470313 + 426.598190876 * t);
            R0 += 0.00000000057 * Math.Cos(2.09567711267 + 60530.4889857418 * t);
            R0 += 0.00000000060 * Math.Cos(5.94659889997 + 13897.6635962012 * t);
            R0 += 0.00000000051 * Math.Cos(5.47238517720 + 57837.1383323006 * t);
            R0 += 0.00000000051 * Math.Cos(2.32438478428 + 19779.56025910719 * t);
            R0 += 0.00000000052 * Math.Cos(3.23766328818 + 18940.59097135679 * t);
            R0 += 0.00000000043 * Math.Cos(5.74921510909 + 51868.2486621788 * t);
            R0 += 0.00000000048 * Math.Cos(1.12206254877 + 9779.5738083342 * t);
            R0 += 0.00000000058 * Math.Cos(3.08646083897 + 12074.488407524 * t);
            R0 += 0.00000000046 * Math.Cos(4.07536026888 + 7863.9425107882 * t);
            R0 += 0.00000000045 * Math.Cos(4.75746520642 + 7856.89627409019 * t);
            R0 += 0.00000000054 * Math.Cos(4.43528236634 + 8617.0991039264 * t);
            R0 += 0.00000000050 * Math.Cos(3.70569982975 + 42456.7840470916 * t);
            R0 += 0.00000000044 * Math.Cos(1.29248911155 + 69166.430989505 * t);
            R0 += 0.00000000046 * Math.Cos(0.41229872114 + 7564.830720738 * t);
            R0 += 0.00000000044 * Math.Cos(6.17937388307 + 13341.6743113068 * t);
            R0 += 0.00000000053 * Math.Cos(4.71388531889 + 53445.5922046266 * t);
            R0 += 0.00000000041 * Math.Cos(3.48003037828 + 37895.4262903674 * t);
            R0 += 0.00000000040 * Math.Cos(1.23305546260 + 10228.538017396 * t);
            R0 += 0.00000000053 * Math.Cos(5.04979874661 + 74673.9842281724 * t);
            R0 += 0.00000000039 * Math.Cos(1.36646013032 + 21202.093703746 * t);
            R0 += 0.00000000039 * Math.Cos(2.15376025201 + 8947.7180675846 * t);
            R0 += 0.00000000041 * Math.Cos(6.17532984460 + 65236.2212932854 * t);
            R0 += 0.00000000052 * Math.Cos(1.29052331493 + 90394.82301305079 * t);
            R0 += 0.00000000039 * Math.Cos(0.70253732683 + 18093.37469954999 * t);
            R0 += 0.00000000052 * Math.Cos(1.18164377451 + 10211.8010735027 * t);
            R0 += 0.00000000047 * Math.Cos(1.78672260794 + 10401.2106939736 * t);
            R0 += 0.00000000040 * Math.Cos(3.66961416802 + 10198.033075026 * t);
            R0 += 0.00000000051 * Math.Cos(2.71698589018 + 94325.0327092704 * t);
            R0 += 0.00000000036 * Math.Cos(1.25091711620 + 10323.4918674304 * t);
            R0 += 0.00000000049 * Math.Cos(1.21335959420 + 9721.6222537522 * t);
            R0 += 0.00000000042 * Math.Cos(6.05968230173 + 105460.99111839019 * t);
            R0 += 0.00000000046 * Math.Cos(5.06978748275 + 20350.3050211464 * t);
            R0 += 0.00000000040 * Math.Cos(1.97645050921 + 32243.4985008806 * t);
            R0 += 0.00000000036 * Math.Cos(4.96702216961 + 36301.18868778519 * t);
            R0 += 0.00000000037 * Math.Cos(5.29642935562 + 38.0276726358 * t);
            R0 += 0.00000000039 * Math.Cos(0.52064327313 + 26709.6469424134 * t);
            R0 += 0.00000000035 * Math.Cos(2.34112124655 + 58946.51688439399 * t);
            R0 += 0.00000000034 * Math.Cos(1.82989750626 + 17675.0234927304 * t);
            R0 += 0.00000000034 * Math.Cos(0.76493664110 + 55798.4583583984 * t);
            R0 += 0.00000000035 * Math.Cos(1.09353675147 + 69159.80243060499 * t);
            R0 += 0.00000000031 * Math.Cos(5.59148330297 + 10639.883737087 * t);
            R0 += 0.00000000032 * Math.Cos(3.32960781870 + 71519.2971432768 * t);
            R0 += 0.00000000031 * Math.Cos(5.98191446392 + 24341.5283174566 * t);
            R0 += 0.00000000031 * Math.Cos(0.68615213145 + 10202.2398459471 * t);
            R0 += 0.00000000030 * Math.Cos(4.42039942947 + 10459.1171924404 * t);
            R0 += 0.00000000029 * Math.Cos(1.30367701539 + 20103.06567576459 * t);
            R0 += 0.00000000031 * Math.Cos(4.51793347997 + 2957.7158944766 * t);
            R0 += 0.00000000035 * Math.Cos(4.05634321290 + 19903.99367432819 * t);
            R0 += 0.00000000030 * Math.Cos(1.32113757427 + 574.3447983348 * t);
            R0 += 0.00000000029 * Math.Cos(3.36506645849 + 10288.0671447783 * t);
            R0 += 0.00000000029 * Math.Cos(1.40019042576 + 9988.9407505091 * t);
            R0 += 0.00000000032 * Math.Cos(0.21932095318 + 24978.5245894808 * t);
            R0 += 0.00000000034 * Math.Cos(5.22945947227 + 8673.969676399 * t);
            R0 += 0.00000000039 * Math.Cos(4.50883171158 + 16004.6981037436 * t);
            R0 += 0.00000000028 * Math.Cos(2.32945945641 + 11392.4800852506 * t);
            R0 += 0.00000000034 * Math.Cos(3.92498967835 + 536.8045120954 * t);
            R0 += 0.00000000032 * Math.Cos(5.46972716255 + 64607.84893354619 * t);
            R0 += 0.00000000028 * Math.Cos(2.38858990128 + 20235.1228263104 * t);
            R0 += 0.00000000030 * Math.Cos(3.34585843979 + 39793.7602546548 * t);
            R0 += 0.00000000026 * Math.Cos(5.36096904409 + 1478.8665740644 * t);
            return R0;
        }

        internal static double Venus_R1(double t) // 180 terms of order 1
        {
            double R1 = 0;
            R1 += 0.00034551039 * Math.Cos(0.89198710598 + 10213.285546211 * t);
            R1 += 0.00000234203 * Math.Cos(1.77224942714 + 20426.571092422 * t);
            R1 -= 0.00000233998;
            R1 += 0.00000023864 * Math.Cos(1.11274502648 + 9437.762934887 * t);
            R1 += 0.00000010568 * Math.Cos(4.59168210921 + 1577.3435424478 * t);
            R1 += 0.00000009124 * Math.Cos(4.53540907003 + 10404.7338123226 * t);
            R1 += 0.00000006599 * Math.Cos(5.97703999838 + 5507.5532386674 * t);
            R1 += 0.00000004667 * Math.Cos(3.87683960551 + 9153.9036160218 * t);
            R1 += 0.00000003840 * Math.Cos(5.66196924375 + 13367.9726311066 * t);
            R1 += 0.00000002666 * Math.Cos(2.82413291285 + 10206.1719992102 * t);
            R1 += 0.00000002194 * Math.Cos(2.05314419626 + 775.522611324 * t);
            R1 += 0.00000002094 * Math.Cos(2.55137285015 + 18837.49819713819 * t);
            R1 += 0.00000001782 * Math.Cos(2.64808558644 + 30639.856638633 * t);
            R1 += 0.00000001845 * Math.Cos(1.87612936641 + 11015.1064773348 * t);
            R1 += 0.00000001303 * Math.Cos(0.20613045603 + 11322.6640983044 * t);
            R1 += 0.00000001169 * Math.Cos(0.79431893441 + 17298.1823273262 * t);
            R1 += 0.00000001001 * Math.Cos(6.16555101536 + 10239.5838660108 * t);
            R1 += 0.00000000915 * Math.Cos(4.59854496966 + 1109.3785520934 * t);
            R1 += 0.00000000884 * Math.Cos(0.66706834422 + 18073.7049386502 * t);
            R1 += 0.00000000849 * Math.Cos(5.58641571940 + 12566.1516999828 * t);
            R1 += 0.00000001071 * Math.Cos(4.94792017474 + 6283.0758499914 * t);
            R1 += 0.00000000887 * Math.Cos(2.47785193216 + 3154.6870848956 * t);
            R1 += 0.00000000904 * Math.Cos(0.81413053841 + 10596.1820784342 * t);
            R1 += 0.00000000818 * Math.Cos(0.90016838097 + 5661.3320491522 * t);
            R1 += 0.00000000845 * Math.Cos(5.48504338112 + 529.6909650946 * t);
            R1 += 0.00000000824 * Math.Cos(3.74837629121 + 7084.8967811152 * t);
            R1 += 0.00000000652 * Math.Cos(5.07444932607 + 22003.9146348698 * t);
            R1 += 0.00000000847 * Math.Cos(0.44119876869 + 8635.9420037632 * t);
            R1 += 0.00000000638 * Math.Cos(4.10125791268 + 191.4482661116 * t);
            R1 += 0.00000000615 * Math.Cos(3.14417599741 + 10186.9872264112 * t);
            R1 += 0.00000000527 * Math.Cos(5.86792949279 + 2352.8661537718 * t);
            R1 += 0.00000000520 * Math.Cos(5.33201358267 + 14143.4952424306 * t);
            R1 += 0.00000000576 * Math.Cos(2.25212731258 + 21228.3920235458 * t);
            R1 += 0.00000000662 * Math.Cos(2.86880467345 + 8624.2126509272 * t);
            R1 += 0.00000000554 * Math.Cos(2.17186191243 + 18307.8072320436 * t);
            R1 += 0.00000000515 * Math.Cos(4.34331395104 + 9786.687355335 * t);
            R1 += 0.00000000501 * Math.Cos(5.56479589366 + 10742.9765113056 * t);
            R1 += 0.00000000426 * Math.Cos(1.02161443120 + 7064.1213856228 * t);
            R1 += 0.00000000418 * Math.Cos(1.26803034691 + 9676.4810341156 * t);
            R1 += 0.00000000391 * Math.Cos(0.78974645621 + 9690.7081281172 * t);
            R1 += 0.00000000334 * Math.Cos(3.18175822557 + 10988.808157535 * t);
            R1 += 0.00000000375 * Math.Cos(0.66142254036 + 19360.07561523199 * t);
            R1 += 0.00000000364 * Math.Cos(0.19369831864 + 19374.3027092336 * t);
            R1 += 0.00000000313 * Math.Cos(1.09734397626 + 4551.9534970588 * t);
            R1 += 0.00000000330 * Math.Cos(0.58817502306 + 16496.3613962024 * t);
            R1 += 0.00000000339 * Math.Cos(5.76768761396 + 10021.8372800994 * t);
            R1 += 0.00000000291 * Math.Cos(3.65846764668 + 25158.6017197654 * t);
            R1 += 0.00000000223 * Math.Cos(4.33581625553 + 19786.67380610799 * t);
            R1 += 0.00000000266 * Math.Cos(3.57408827667 + 801.8209311238 * t);
            R1 += 0.00000000274 * Math.Cos(5.73346687248 + 11790.6290886588 * t);
            R1 += 0.00000000275 * Math.Cos(5.65814317085 + 19896.8801273274 * t);
            R1 += 0.00000000212 * Math.Cos(4.27038489878 + 4705.7323075436 * t);
            R1 += 0.00000000230 * Math.Cos(6.13406345590 + 1059.3819301892 * t);
            R1 += 0.00000000204 * Math.Cos(4.87348390351 + 7860.4193924392 * t);
            R1 += 0.00000000241 * Math.Cos(1.13551531894 + 26.2983197998 * t);
            R1 += 0.00000000206 * Math.Cos(0.31907973682 + 382.8965322232 * t);
            R1 += 0.00000000216 * Math.Cos(2.54741101724 + 19651.048481098 * t);
            R1 += 0.00000000212 * Math.Cos(3.15264941106 + 14945.3161735544 * t);
            R1 += 0.00000000163 * Math.Cos(1.13604744392 + 13936.794505134 * t);
            R1 += 0.00000000151 * Math.Cos(5.11341268743 + 28521.0927782546 * t);
            R1 += 0.00000000151 * Math.Cos(0.81278755582 + 6770.7106012456 * t);
            R1 += 0.00000000150 * Math.Cos(5.02227334847 + 29088.811415985 * t);
            R1 += 0.00000000146 * Math.Cos(1.37568138685 + 10220.3990932118 * t);
            R1 += 0.00000000127 * Math.Cos(4.49298610074 + 3532.0606928114 * t);
            R1 += 0.00000000121 * Math.Cos(6.26589208179 + 29580.4747084438 * t);
            R1 += 0.00000000147 * Math.Cos(6.16092774714 + 8662.240323563 * t);
            R1 += 0.00000000114 * Math.Cos(0.00114012635 + 25934.1243310894 * t);
            R1 += 0.00000000115 * Math.Cos(3.56897715344 + 24356.7807886416 * t);
            R1 += 0.00000000124 * Math.Cos(0.67547060274 + 3723.508958923 * t);
            R1 += 0.00000000145 * Math.Cos(0.36415036222 + 9146.790069021 * t);
            R1 += 0.00000000104 * Math.Cos(4.27865011376 + 9573.388259897 * t);
            R1 += 0.00000000136 * Math.Cos(5.09581116181 + 19367.1891622328 * t);
            R1 += 0.00000000102 * Math.Cos(1.53637788668 + 17277.4069318338 * t);
            R1 += 0.00000000117 * Math.Cos(0.57543238496 + 9999.986450773 * t);
            R1 += 0.00000000092 * Math.Cos(0.22936081655 + 18830.38465013739 * t);
            R1 += 0.00000000112 * Math.Cos(4.04771058036 + 9103.9069941176 * t);
            R1 += 0.00000000098 * Math.Cos(3.78447692407 + 213.299095438 * t);
            R1 += 0.00000000085 * Math.Cos(5.84471458481 + 10234.0609417034 * t);
            R1 += 0.00000000079 * Math.Cos(0.64440357793 + 18844.61174413899 * t);
            R1 += 0.00000000084 * Math.Cos(0.56950139213 + 9683.5945811164 * t);
            R1 += 0.00000000107 * Math.Cos(1.77067111589 + 17778.11626694899 * t);
            R1 += 0.00000000081 * Math.Cos(6.19048382717 + 20618.0193585336 * t);
            R1 += 0.00000000087 * Math.Cos(0.15771136594 + 33019.0211122046 * t);
            R1 += 0.00000000082 * Math.Cos(4.80683817059 + 3930.2096962196 * t);
            R1 += 0.00000000086 * Math.Cos(2.21505615071 + 8094.5216858326 * t);
            R1 += 0.00000000064 * Math.Cos(2.69215119482 + 16983.9961474566 * t);
            R1 += 0.00000000069 * Math.Cos(0.83385751986 + 3128.3887650958 * t);
            R1 += 0.00000000081 * Math.Cos(4.88025042367 + 4732.0306273434 * t);
            R1 += 0.00000000059 * Math.Cos(3.34348033725 + 10787.6303445458 * t);
            R1 += 0.00000000061 * Math.Cos(0.04044699966 + 9161.0171630226 * t);
            R1 += 0.00000000064 * Math.Cos(4.13127333938 + 9992.8729037722 * t);
            R1 += 0.00000000060 * Math.Cos(6.24603986632 + 32217.2001810808 * t);
            R1 += 0.00000000054 * Math.Cos(3.38449893196 + 10426.584641649 * t);
            R1 += 0.00000000054 * Math.Cos(5.15939119644 + 28286.9904848612 * t);
            R1 += 0.00000000063 * Math.Cos(4.32339245083 + 12592.4500197826 * t);
            R1 += 0.00000000060 * Math.Cos(4.48753846170 + 18875.525869774 * t);
            R1 += 0.00000000057 * Math.Cos(3.64912085313 + 10007.0999977738 * t);
            R1 += 0.00000000049 * Math.Cos(5.10267262491 + 19573.37471066999 * t);
            R1 += 0.00000000047 * Math.Cos(5.79444960738 + 68050.42387851159 * t);
            R1 += 0.00000000052 * Math.Cos(3.56658420552 + 7255.5696517344 * t);
            R1 += 0.00000000050 * Math.Cos(1.61783309819 + 36949.2308084242 * t);
            R1 += 0.00000000053 * Math.Cos(2.64370544855 + 15874.6175953632 * t);
            R1 += 0.00000000040 * Math.Cos(3.93466530964 + 20419.45754542119 * t);
            R1 += 0.00000000051 * Math.Cos(0.79154899901 + 23581.2581773176 * t);
            R1 += 0.00000000038 * Math.Cos(1.77428239418 + 10103.0792249916 * t);
            R1 += 0.00000000049 * Math.Cos(1.12423644455 + 3442.5749449654 * t);
            R1 += 0.00000000040 * Math.Cos(5.22874487975 + 21535.9496445154 * t);
            R1 += 0.00000000038 * Math.Cos(1.12473430132 + 7.1135470008 * t);
            R1 += 0.00000000038 * Math.Cos(0.11510547453 + 11272.6674764002 * t);
            R1 += 0.00000000036 * Math.Cos(2.02476324983 + 7058.5984613154 * t);
            R1 += 0.00000000047 * Math.Cos(0.05589432390 + 12432.0426503978 * t);
            R1 += 0.00000000034 * Math.Cos(3.45481114998 + 9830.3890139878 * t);
            R1 += 0.00000000045 * Math.Cos(4.59817214088 + 10192.5101507186 * t);
            R1 += 0.00000000037 * Math.Cos(4.93959675364 + 3340.6124266998 * t);
            R1 += 0.00000000044 * Math.Cos(0.70533027806 + 20213.271996984 * t);
            R1 += 0.00000000034 * Math.Cos(2.16487642765 + 64460.6986819614 * t);
            R1 += 0.00000000031 * Math.Cos(1.57612397319 + 36147.4098773004 * t);
            R1 += 0.00000000028 * Math.Cos(2.56454760402 + 94138.32702008578 * t);
            R1 += 0.00000000033 * Math.Cos(1.08907268562 + 29864.334027309 * t);
            R1 += 0.00000000029 * Math.Cos(0.59718407064 + 59728.668054618 * t);
            R1 += 0.00000000031 * Math.Cos(3.04423979263 + 40879.4405046438 * t);
            R1 += 0.00000000035 * Math.Cos(0.32247158762 + 1589.0728952838 * t);
            R1 += 0.00000000031 * Math.Cos(3.27727318906 + 19992.85935454519 * t);
            R1 += 0.00000000027 * Math.Cos(5.83705748551 + 17085.9586657222 * t);
            R1 += 0.00000000032 * Math.Cos(2.64260788260 + 41962.5207369374 * t);
            R1 += 0.00000000028 * Math.Cos(4.90613317287 + 29050.7837433492 * t);
            R1 += 0.00000000025 * Math.Cos(4.55050389739 + 14919.0178537546 * t);
            R1 += 0.00000000028 * Math.Cos(3.58851614957 + 40853.142184844 * t);
            R1 += 0.00000000029 * Math.Cos(2.79705093386 + 20007.0864485468 * t);
            R1 += 0.00000000033 * Math.Cos(0.93862065616 + 15720.8387848784 * t);
            R1 += 0.00000000024 * Math.Cos(2.74970637101 + 18947.7045183576 * t);
            R1 += 0.00000000024 * Math.Cos(4.38966861409 + 46386.9937433112 * t);
            R1 += 0.00000000024 * Math.Cos(0.73361964525 + 9411.4646150872 * t);
            R1 += 0.00000000028 * Math.Cos(4.19559784013 + 37674.9963942764 * t);
            R1 += 0.00000000023 * Math.Cos(1.00023735538 + 22779.4372461938 * t);
            R1 += 0.00000000026 * Math.Cos(0.46990555736 + 13745.3462390224 * t);
            R1 += 0.00000000028 * Math.Cos(4.65181292126 + 1551.045222648 * t);
            R1 += 0.00000000025 * Math.Cos(4.18690270765 + 44007.8292697396 * t);
            R1 += 0.00000000022 * Math.Cos(0.98102807789 + 426.598190876 * t);
            R1 += 0.00000000030 * Math.Cos(1.24986033487 + 27461.7108480654 * t);
            R1 += 0.00000000027 * Math.Cos(3.94986823486 + 17468.8551979454 * t);
            R1 += 0.00000000021 * Math.Cos(6.09897508157 + 18300.69368504279 * t);
            R1 += 0.00000000025 * Math.Cos(4.75875623888 + 27991.40181316 * t);
            R1 += 0.00000000022 * Math.Cos(2.95281481673 + 40077.61957352 * t);
            R1 += 0.00000000028 * Math.Cos(6.12038264955 + 38500.2760310722 * t);
            R1 += 0.00000000022 * Math.Cos(4.11184201321 + 19779.56025910719 * t);
            R1 += 0.00000000027 * Math.Cos(3.72446446080 + 19793.7873531088 * t);
            R1 += 0.00000000020 * Math.Cos(4.27086627368 + 31441.6775697568 * t);
            R1 += 0.00000000022 * Math.Cos(4.99040169444 + 31022.7531708562 * t);
            R1 += 0.00000000023 * Math.Cos(1.33505132122 + 65236.2212932854 * t);
            R1 += 0.00000000021 * Math.Cos(4.46897353468 + 53285.1848352418 * t);
            R1 += 0.00000000020 * Math.Cos(4.15140915983 + 2218.7571041868 * t);
            R1 += 0.00000000025 * Math.Cos(2.18447182965 + 27511.4678735372 * t);
            R1 += 0.00000000019 * Math.Cos(1.43653410349 + 27197.2816936676 * t);
            R1 += 0.00000000027 * Math.Cos(1.22555218015 + 42430.4857272918 * t);
            R1 += 0.00000000019 * Math.Cos(3.65054338893 + 49515.382508407 * t);
            R1 += 0.00000000022 * Math.Cos(5.88380811711 + 10218.8084705184 * t);
            R1 += 0.00000000018 * Math.Cos(2.29853355765 + 19264.0963880142 * t);
            R1 += 0.00000000017 * Math.Cos(5.44429906531 + 6681.2248533996 * t);
            R1 += 0.00000000020 * Math.Cos(3.68116637773 + 14128.2427712456 * t);
            R1 += 0.00000000021 * Math.Cos(4.30316190532 + 44809.6502008634 * t);
            R1 += 0.00000000020 * Math.Cos(2.48583613985 + 33794.5437235286 * t);
            R1 += 0.00000000017 * Math.Cos(3.02735393984 + 28528.2063252554 * t);
            R1 += 0.00000000019 * Math.Cos(5.92656850674 + 22805.7355659936 * t);
            R1 += 0.00000000022 * Math.Cos(5.30827572791 + 10207.7626219036 * t);
            R1 += 0.00000000020 * Math.Cos(0.75829381378 + 18314.9207790444 * t);
            R1 += 0.00000000017 * Math.Cos(5.63315744126 + 16522.6597160022 * t);
            R1 += 0.00000000016 * Math.Cos(1.71021408448 + 536.8045120954 * t);
            R1 += 0.00000000015 * Math.Cos(5.27016880041 + 53445.5922046266 * t);
            R1 += 0.00000000017 * Math.Cos(5.61443395877 + 47938.0389659592 * t);
            R1 += 0.00000000015 * Math.Cos(5.81110284451 + 43071.8992890308 * t);
            R1 += 0.00000000015 * Math.Cos(4.96237667003 + 19999.97290154599 * t);
            R1 += 0.00000000018 * Math.Cos(0.55618686515 + 14765.2390432698 * t);
            R1 += 0.00000000014 * Math.Cos(3.48144272414 + 29786.660256881 * t);
            R1 += 0.00000000015 * Math.Cos(5.84132627836 + 10228.538017396 * t);
            R1 += 0.00000000016 * Math.Cos(1.05720065324 + 26735.9452622132 * t);
            R1 += 0.00000000014 * Math.Cos(6.08462030302 + 35371.8872659764 * t);
            R1 += 0.00000000014 * Math.Cos(2.84532871890 + 574.3447983348 * t);
            R1 += 0.00000000015 * Math.Cos(5.34517715140 + 10198.033075026 * t);
            R1 += 0.00000000013 * Math.Cos(0.45004137509 + 20452.8694122218 * t);
            return R1 * t;
        }

        internal static double Venus_R2(double t) // 63 terms of order 2
        {
            double R2 = 0;
            R2 += 0.00001406587 * Math.Cos(5.06366395190 + 10213.285546211 * t);
            R2 += 0.00000015529 * Math.Cos(5.47321687981 + 20426.571092422 * t);
            R2 += 0.00000013059;
            R2 += 0.00000001099 * Math.Cos(2.78883988292 + 9437.762934887 * t);
            R2 += 0.00000000488 * Math.Cos(6.27806914496 + 1577.3435424478 * t);
            R2 += 0.00000000361 * Math.Cos(6.11914188253 + 10404.7338123226 * t);
            R2 += 0.00000000310 * Math.Cos(1.38984998403 + 5507.5532386674 * t);
            R2 += 0.00000000389 * Math.Cos(1.95017779915 + 11015.1064773348 * t);
            R2 += 0.00000000372 * Math.Cos(2.33222828423 + 775.522611324 * t);
            R2 += 0.00000000207 * Math.Cos(5.63406721595 + 10239.5838660108 * t);
            R2 += 0.00000000168 * Math.Cos(1.10765197296 + 13367.9726311066 * t);
            R2 += 0.00000000175 * Math.Cos(6.16674652950 + 30639.856638633 * t);
            R2 += 0.00000000168 * Math.Cos(3.64495311632 + 7084.8967811152 * t);
            R2 += 0.00000000120 * Math.Cos(5.85815843789 + 9153.9036160218 * t);
            R2 += 0.00000000160 * Math.Cos(2.21564938463 + 3154.6870848956 * t);
            R2 += 0.00000000118 * Math.Cos(2.62358866565 + 8635.9420037632 * t);
            R2 += 0.00000000112 * Math.Cos(2.36235956804 + 10596.1820784342 * t);
            R2 += 0.00000000092 * Math.Cos(0.72664449269 + 12566.1516999828 * t);
            R2 += 0.00000000067 * Math.Cos(3.76089669118 + 18837.49819713819 * t);
            R2 += 0.00000000065 * Math.Cos(2.47983709990 + 11790.6290886588 * t);
            R2 += 0.00000000048 * Math.Cos(4.26620187144 + 2352.8661537718 * t);
            R2 += 0.00000000048 * Math.Cos(5.50898189550 + 191.4482661116 * t);
            R2 += 0.00000000048 * Math.Cos(2.54730918293 + 17298.1823273262 * t);
            R2 += 0.00000000046 * Math.Cos(3.40293459332 + 14143.4952424306 * t);
            R2 += 0.00000000041 * Math.Cos(1.83997113019 + 11322.6640983044 * t);
            R2 += 0.00000000037 * Math.Cos(6.17871126027 + 1109.3785520934 * t);
            R2 += 0.00000000039 * Math.Cos(4.77190210316 + 18073.7049386502 * t);
            R2 += 0.00000000035 * Math.Cos(3.10133256432 + 4705.7323075436 * t);
            R2 += 0.00000000046 * Math.Cos(3.30090415967 + 6283.0758499914 * t);
            R2 += 0.00000000034 * Math.Cos(3.91721765773 + 10021.8372800994 * t);
            R2 += 0.00000000034 * Math.Cos(3.24663787383 + 22003.9146348698 * t);
            R2 += 0.00000000042 * Math.Cos(3.39360926939 + 14945.3161735544 * t);
            R2 += 0.00000000044 * Math.Cos(4.42979374073 + 7860.4193924392 * t);
            R2 += 0.00000000034 * Math.Cos(2.16381407025 + 16496.3613962024 * t);
            R2 += 0.00000000031 * Math.Cos(0.45714618479 + 26.2983197998 * t);
            R2 += 0.00000000035 * Math.Cos(3.62868651241 + 801.8209311238 * t);
            R2 += 0.00000000032 * Math.Cos(1.84138997078 + 382.8965322232 * t);
            R2 += 0.00000000025 * Math.Cos(3.32908650295 + 18307.8072320436 * t);
            R2 += 0.00000000026 * Math.Cos(3.64313769818 + 29088.811415985 * t);
            R2 += 0.00000000029 * Math.Cos(3.82967178810 + 10186.9872264112 * t);
            R2 += 0.00000000022 * Math.Cos(3.17741520378 + 28521.0927782546 * t);
            R2 += 0.00000000021 * Math.Cos(2.52643834111 + 529.6909650946 * t);
            R2 += 0.00000000025 * Math.Cos(5.71401244457 + 21202.093703746 * t);
            R2 += 0.00000000021 * Math.Cos(3.77813434325 + 21228.3920235458 * t);
            R2 += 0.00000000019 * Math.Cos(5.24505118517 + 19896.8801273274 * t);
            R2 += 0.00000000018 * Math.Cos(4.62463651925 + 19651.048481098 * t);
            R2 += 0.00000000016 * Math.Cos(3.35893297896 + 28286.9904848612 * t);
            R2 += 0.00000000015 * Math.Cos(5.05571633205 + 33019.0211122046 * t);
            R2 += 0.00000000014 * Math.Cos(2.83786355803 + 19786.67380610799 * t);
            R2 += 0.00000000014 * Math.Cos(1.79922718553 + 9830.3890139878 * t);
            R2 += 0.00000000014 * Math.Cos(3.14801263138 + 19367.1891622328 * t);
            R2 += 0.00000000014 * Math.Cos(3.57896195191 + 10988.808157535 * t);
            R2 += 0.00000000013 * Math.Cos(3.06303088617 + 10742.9765113056 * t);
            R2 += 0.00000000013 * Math.Cos(5.43981998532 + 25158.6017197654 * t);
            R2 += 0.00000000015 * Math.Cos(4.83166312889 + 18875.525869774 * t);
            R2 += 0.00000000012 * Math.Cos(2.54141086214 + 7064.1213856228 * t);
            R2 += 0.00000000012 * Math.Cos(4.45255110769 + 15720.8387848784 * t);
            R2 += 0.00000000010 * Math.Cos(1.87933121728 + 24356.7807886416 * t);
            R2 += 0.00000000011 * Math.Cos(2.58708635685 + 9103.9069941176 * t);
            R2 += 0.00000000010 * Math.Cos(2.17901309900 + 3723.508958923 * t);
            R2 += 0.00000000008 * Math.Cos(3.63520673832 + 1059.3819301892 * t);
            R2 += 0.00000000008 * Math.Cos(4.67523115598 + 25934.1243310894 * t);
            R2 += 0.00000000009 * Math.Cos(5.97856553283 + 9683.5945811164 * t);
            return R2 * t * t;
        }

        internal static double Venus_R3(double t) // 7 terms of order 3
        {
            double R3 = 0;
            R3 += 0.00000049582 * Math.Cos(3.22263554520 + 10213.285546211 * t);
            R3 += 0.00000000831 * Math.Cos(3.21219077104 + 20426.571092422 * t);
            R3 -= 0.00000000112;
            R3 += 0.00000000013 * Math.Cos(3.77448689585 + 30639.856638633 * t);
            R3 += 0.00000000009 * Math.Cos(4.19802043629 + 10239.5838660108 * t);
            R3 += 0.00000000006 * Math.Cos(0.20714935358 + 10186.9872264112 * t);
            R3 += 0.00000000005 * Math.Cos(0.68781956122 + 8635.9420037632 * t);
            return R3 * t * t * t;
        }

        internal static double Venus_R4(double t) // 3 terms of order 4
        {
            double R4 = 0;
            R4 += 0.00000000573 * Math.Cos(0.92229697820 + 10213.285546211 * t);
            R4 += 0.00000000040 * Math.Cos(0.95468912157 + 20426.571092422 * t);
            R4 -= 0.00000000006;
            return R4 * t * t * t * t;
        }

        internal static double Venus_R5(double t) // 2 terms of order 5
        {
            double R5 = 0;
            R5 += 0.00000000045 * Math.Cos(0.30032866722 + 10213.285546211 * t);
            R5 += 0.00000000002 * Math.Cos(5.29627718483 + 20426.571092422 * t);
            return R5 * t * t * t * t * t;
        }

        #endregion 

    }

}
