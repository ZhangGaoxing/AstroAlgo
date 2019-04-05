﻿using AstroAlgo.Base;
using AstroAlgo.Model;

using System;

namespace AstroAlgo.SolarSystem
{
    /// <summary>
    /// 海王星
    /// </summary>
    public class Neptune : IPlanet
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
        /// 初始化一个海王星实例
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <param name="localZone"></param>
        public Neptune(double latitude, double longitude, TimeZoneInfo localZone)
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

            double L = 304.348665 + 219.8833092 * T + 0.00030926 * T * T + 0.000000018 * T * T * T;
            double a = 30.110386869 - 0.0000001663 * T + 0.00000000069 * T * T;
            double e = 0.00898809 + 0.000006408 * T - 0.0000000008 * T * T - 0.00000000005 * T * T * T;
            double i = 1.769952 - 0.0093082 * T - 0.00000708 * T * T + 0.000000028 * T * T * T;
            double Omega = 131.784057 + 1.1022057 * T + 0.00026006 * T * T - 0.000000636 * T * T * T;
            double pi = 48.123691 + 1.4262677 * T + 0.00037918 * T * T - 0.000000003 * T * T * T;

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
                Longitude = BasicTools.AngleSimplification((Neptune_L0(t) + Neptune_L1(t) + Neptune_L2(t) + Neptune_L3(t) + Neptune_L4(t) + Neptune_L5(t)) * (180 / Math.PI)),
                Latitude = (Neptune_B0(t) + Neptune_B1(t) + Neptune_B2(t) + Neptune_B3(t) + Neptune_B4(t) + Neptune_B5(t)) * (180 / Math.PI)
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

            return Neptune_R0(t) + Neptune_R1(t) + Neptune_R2(t) + Neptune_R3(t) + Neptune_R4(t) + Neptune_R5(t);
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
            double t = (Julian.ToJulianDay(time) - jd - 2451545) / 365250.0;

            return new Ecliptic
            {
                Longitude = BasicTools.AngleSimplification((Neptune_L0(t) + Neptune_L1(t) + Neptune_L2(t) + Neptune_L3(t) + Neptune_L4(t) + Neptune_L5(t)) * (180 / Math.PI)),
                Latitude = (Neptune_B0(t) + Neptune_B1(t) + Neptune_B2(t) + Neptune_B3(t) + Neptune_B4(t) + Neptune_B5(t)) * (180 / Math.PI)
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

            return Neptune_R0(t) + Neptune_R1(t) + Neptune_R2(t) + Neptune_R3(t) + Neptune_R4(t) + Neptune_R5(t);
        }

        #endregion

        #region VSOP87

        /*
           NEPTUNE - VSOP87 Series Version D
           HELIOCENTRIC DYNAMICAL ECLIPTIC AND EQUINOX OF THE DATE
           Spherical (L,B,R) Coordinates

           Series Validity Span: 4000 BC < Date < 8000 AD
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

        internal static double Neptune_L0(double t) // 423 terms of order 0
        {
            double L0 = 0;
            L0 += 5.31188633047;
            L0 += 0.01798475509 * Math.Cos(2.90101273050 + 38.1330356378 * t);
            L0 += 0.01019727662 * Math.Cos(0.48580923660 + 1.4844727083 * t);
            L0 += 0.00124531845 * Math.Cos(4.83008090682 + 36.6485629295 * t);
            L0 += 0.00042064450 * Math.Cos(5.41054991607 + 2.9689454166 * t);
            L0 += 0.00037714589 * Math.Cos(6.09221834946 + 35.1640902212 * t);
            L0 += 0.00033784734 * Math.Cos(1.24488865578 + 76.2660712756 * t);
            L0 += 0.00016482741 * Math.Cos(0.00007729261 + 491.5579294568 * t);
            L0 += 0.00009198582 * Math.Cos(4.93747059924 + 39.6175083461 * t);
            L0 += 0.00008994249 * Math.Cos(0.27462142569 + 175.1660598002 * t);
            L0 += 0.00004216235 * Math.Cos(1.98711914364 + 73.297125859 * t);
            L0 += 0.00003364818 * Math.Cos(1.03590121818 + 33.6796175129 * t);
            L0 += 0.00002284800 * Math.Cos(4.20606932559 + 4.4534181249 * t);
            L0 += 0.00001433512 * Math.Cos(2.78340432711 + 74.7815985673 * t);
            L0 += 0.00000900240 * Math.Cos(2.07606702418 + 109.9456887885 * t);
            L0 += 0.00000744996 * Math.Cos(3.19032530145 + 71.8126531507 * t);
            L0 += 0.00000506206 * Math.Cos(5.74785370252 + 114.3991069134 * t);
            L0 += 0.00000399552 * Math.Cos(0.34972342569 + 1021.2488945514 * t);
            L0 += 0.00000345195 * Math.Cos(3.46186210169 + 41.1019810544 * t);
            L0 += 0.00000306338 * Math.Cos(0.49684039897 + 0.5212648618 * t);
            L0 += 0.00000287322 * Math.Cos(4.50523446022 + 0.0481841098 * t);
            L0 += 0.00000323004 * Math.Cos(2.24815188609 + 32.1951448046 * t);
            L0 += 0.00000340323 * Math.Cos(3.30369900416 + 77.7505439839 * t);
            L0 += 0.00000266605 * Math.Cos(4.88932609483 + 0.9632078465 * t);
            L0 += 0.00000227079 * Math.Cos(1.79713054538 + 453.424893819 * t);
            L0 += 0.00000244722 * Math.Cos(1.24693337933 + 9.5612275556 * t);
            L0 += 0.00000232887 * Math.Cos(2.50459795017 + 137.0330241624 * t);
            L0 += 0.00000282170 * Math.Cos(2.24565579693 + 146.594251718 * t);
            L0 += 0.00000251941 * Math.Cos(5.78166597292 + 388.4651552382 * t);
            L0 += 0.00000150180 * Math.Cos(2.99706110414 + 5.9378908332 * t);
            L0 += 0.00000170404 * Math.Cos(3.32390630650 + 108.4612160802 * t);
            L0 += 0.00000151401 * Math.Cos(2.19153094280 + 33.9402499438 * t);
            L0 += 0.00000148295 * Math.Cos(0.85948986145 + 111.4301614968 * t);
            L0 += 0.00000118672 * Math.Cos(3.67706204305 + 2.4476805548 * t);
            L0 += 0.00000101821 * Math.Cos(5.70539236951 + 0.1118745846 * t);
            L0 += 0.00000097873 * Math.Cos(2.80518260528 + 8.0767548473 * t);
            L0 += 0.00000103054 * Math.Cos(4.40441222000 + 70.3281804424 * t);
            L0 += 0.00000103305 * Math.Cos(0.04078966679 + 0.2606324309 * t);
            L0 += 0.00000109300 * Math.Cos(2.41599378049 + 183.2428146475 * t);
            L0 += 0.00000073938 * Math.Cos(1.32805041516 + 529.6909650946 * t);
            L0 += 0.00000077725 * Math.Cos(4.16446516424 + 4.192785694 * t);
            L0 += 0.00000086379 * Math.Cos(4.22834506045 + 490.0734567485 * t);
            L0 += 0.00000081536 * Math.Cos(5.19908046216 + 493.0424021651 * t);
            L0 += 0.00000071503 * Math.Cos(5.29530386579 + 350.3321196004 * t);
            L0 += 0.00000064418 * Math.Cos(3.54541016050 + 168.0525127994 * t);
            L0 += 0.00000062570 * Math.Cos(0.15028731465 + 182.279606801 * t);
            L0 += 0.00000058488 * Math.Cos(3.50106873945 + 145.1097790097 * t);
            L0 += 0.00000048276 * Math.Cos(1.11259925628 + 112.9146342051 * t);
            L0 += 0.00000047229 * Math.Cos(4.57373229818 + 46.2097904851 * t);
            L0 += 0.00000039124 * Math.Cos(1.66569356050 + 213.299095438 * t);
            L0 += 0.00000047728 * Math.Cos(0.12906212461 + 484.444382456 * t);
            L0 += 0.00000046858 * Math.Cos(3.01699530327 + 498.6714764576 * t);
            L0 += 0.00000038659 * Math.Cos(2.38685706479 + 2.9207613068 * t);
            L0 += 0.00000047046 * Math.Cos(4.49844660400 + 173.6815870919 * t);
            L0 += 0.00000047565 * Math.Cos(2.58404814824 + 219.891377577 * t);
            L0 += 0.00000044714 * Math.Cos(5.47302733614 + 176.6505325085 * t);
            L0 += 0.00000032279 * Math.Cos(3.45759151220 + 30.7106720963 * t);
            L0 += 0.00000028249 * Math.Cos(4.13282446716 + 6.592282139 * t);
            L0 += 0.00000024433 * Math.Cos(4.55736848232 + 106.9767433719 * t);
            L0 += 0.00000024661 * Math.Cos(3.67822620786 + 181.7583419392 * t);
            L0 += 0.00000024505 * Math.Cos(1.55095867965 + 7.1135470008 * t);
            L0 += 0.00000021848 * Math.Cos(1.04366818343 + 39.0962434843 * t);
            L0 += 0.00000016936 * Math.Cos(6.10896452834 + 44.7253177768 * t);
            L0 += 0.00000022169 * Math.Cos(2.74932970271 + 256.5399405065 * t);
            L0 += 0.00000016614 * Math.Cos(4.98188930613 + 37.611770776 * t);
            L0 += 0.00000017728 * Math.Cos(3.55049134167 + 1.3725981237 * t);
            L0 += 0.00000017347 * Math.Cos(2.14069234880 + 42.5864537627 * t);
            L0 += 0.00000014953 * Math.Cos(3.36405649131 + 98.8999885246 * t);
            L0 += 0.00000014566 * Math.Cos(0.69857991985 + 1550.939859646 * t);
            L0 += 0.00000015676 * Math.Cos(6.22010212025 + 454.9093665273 * t);
            L0 += 0.00000013243 * Math.Cos(5.61712542227 + 68.8437077341 * t);
            L0 += 0.00000014837 * Math.Cos(3.52557245517 + 25.6028626656 * t);
            L0 += 0.00000012757 * Math.Cos(0.04509743861 + 11.0457002639 * t);
            L0 += 0.00000011988 * Math.Cos(4.81687553351 + 24.1183899573 * t);
            L0 += 0.00000011060 * Math.Cos(1.78958277553 + 7.4223635415 * t);
            L0 += 0.00000012108 * Math.Cos(1.87022663714 + 79.2350166922 * t);
            L0 += 0.00000011698 * Math.Cos(0.49005698002 + 1.5963472929 * t);
            L0 += 0.00000010459 * Math.Cos(2.38743199893 + 381.3516082374 * t);
            L0 += 0.00000011681 * Math.Cos(3.85151357766 + 218.4069048687 * t);
            L0 += 0.00000008744 * Math.Cos(0.14168568610 + 148.0787244263 * t);
            L0 += 0.00000009196 * Math.Cos(1.00274090619 + 72.0732855816 * t);
            L0 += 0.00000011343 * Math.Cos(0.81432278263 + 525.4981794006 * t);
            L0 += 0.00000010097 * Math.Cos(5.03383557061 + 601.7642506762 * t);
            L0 += 0.00000008035 * Math.Cos(1.77685723010 + 0.2124483211 * t);
            L0 += 0.00000008382 * Math.Cos(3.07534786987 + 1.2720243872 * t);
            L0 += 0.00000010803 * Math.Cos(2.92081211459 + 293.188503436 * t);
            L0 += 0.00000007666 * Math.Cos(1.52223325105 + 115.8835796217 * t);
            L0 += 0.00000007531 * Math.Cos(5.37537256533 + 5.1078094307 * t);
            L0 += 0.00000008691 * Math.Cos(4.74352784364 + 143.6253063014 * t);
            L0 += 0.00000010183 * Math.Cos(1.15395455831 + 6244.9428143536 * t);
            L0 += 0.00000008283 * Math.Cos(0.35956716764 + 138.5174968707 * t);
            L0 += 0.00000009544 * Math.Cos(4.02452832984 + 152.5321425512 * t);
            L0 += 0.00000007274 * Math.Cos(4.10937535938 + 251.4321310758 * t);
            L0 += 0.00000007465 * Math.Cos(1.72131945843 + 31.019488637 * t);
            L0 += 0.00000006902 * Math.Cos(4.62452068308 + 2.7083129857 * t);
            L0 += 0.00000007094 * Math.Cos(5.11528393609 + 312.1990839626 * t);
            L0 += 0.00000007929 * Math.Cos(2.10765101655 + 27.0873353739 * t);
            L0 += 0.00000006156 * Math.Cos(3.50746507109 + 28.5718080822 * t);
            L0 += 0.00000007134 * Math.Cos(2.05292376023 + 278.2588340188 * t);
            L0 += 0.00000008193 * Math.Cos(2.58588219154 + 141.2258098564 * t);
            L0 += 0.00000005499 * Math.Cos(2.09250039025 + 1.6969210294 * t);
            L0 += 0.00000005279 * Math.Cos(4.09390686798 + 983.1158589136 * t);
            L0 += 0.00000006947 * Math.Cos(3.48041784595 + 415.2918581812 * t);
            L0 += 0.00000005916 * Math.Cos(0.68957324226 + 62.2514255951 * t);
            L0 += 0.00000005925 * Math.Cos(4.02504592620 + 255.0554677982 * t);
            L0 += 0.00000004606 * Math.Cos(1.17779101436 + 43.2408450685 * t);
            L0 += 0.00000005357 * Math.Cos(3.63061058987 + 5.4166259714 * t);
            L0 += 0.00000005918 * Math.Cos(2.57693824084 + 10175.1525105732 * t);
            L0 += 0.00000005482 * Math.Cos(3.07979737280 + 329.8370663655 * t);
            L0 += 0.00000003956 * Math.Cos(5.00418696742 + 184.7272873558 * t);
            L0 += 0.00000005408 * Math.Cos(3.31313295602 + 528.2064923863 * t);
            L0 += 0.00000004767 * Math.Cos(4.91981150665 + 456.3938392356 * t);
            L0 += 0.00000003770 * Math.Cos(1.57277409442 + 32.7164096664 * t);
            L0 += 0.00000003924 * Math.Cos(4.92763242635 + 180.2738692309 * t);
            L0 += 0.00000003707 * Math.Cos(4.82965453201 + 221.3758502853 * t);
            L0 += 0.00000003802 * Math.Cos(4.96279204998 + 594.6507036754 * t);
            L0 += 0.00000004014 * Math.Cos(1.63905164030 + 40.5807161926 * t);
            L0 += 0.00000003061 * Math.Cos(0.39713858313 + 1.4362885985 * t);
            L0 += 0.00000003261 * Math.Cos(4.65478978469 + 29.226199388 * t);
            L0 += 0.00000003474 * Math.Cos(5.65891305944 + 395.578702239 * t);
            L0 += 0.00000002918 * Math.Cos(5.91079083895 + 1.2238402774 * t);
            L0 += 0.00000003649 * Math.Cos(3.88114678609 + 494.5268748734 * t);
            L0 += 0.00000003225 * Math.Cos(5.57423738665 + 1014.1353475506 * t);
            L0 += 0.00000002845 * Math.Cos(0.56009386585 + 144.1465711632 * t);
            L0 += 0.00000002848 * Math.Cos(0.55423029727 + 567.8240007324 * t);
            L0 += 0.00000003440 * Math.Cos(1.70887250883 + 12.5301729722 * t);
            L0 += 0.00000003267 * Math.Cos(5.63287799820 + 488.5889840402 * t);
            L0 += 0.00000003107 * Math.Cos(5.79335949207 + 105.4922706636 * t);
            L0 += 0.00000002712 * Math.Cos(2.43726364359 + 60.7669528868 * t);
            L0 += 0.00000003202 * Math.Cos(2.21483496593 + 41.0537969446 * t);
            L0 += 0.00000003134 * Math.Cos(4.69665220513 + 82.8583534146 * t);
            L0 += 0.00000003590 * Math.Cos(5.69939670162 + 1124.34166877 * t);
            L0 += 0.00000002967 * Math.Cos(0.54448940101 + 135.5485514541 * t);
            L0 += 0.00000003211 * Math.Cos(4.19927605853 + 291.7040307277 * t);
            L0 += 0.00000002899 * Math.Cos(5.99669788291 + 22.633917249 * t);
            L0 += 0.00000003143 * Math.Cos(2.93495725805 + 31.2319369581 * t);
            L0 += 0.00000002729 * Math.Cos(4.62707721219 + 5.6290742925 * t);
            L0 += 0.00000002513 * Math.Cos(5.60391563025 + 19.1224551112 * t);
            L0 += 0.00000002690 * Math.Cos(5.32070128202 + 2.0057375701 * t);
            L0 += 0.00000002630 * Math.Cos(6.00855841124 + 37.1698277913 * t);
            L0 += 0.00000002296 * Math.Cos(6.06934502789 + 451.9404211107 * t);
            L0 += 0.00000002858 * Math.Cos(4.88677262419 + 258.0244132148 * t);
            L0 += 0.00000002879 * Math.Cos(5.12239168488 + 38.6543004996 * t);
            L0 += 0.00000002270 * Math.Cos(2.08634524182 + 30.0562807905 * t);
            L0 += 0.00000002301 * Math.Cos(3.35951602914 + 1028.3624415522 * t);
            L0 += 0.00000003001 * Math.Cos(3.59143817947 + 211.8146227297 * t);
            L0 += 0.00000002237 * Math.Cos(0.38455553470 + 3.6233367224 * t);
            L0 += 0.00000002901 * Math.Cos(3.24755614136 + 366.485629295 * t);
            L0 += 0.00000002592 * Math.Cos(1.36262641469 + 35.4247226521 * t);
            L0 += 0.00000002418 * Math.Cos(4.93467056526 + 47.6942631934 * t);
            L0 += 0.00000002089 * Math.Cos(5.79838063413 + 4.665866446 * t);
            L0 += 0.00000002586 * Math.Cos(2.69392971321 + 38.1812197476 * t);
            L0 += 0.00000001913 * Math.Cos(5.53560681085 + 149.5631971346 * t);
            L0 += 0.00000001971 * Math.Cos(6.00790964671 + 34.2008823747 * t);
            L0 += 0.00000002586 * Math.Cos(6.24984047544 + 38.084851528 * t);
            L0 += 0.00000002098 * Math.Cos(4.57819744766 + 1019.7644218431 * t);
            L0 += 0.00000001869 * Math.Cos(3.85907708723 + 911.042573332 * t);
            L0 += 0.00000002486 * Math.Cos(5.21235809332 + 140.001969579 * t);
            L0 += 0.00000001795 * Math.Cos(1.68012868451 + 1059.3819301892 * t);
            L0 += 0.00000002326 * Math.Cos(2.82664069146 + 807.9497991134 * t);
            L0 += 0.00000001984 * Math.Cos(5.54763522932 + 1022.7333672597 * t);
            L0 += 0.00000001919 * Math.Cos(5.10717766499 + 216.9224321604 * t);
            L0 += 0.00000002004 * Math.Cos(5.47811228948 + 63.7358983034 * t);
            L0 += 0.00000002021 * Math.Cos(4.15631916516 + 178.1350052168 * t);
            L0 += 0.00000001760 * Math.Cos(6.00927149342 + 172.1971143836 * t);
            L0 += 0.00000002140 * Math.Cos(2.65037925793 + 700.6642392008 * t);
            L0 += 0.00000001988 * Math.Cos(3.35850272780 + 186.2117600641 * t);
            L0 += 0.00000001956 * Math.Cos(5.01527508588 + 294.6729761443 * t);
            L0 += 0.00000001966 * Math.Cos(4.07957525462 + 20.6069278195 * t);
            L0 += 0.00000001637 * Math.Cos(0.53823942149 + 67.3592350258 * t);
            L0 += 0.00000001540 * Math.Cos(2.62327849119 + 41.7563723602 * t);
            L0 += 0.00000001810 * Math.Cos(5.81430038477 + 129.9194771616 * t);
            L0 += 0.00000001776 * Math.Cos(4.37047808449 + 328.3525936572 * t);
            L0 += 0.00000001460 * Math.Cos(2.63664516309 + 2.857070832 * t);
            L0 += 0.00000001388 * Math.Cos(2.10598045632 + 3.9321532631 * t);
            L0 += 0.00000001352 * Math.Cos(0.55618245459 + 0.6543913058 * t);
            L0 += 0.00000001668 * Math.Cos(2.77543377384 + 16.1535096946 * t);
            L0 += 0.00000001338 * Math.Cos(0.37643611305 + 14.0146456805 * t);
            L0 += 0.00000001218 * Math.Cos(0.73456434750 + 426.598190876 * t);
            L0 += 0.00000001531 * Math.Cos(4.54891769768 + 526.722019678 * t);
            L0 += 0.00000001610 * Math.Cos(3.40993944436 + 403.1341922245 * t);
            L0 += 0.00000001361 * Math.Cos(4.48227243414 + 17.6379824029 * t);
            L0 += 0.00000001589 * Math.Cos(5.59323020112 + 3302.479391062 * t);
            L0 += 0.00000001132 * Math.Cos(5.64520725360 + 151.0476698429 * t);
            L0 += 0.00000001357 * Math.Cos(4.06399031430 + 26.826702943 * t);
            L0 += 0.00000001494 * Math.Cos(4.98692049495 + 666.723989257 * t);
            L0 += 0.00000001077 * Math.Cos(4.30911470250 + 0.6331394464 * t);
            L0 += 0.00000001042 * Math.Cos(6.02756893581 + 106.0135355254 * t);
            L0 += 0.00000001060 * Math.Cos(0.74679491358 + 487.3651437628 * t);
            L0 += 0.00000001310 * Math.Cos(3.78526380930 + 386.9806825299 * t);
            L0 += 0.00000001342 * Math.Cos(4.52685061062 + 563.6312150384 * t);
            L0 += 0.00000000986 * Math.Cos(0.00600924269 + 81.3738807063 * t);
            L0 += 0.00000001232 * Math.Cos(5.17443930901 + 331.3215390738 * t);
            L0 += 0.00000000929 * Math.Cos(4.51267465978 + 38.3936680687 * t);
            L0 += 0.00000000956 * Math.Cos(3.50447791020 + 64.9597385808 * t);
            L0 += 0.00000000929 * Math.Cos(4.43109514438 + 37.8724032069 * t);
            L0 += 0.00000000926 * Math.Cos(6.09803297747 + 4.1446015842 * t);
            L0 += 0.00000000972 * Math.Cos(0.59038366513 + 8.9068362498 * t);
            L0 += 0.00000001246 * Math.Cos(4.69840351226 + 389.9496279465 * t);
            L0 += 0.00000001009 * Math.Cos(5.98451242784 + 142.1408335931 * t);
            L0 += 0.00000001020 * Math.Cos(0.83233892300 + 39.3568759152 * t);
            L0 += 0.00000001013 * Math.Cos(0.37845630298 + 36.9091953604 * t);
            L0 += 0.00000000940 * Math.Cos(2.42688145966 + 343.2185725996 * t);
            L0 += 0.00000000974 * Math.Cos(5.23958752786 + 253.5709950899 * t);
            L0 += 0.00000000964 * Math.Cos(5.09748190218 + 357.4456666012 * t);
            L0 += 0.00000000835 * Math.Cos(1.45568626670 + 35.212274331 * t);
            L0 += 0.00000001077 * Math.Cos(0.71409061316 + 44.070926471 * t);
            L0 += 0.00000001083 * Math.Cos(2.27578897621 + 6.9010986797 * t);
            L0 += 0.00000000938 * Math.Cos(5.03471583911 + 69.3649725959 * t);
            L0 += 0.00000001078 * Math.Cos(1.20253141912 + 35.685355083 * t);
            L0 += 0.00000001027 * Math.Cos(0.18243183397 + 84.3428261229 * t);
            L0 += 0.00000000764 * Math.Cos(4.62720907712 + 0.8300814025 * t);
            L0 += 0.00000001013 * Math.Cos(0.42234855022 + 32.4557772355 * t);
            L0 += 0.00000000939 * Math.Cos(4.50445799766 + 365.0011565867 * t);
            L0 += 0.00000000756 * Math.Cos(0.82872484717 + 17.5261078183 * t);
            L0 += 0.00000000916 * Math.Cos(3.89409205418 + 38.2449102224 * t);
            L0 += 0.00000000736 * Math.Cos(4.78125743795 + 5.3684418616 * t);
            L0 += 0.00000000762 * Math.Cos(0.01897337130 + 189.3931538018 * t);
            L0 += 0.00000000738 * Math.Cos(2.31770478416 + 42.3258213318 * t);
            L0 += 0.00000000860 * Math.Cos(4.82440483506 + 210.3301500214 * t);
            L0 += 0.00000000888 * Math.Cos(3.20360339895 + 348.8476468921 * t);
            L0 += 0.00000000916 * Math.Cos(5.04967792934 + 38.0211610532 * t);
            L0 += 0.00000000638 * Math.Cos(0.63267396269 + 244.318584075 * t);
            L0 += 0.00000000636 * Math.Cos(1.02615137352 + 2080.6308247406 * t);
            L0 += 0.00000000774 * Math.Cos(5.44432678139 + 367.9701020033 * t);
            L0 += 0.00000000644 * Math.Cos(1.94044989547 + 446.3113468182 * t);
            L0 += 0.00000000631 * Math.Cos(4.82928491724 + 460.5384408198 * t);
            L0 += 0.00000000855 * Math.Cos(3.57592750113 + 439.782755154 * t);
            L0 += 0.00000000678 * Math.Cos(4.48687912809 + 351.8165923087 * t);
            L0 += 0.00000000724 * Math.Cos(4.89141609280 + 119.5069163441 * t);
            L0 += 0.00000000594 * Math.Cos(0.59315717529 + 491.036664595 * t);
            L0 += 0.00000000655 * Math.Cos(1.99014093000 + 19.0105805266 * t);
            L0 += 0.00000000580 * Math.Cos(2.57189536188 + 492.0791943186 * t);
            L0 += 0.00000000694 * Math.Cos(0.08328521209 + 5.6772584023 * t);
            L0 += 0.00000000733 * Math.Cos(5.81485239057 + 29.7474642498 * t);
            L0 += 0.00000000666 * Math.Cos(3.42196897591 + 179.0982130633 * t);
            L0 += 0.00000000678 * Math.Cos(0.29428615814 + 171.2339065371 * t);
            L0 += 0.00000000635 * Math.Cos(2.13805182663 + 164.1203595363 * t);
            L0 += 0.00000000623 * Math.Cos(5.61454940380 + 285.3723810196 * t);
            L0 += 0.00000000529 * Math.Cos(1.88063108785 + 416.7763308895 * t);
            L0 += 0.00000000529 * Math.Cos(5.13250788030 + 697.743477894 * t);
            L0 += 0.00000000500 * Math.Cos(1.49548514415 + 704.8570248948 * t);
            L0 += 0.00000000487 * Math.Cos(4.97772067947 + 274.0660483248 * t);
            L0 += 0.00000000666 * Math.Cos(6.26456825266 + 1474.6737883704 * t);
            L0 += 0.00000000532 * Math.Cos(0.25784352716 + 477.3308354552 * t);
            L0 += 0.00000000557 * Math.Cos(0.71378452161 + 80.7194894005 * t);
            L0 += 0.00000000556 * Math.Cos(2.60791360513 + 418.2608035978 * t);
            L0 += 0.00000000584 * Math.Cos(4.29064541383 + 16.6747745564 * t);
            L0 += 0.00000000524 * Math.Cos(5.42759392280 + 290.2195580194 * t);
            L0 += 0.00000000524 * Math.Cos(0.29054995359 + 247.2393453818 * t);
            L0 += 0.00000000541 * Math.Cos(4.36400580938 + 815.0633461142 * t);
            L0 += 0.00000000526 * Math.Cos(1.66512720297 + 97.4155158163 * t);
            L0 += 0.00000000497 * Math.Cos(4.72640318293 + 401.6497195162 * t);
            L0 += 0.00000000432 * Math.Cos(2.98481475894 + 100.3844612329 * t);
            L0 += 0.00000000382 * Math.Cos(0.28067758468 + 8.385571388 * t);
            L0 += 0.00000000424 * Math.Cos(6.16774845481 + 178.7893965226 * t);
            L0 += 0.00000000484 * Math.Cos(0.01535318279 + 738.7972748386 * t);
            L0 += 0.00000000518 * Math.Cos(4.48916591410 + 875.830299001 * t);
            L0 += 0.00000000506 * Math.Cos(5.38611121207 + 404.6186649328 * t);
            L0 += 0.00000000396 * Math.Cos(4.62747640832 + 6.1503391543 * t);
            L0 += 0.00000000466 * Math.Cos(0.23340415764 + 120.9913890524 * t);
            L0 += 0.00000000409 * Math.Cos(3.08849480895 + 59.2824801785 * t);
            L0 += 0.00000000470 * Math.Cos(5.01853200224 + 313.6835566709 * t);
            L0 += 0.00000000442 * Math.Cos(3.68919475089 + 457.8783119439 * t);
            L0 += 0.00000000384 * Math.Cos(3.69499925394 + 160.9389657986 * t);
            L0 += 0.00000000364 * Math.Cos(0.76192181046 + 104.0077979553 * t);
            L0 += 0.00000000416 * Math.Cos(0.26652109651 + 103.0927742186 * t);
            L0 += 0.00000000401 * Math.Cos(4.06530055968 + 14.6690369863 * t);
            L0 += 0.00000000454 * Math.Cos(3.72767803715 + 476.4313180835 * t);
            L0 += 0.00000000434 * Math.Cos(0.33533802200 + 984.6003316219 * t);
            L0 += 0.00000000340 * Math.Cos(0.99915726716 + 31.5407534988 * t);
            L0 += 0.00000000420 * Math.Cos(3.65147769268 + 20.4950532349 * t);
            L0 += 0.00000000334 * Math.Cos(0.35121412008 + 1227.4344429886 * t);
            L0 += 0.00000000323 * Math.Cos(5.45836731979 + 918.1561203328 * t);
            L0 += 0.00000000407 * Math.Cos(4.19457842203 + 309.7995875176 * t);
            L0 += 0.00000000381 * Math.Cos(0.01364856960 + 495.4900827199 * t);
            L0 += 0.00000000334 * Math.Cos(4.05924071124 + 8.3373872782 * t);
            L0 += 0.00000000380 * Math.Cos(3.17063415023 + 487.6257761937 * t);
            L0 += 0.00000000309 * Math.Cos(0.48352303405 + 118.0224436358 * t);
            L0 += 0.00000000380 * Math.Cos(2.70238752925 + 134.1122628556 * t);
            L0 += 0.00000000362 * Math.Cos(4.88985810610 + 438.2982824457 * t);
            L0 += 0.00000000327 * Math.Cos(2.91090790412 + 505.7850234584 * t);
            L0 += 0.00000000308 * Math.Cos(0.96082817124 + 21.1494445407 * t);
            L0 += 0.00000000288 * Math.Cos(1.48123872077 + 220.4126424388 * t);
            L0 += 0.00000000293 * Math.Cos(2.56582281789 + 662.531203563 * t);
            L0 += 0.00000000331 * Math.Cos(4.37715965811 + 180.7951340927 * t);
            L0 += 0.00000000326 * Math.Cos(2.46104924164 + 169.5369855077 * t);
            L0 += 0.00000000289 * Math.Cos(2.63591886391 + 55.7710180407 * t);
            L0 += 0.00000000288 * Math.Cos(5.02487283285 + 1440.7335384266 * t);
            L0 += 0.00000000344 * Math.Cos(1.48930997270 + 166.5680400911 * t);
            L0 += 0.00000000266 * Math.Cos(0.63672427386 + 79.1868325824 * t);
            L0 += 0.00000000268 * Math.Cos(5.02354540478 + 377.4194549743 * t);
            L0 += 0.00000000308 * Math.Cos(1.50185265748 + 77.2292791221 * t);
            L0 += 0.00000000324 * Math.Cos(5.30240189273 + 457.617679513 * t);
            L0 += 0.00000000265 * Math.Cos(1.08736632800 + 450.4559484024 * t);
            L0 += 0.00000000264 * Math.Cos(0.83337660655 + 488.3765357191 * t);
            L0 += 0.00000000290 * Math.Cos(1.80003152563 + 101.8689339412 * t);
            L0 += 0.00000000262 * Math.Cos(2.30390003360 + 494.7393231945 * t);
            L0 += 0.00000000325 * Math.Cos(5.52669889053 + 441.2672278623 * t);
            L0 += 0.00000000254 * Math.Cos(0.02963623277 + 117.36805233 * t);
            L0 += 0.00000000300 * Math.Cos(0.17435705540 + 252.9166037841 * t);
            L0 += 0.00000000315 * Math.Cos(5.34885013040 + 183.7640795093 * t);
            L0 += 0.00000000313 * Math.Cos(5.45945846595 + 13.4933808187 * t);
            L0 += 0.00000000306 * Math.Cos(5.23085809622 + 45.2465826386 * t);
            L0 += 0.00000000237 * Math.Cos(0.32676889138 + 208.8456773131 * t);
            L0 += 0.00000000263 * Math.Cos(2.66670785888 + 464.7312265138 * t);
            L0 += 0.00000000234 * Math.Cos(1.82700149824 + 52175.8062831484 * t);
            L0 += 0.00000000275 * Math.Cos(5.04385701142 + 156.1554792736 * t);
            L0 += 0.00000000265 * Math.Cos(5.64967127743 + 326.8681209489 * t);
            L0 += 0.00000000247 * Math.Cos(1.74540930625 + 65.8747623175 * t);
            L0 += 0.00000000269 * Math.Cos(6.09827783249 + 1654.0326338646 * t);
            L0 += 0.00000000229 * Math.Cos(2.25832077914 + 190.665178189 * t);
            L0 += 0.00000000294 * Math.Cos(5.45249564193 + 206.1855484372 * t);
            L0 += 0.00000000238 * Math.Cos(1.55647021369 + 79.889407998 * t);
            L0 += 0.00000000230 * Math.Cos(6.13158632762 + 178.3474535379 * t);
            L0 += 0.00000000274 * Math.Cos(4.10829870815 + 518.3846323998 * t);
            L0 += 0.00000000225 * Math.Cos(3.86300359251 + 171.9846660625 * t);
            L0 += 0.00000000228 * Math.Cos(2.48511565618 + 12566.1516999828 * t);
            L0 += 0.00000000272 * Math.Cos(5.61149862463 + 148.3393568572 * t);
            L0 += 0.00000000214 * Math.Cos(1.45987216039 + 522.5774180938 * t);
            L0 += 0.00000000211 * Math.Cos(4.04791980901 + 6205.3253060075 * t);
            L0 += 0.00000000266 * Math.Cos(0.99036038827 + 209.106309744 * t);
            L0 += 0.00000000230 * Math.Cos(0.54049951530 + 532.6117264014 * t);
            L0 += 0.00000000226 * Math.Cos(3.84152961620 + 283.6272758804 * t);
            L0 += 0.00000000243 * Math.Cos(5.32730346969 + 485.9288551643 * t);
            L0 += 0.00000000209 * Math.Cos(4.35051470487 + 536.8045120954 * t);
            L0 += 0.00000000232 * Math.Cos(3.01948719112 + 10.9338256793 * t);
            L0 += 0.00000000264 * Math.Cos(5.70536379124 + 490.3340891794 * t);
            L0 += 0.00000000280 * Math.Cos(3.99993658196 + 674.8007441043 * t);
            L0 += 0.00000000246 * Math.Cos(0.37698964335 + 157.6399519819 * t);
            L0 += 0.00000000219 * Math.Cos(5.67679857772 + 52099.5402118728 * t);
            L0 += 0.00000000251 * Math.Cos(1.52353965506 + 6.8529145699 * t);
            L0 += 0.00000000203 * Math.Cos(5.44328656642 + 145.6310438715 * t);
            L0 += 0.00000000238 * Math.Cos(0.96169723853 + 497.1870037493 * t);
            L0 += 0.00000000219 * Math.Cos(4.52300776062 + 1615.8995982268 * t);
            L0 += 0.00000000275 * Math.Cos(2.37619210741 + 2118.7638603784 * t);
            L0 += 0.00000000258 * Math.Cos(5.12448148780 + 608.877797677 * t);
            L0 += 0.00000000260 * Math.Cos(3.88543008475 + 513.079881013 * t);
            L0 += 0.00000000191 * Math.Cos(3.72574595369 + 65.2203710117 * t);
            L0 += 0.00000000211 * Math.Cos(0.06484535455 + 215.4379594521 * t);
            L0 += 0.00000000236 * Math.Cos(3.95835282821 + 141.4864422873 * t);
            L0 += 0.00000000189 * Math.Cos(5.28135043909 + 377.1588225434 * t);
            L0 += 0.00000000243 * Math.Cos(4.35559878377 + 482.9599097477 * t);
            L0 += 0.00000000243 * Math.Cos(6.06808644973 + 154.0166152595 * t);
            L0 += 0.00000000249 * Math.Cos(1.57215637373 + 14.2270940016 * t);
            L0 += 0.00000000238 * Math.Cos(1.93340192445 + 500.1559491659 * t);
            L0 += 0.00000000209 * Math.Cos(5.02893682321 + 364.559213602 * t);
            L0 += 0.00000000227 * Math.Cos(5.72984298540 + 1543.8263126452 * t);
            L0 += 0.00000000217 * Math.Cos(2.45036922991 + 187.1749679106 * t);
            L0 += 0.00000000181 * Math.Cos(1.65699502247 + 1627.2059309216 * t);
            L0 += 0.00000000214 * Math.Cos(1.60213179145 + 11.3063326948 * t);
            L0 += 0.00000000203 * Math.Cos(0.74638490279 + 14.5571624017 * t);
            L0 += 0.00000000192 * Math.Cos(3.17719161639 + 343.4792050305 * t);
            L0 += 0.00000000177 * Math.Cos(1.50027795761 + 9.449352971 * t);
            L0 += 0.00000000177 * Math.Cos(0.03038098292 + 165.6048322446 * t);
            L0 += 0.00000000176 * Math.Cos(4.64462444674 + 315.1680293792 * t);
            L0 += 0.00000000208 * Math.Cos(2.65835778368 + 496.0113475817 * t);
            L0 += 0.00000000174 * Math.Cos(2.76155855705 + 49.1787359017 * t);
            L0 += 0.00000000196 * Math.Cos(1.95549714182 + 335.7749571987 * t);
            L0 += 0.00000000200 * Math.Cos(4.16839394758 + 285.1117485887 * t);
            L0 += 0.00000000199 * Math.Cos(0.06168021293 + 73.5577582899 * t);
            L0 += 0.00000000188 * Math.Cos(6.17288913873 + 535.3200393871 * t);
            L0 += 0.00000000215 * Math.Cos(1.92414563346 + 552.6973893591 * t);
            L0 += 0.00000000166 * Math.Cos(5.49038139690 + 10135.5350022271 * t);
            L0 += 0.00000000192 * Math.Cos(0.96973434120 + 304.2342036999 * t);
            L0 += 0.00000000209 * Math.Cos(5.34065233845 + 13.642138665 * t);
            L0 += 0.00000000203 * Math.Cos(5.11234865419 + 324.7292569348 * t);
            L0 += 0.00000000177 * Math.Cos(3.50680841790 + 207.3612046048 * t);
            L0 += 0.00000000174 * Math.Cos(1.95010708561 + 319.3126309634 * t);
            L0 += 0.00000000187 * Math.Cos(5.57685931698 + 266.1011680621 * t);
            L0 += 0.00000000181 * Math.Cos(1.43525075751 + 279.7433067271 * t);
            L0 += 0.00000000165 * Math.Cos(4.00537112057 + 493.5636670269 * t);
            L0 += 0.00000000191 * Math.Cos(1.68313683465 + 563.3705826075 * t);
            L0 += 0.00000000173 * Math.Cos(3.93200456456 + 238.9019581036 * t);
            L0 += 0.00000000161 * Math.Cos(5.96143146317 + 36.1272980677 * t);
            L0 += 0.00000000194 * Math.Cos(2.37664231450 + 944.9828232758 * t);
            L0 += 0.00000000165 * Math.Cos(0.97421918976 + 556.5176680376 * t);
            L0 += 0.00000000189 * Math.Cos(1.11279570541 + 1127.2624300768 * t);
            L0 += 0.00000000172 * Math.Cos(0.75085513952 + 267.5856407704 * t);
            L0 += 0.00000000193 * Math.Cos(2.12636756833 + 20350.3050211464 * t);
            L0 += 0.00000000181 * Math.Cos(2.10814562080 + 113.8778420516 * t);
            L0 += 0.00000000194 * Math.Cos(1.13504964219 + 57.255490749 * t);
            L0 += 0.00000000181 * Math.Cos(6.23699820519 + 355.9611938929 * t);
            L0 += 0.00000000198 * Math.Cos(5.68125942959 + 6280.1069045748 * t);
            L0 += 0.00000000173 * Math.Cos(5.15083799917 + 474.9468453752 * t);
            L0 += 0.00000000151 * Math.Cos(1.66981962338 + 116.5379709275 * t);
            L0 += 0.00000000150 * Math.Cos(5.42593657173 + 526.9826521089 * t);
            L0 += 0.00000000205 * Math.Cos(4.16096717573 + 711.4493070338 * t);
            L0 += 0.00000000177 * Math.Cos(3.49360697678 + 421.2297490144 * t);
            L0 += 0.00000000168 * Math.Cos(0.52839230204 + 487.1045113319 * t);
            L0 += 0.00000000160 * Math.Cos(4.77712663799 + 524.0137066923 * t);
            L0 += 0.00000000145 * Math.Cos(2.81448128781 + 1512.8068240082 * t);
            L0 += 0.00000000146 * Math.Cos(4.99570112660 + 142.6620984549 * t);
            L0 += 0.00000000188 * Math.Cos(0.82104161550 + 10210.3166007944 * t);
            L0 += 0.00000000145 * Math.Cos(4.96888131586 + 1189.3014073508 * t);
            L0 += 0.00000000181 * Math.Cos(2.99704790590 + 75.7448064138 * t);
            L0 += 0.00000000176 * Math.Cos(0.41626373842 + 222.8603229936 * t);
            L0 += 0.00000000137 * Math.Cos(2.96534226337 + 6206.8097787158 * t);
            L0 += 0.00000000138 * Math.Cos(1.22260849471 + 187.6962327724 * t);
            L0 += 0.00000000128 * Math.Cos(2.53394068407 + 276.7743613105 * t);
            L0 += 0.00000000130 * Math.Cos(3.04810765699 + 310.7146112543 * t);
            L0 += 0.00000000122 * Math.Cos(3.01323006886 + 70.8494453042 * t);
            L0 += 0.00000000111 * Math.Cos(0.77449448649 + 179.3588454942 * t);
            L0 += 0.00000000141 * Math.Cos(0.18423889807 + 131.4039498699 * t);
            L0 += 0.00000000126 * Math.Cos(5.77648809669 + 525.2375469697 * t);
            L0 += 0.00000000124 * Math.Cos(2.93225731024 + 179.6194779251 * t);
            L0 += 0.00000000111 * Math.Cos(6.18471578216 + 981.6313862053 * t);
            L0 += 0.00000000141 * Math.Cos(2.63342951123 + 381.6122406683 * t);
            L0 += 0.00000000110 * Math.Cos(5.25053027081 + 986.0848043302 * t);
            L0 += 0.00000000096 * Math.Cos(3.86591534559 + 240.125798381 * t);
            L0 += 0.00000000120 * Math.Cos(3.78755085035 + 1057.8974574809 * t);
            L0 += 0.00000000093 * Math.Cos(4.54014016637 + 36.6967470393 * t);
            L0 += 0.00000000109 * Math.Cos(1.53327585900 + 419.7452763061 * t);
            L0 += 0.00000000094 * Math.Cos(4.21870300178 + 1024.217839968 * t);
            L0 += 0.00000000109 * Math.Cos(2.15905156247 + 289.5651667136 * t);
            L0 += 0.00000000104 * Math.Cos(0.20665642552 + 564.8550553158 * t);
            L0 += 0.00000000081 * Math.Cos(1.89134135215 + 36.6003788197 * t);
            L0 += 0.00000000080 * Math.Cos(4.38832594589 + 10137.0194749354 * t);
            L0 += 0.00000000080 * Math.Cos(1.73940577376 + 39.5056337615 * t);
            L0 += 0.00000000084 * Math.Cos(0.81316746605 + 170.7126416753 * t);
            L0 += 0.00000000090 * Math.Cos(0.60145818457 + 36.7604375141 * t);
            L0 += 0.00000000074 * Math.Cos(4.92511651321 + 1549.4553869377 * t);
            L0 += 0.00000000072 * Math.Cos(5.06852406179 + 249.9476583675 * t);
            return L0;
        }

        internal static double Neptune_L1(double t) // 183 terms of order 1
        {
            double L1 = 0;
            L1 += 38.37687716731;
            L1 += 0.00016604187 * Math.Cos(4.86319129565 + 1.4844727083 * t);
            L1 += 0.00015807148 * Math.Cos(2.27923488532 + 38.1330356378 * t);
            L1 += 0.00003334701 * Math.Cos(3.68199676020 + 76.2660712756 * t);
            L1 += 0.00001305840 * Math.Cos(3.67320813491 + 2.9689454166 * t);
            L1 += 0.00000604832 * Math.Cos(1.50477747549 + 35.1640902212 * t);
            L1 += 0.00000178623 * Math.Cos(3.45318524147 + 39.6175083461 * t);
            L1 += 0.00000106537 * Math.Cos(2.45126138334 + 4.4534181249 * t);
            L1 += 0.00000105747 * Math.Cos(2.75479326550 + 33.6796175129 * t);
            L1 += 0.00000072684 * Math.Cos(5.48724732699 + 36.6485629295 * t);
            L1 += 0.00000057069 * Math.Cos(5.21649804970 + 0.5212648618 * t);
            L1 += 0.00000057355 * Math.Cos(1.85767603384 + 114.3991069134 * t);
            L1 += 0.00000035368 * Math.Cos(4.51676827545 + 74.7815985673 * t);
            L1 += 0.00000032216 * Math.Cos(5.90411489680 + 77.7505439839 * t);
            L1 += 0.00000029871 * Math.Cos(3.67043294114 + 388.4651552382 * t);
            L1 += 0.00000028866 * Math.Cos(5.16877529164 + 9.5612275556 * t);
            L1 += 0.00000028742 * Math.Cos(5.16732589024 + 2.4476805548 * t);
            L1 += 0.00000025507 * Math.Cos(5.24526281928 + 168.0525127994 * t);
            L1 += 0.00000024869 * Math.Cos(4.73193067810 + 182.279606801 * t);
            L1 += 0.00000020205 * Math.Cos(5.78945415677 + 1021.2488945514 * t);
            L1 += 0.00000019022 * Math.Cos(1.82981144269 + 484.444382456 * t);
            L1 += 0.00000018661 * Math.Cos(1.31606255521 + 498.6714764576 * t);
            L1 += 0.00000015063 * Math.Cos(4.95003893760 + 137.0330241624 * t);
            L1 += 0.00000015094 * Math.Cos(3.98705254940 + 32.1951448046 * t);
            L1 += 0.00000010720 * Math.Cos(2.44148149225 + 4.192785694 * t);
            L1 += 0.00000011725 * Math.Cos(4.89255650674 + 71.8126531507 * t);
            L1 += 0.00000009581 * Math.Cos(1.23188039594 + 5.9378908332 * t);
            L1 += 0.00000009606 * Math.Cos(1.88534821556 + 41.1019810544 * t);
            L1 += 0.00000008968 * Math.Cos(0.01758559103 + 8.0767548473 * t);
            L1 += 0.00000009882 * Math.Cos(6.08165628679 + 7.1135470008 * t);
            L1 += 0.00000007632 * Math.Cos(5.51307048241 + 73.297125859 * t);
            L1 += 0.00000006992 * Math.Cos(0.61688864282 + 2.9207613068 * t);
            L1 += 0.00000005543 * Math.Cos(2.24141557794 + 46.2097904851 * t);
            L1 += 0.00000004845 * Math.Cos(3.71055823750 + 112.9146342051 * t);
            L1 += 0.00000003700 * Math.Cos(5.25713252333 + 111.4301614968 * t);
            L1 += 0.00000003233 * Math.Cos(6.10303038418 + 70.3281804424 * t);
            L1 += 0.00000002939 * Math.Cos(4.86520586648 + 98.8999885246 * t);
            L1 += 0.00000002403 * Math.Cos(2.90637675099 + 601.7642506762 * t);
            L1 += 0.00000002398 * Math.Cos(1.04343654629 + 6.592282139 * t);
            L1 += 0.00000002784 * Math.Cos(4.95821114677 + 108.4612160802 * t);
            L1 += 0.00000002894 * Math.Cos(4.20148844767 + 381.3516082374 * t);
            L1 += 0.00000002111 * Math.Cos(5.93089610785 + 25.6028626656 * t);
            L1 += 0.00000002075 * Math.Cos(5.20632201951 + 30.7106720963 * t);
            L1 += 0.00000002126 * Math.Cos(0.54976393136 + 41.0537969446 * t);
            L1 += 0.00000002235 * Math.Cos(2.38045158073 + 453.424893819 * t);
            L1 += 0.00000001859 * Math.Cos(0.89409373259 + 24.1183899573 * t);
            L1 += 0.00000002018 * Math.Cos(3.42245274178 + 31.019488637 * t);
            L1 += 0.00000001700 * Math.Cos(3.91715254287 + 11.0457002639 * t);
            L1 += 0.00000001776 * Math.Cos(3.86571077241 + 395.578702239 * t);
            L1 += 0.00000001644 * Math.Cos(0.15855999051 + 152.5321425512 * t);
            L1 += 0.00000001646 * Math.Cos(3.34591387314 + 44.7253177768 * t);
            L1 += 0.00000001876 * Math.Cos(2.59784179105 + 33.9402499438 * t);
            L1 += 0.00000001614 * Math.Cos(0.42137145545 + 175.1660598002 * t);
            L1 += 0.00000001468 * Math.Cos(6.12983933526 + 1550.939859646 * t);
            L1 += 0.00000001408 * Math.Cos(6.13722948564 + 490.0734567485 * t);
            L1 += 0.00000001207 * Math.Cos(0.59525736062 + 312.1990839626 * t);
            L1 += 0.00000001336 * Math.Cos(3.28611928206 + 493.0424021651 * t);
            L1 += 0.00000001176 * Math.Cos(5.87266726996 + 5.4166259714 * t);
            L1 += 0.00000001517 * Math.Cos(3.12967210501 + 491.5579294568 * t);
            L1 += 0.00000001053 * Math.Cos(4.60375516830 + 79.2350166922 * t);
            L1 += 0.00000001037 * Math.Cos(4.89007314395 + 1.2720243872 * t);
            L1 += 0.00000001034 * Math.Cos(5.93741289103 + 32.7164096664 * t);
            L1 += 0.00000001038 * Math.Cos(1.13470380744 + 1014.1353475506 * t);
            L1 += 0.00000001002 * Math.Cos(1.85850922283 + 5.1078094307 * t);
            L1 += 0.00000000983 * Math.Cos(0.05345050384 + 7.4223635415 * t);
            L1 += 0.00000000998 * Math.Cos(1.73689827444 + 1028.3624415522 * t);
            L1 += 0.00000001193 * Math.Cos(4.63176675581 + 60.7669528868 * t);
            L1 += 0.00000000940 * Math.Cos(3.09103721222 + 62.2514255951 * t);
            L1 += 0.00000000994 * Math.Cos(4.11489180313 + 4.665866446 * t);
            L1 += 0.00000000890 * Math.Cos(0.87049255398 + 31.2319369581 * t);
            L1 += 0.00000000852 * Math.Cos(5.35508394316 + 144.1465711632 * t);
            L1 += 0.00000000922 * Math.Cos(5.12373360511 + 145.1097790097 * t);
            L1 += 0.00000000789 * Math.Cos(0.37496785039 + 26.826702943 * t);
            L1 += 0.00000000828 * Math.Cos(4.06035194600 + 115.8835796217 * t);
            L1 += 0.00000000711 * Math.Cos(3.14189997439 + 278.2588340188 * t);
            L1 += 0.00000000727 * Math.Cos(1.39718382835 + 213.299095438 * t);
            L1 += 0.00000000781 * Math.Cos(0.10946327923 + 173.6815870919 * t);
            L1 += 0.00000000793 * Math.Cos(6.13086312116 + 567.8240007324 * t);
            L1 += 0.00000000669 * Math.Cos(4.50554989443 + 27.0873353739 * t);
            L1 += 0.00000000825 * Math.Cos(1.35568908148 + 129.9194771616 * t);
            L1 += 0.00000000738 * Math.Cos(3.56766018960 + 176.6505325085 * t);
            L1 += 0.00000000714 * Math.Cos(6.24797992301 + 106.9767433719 * t);
            L1 += 0.00000000654 * Math.Cos(1.13177751192 + 68.8437077341 * t);
            L1 += 0.00000000624 * Math.Cos(0.01567750666 + 28.5718080822 * t);
            L1 += 0.00000000608 * Math.Cos(4.60180625368 + 189.3931538018 * t);
            L1 += 0.00000000595 * Math.Cos(0.00857468445 + 42.5864537627 * t);
            L1 += 0.00000000530 * Math.Cos(5.61201247153 + 12.5301729722 * t);
            L1 += 0.00000000521 * Math.Cos(1.02371768017 + 415.2918581812 * t);
            L1 += 0.00000000639 * Math.Cos(0.68930265745 + 529.6909650946 * t);
            L1 += 0.00000000526 * Math.Cos(3.02138731705 + 5.6290742925 * t);
            L1 += 0.00000000456 * Math.Cos(4.44331571392 + 43.2408450685 * t);
            L1 += 0.00000000524 * Math.Cos(3.43316448349 + 38.6543004996 * t);
            L1 += 0.00000000436 * Math.Cos(2.41630174435 + 82.8583534146 * t);
            L1 += 0.00000000424 * Math.Cos(1.95736011325 + 477.3308354552 * t);
            L1 += 0.00000000443 * Math.Cos(3.39350946329 + 357.4456666012 * t);
            L1 += 0.00000000383 * Math.Cos(1.90232196422 + 22.633917249 * t);
            L1 += 0.00000000479 * Math.Cos(5.55141744216 + 37.611770776 * t);
            L1 += 0.00000000462 * Math.Cos(3.80436154644 + 343.2185725996 * t);
            L1 += 0.00000000384 * Math.Cos(5.60377408953 + 594.6507036754 * t);
            L1 += 0.00000000369 * Math.Cos(4.45577410338 + 6.9010986797 * t);
            L1 += 0.00000000358 * Math.Cos(3.69126616347 + 3.9321532631 * t);
            L1 += 0.00000000352 * Math.Cos(3.10952926034 + 135.5485514541 * t);
            L1 += 0.00000000368 * Math.Cos(3.53577440355 + 40.5807161926 * t);
            L1 += 0.00000000424 * Math.Cos(5.27159202779 + 181.7583419392 * t);
            L1 += 0.00000000361 * Math.Cos(0.29018303419 + 72.0732855816 * t);
            L1 += 0.00000000390 * Math.Cos(5.49512204296 + 350.3321196004 * t);
            L1 += 0.00000000378 * Math.Cos(2.74122401337 + 488.3765357191 * t);
            L1 += 0.00000000372 * Math.Cos(0.39980033572 + 494.7393231945 * t);
            L1 += 0.00000000353 * Math.Cos(1.10614174053 + 20.6069278195 * t);
            L1 += 0.00000000296 * Math.Cos(0.86351261285 + 149.5631971346 * t);
            L1 += 0.00000000307 * Math.Cos(5.39420288683 + 160.9389657986 * t);
            L1 += 0.00000000395 * Math.Cos(1.93577214824 + 10137.0194749354 * t);
            L1 += 0.00000000288 * Math.Cos(2.28755739359 + 47.6942631934 * t);
            L1 += 0.00000000295 * Math.Cos(2.48737537240 + 19.1224551112 * t);
            L1 += 0.00000000290 * Math.Cos(0.18636083306 + 143.6253063014 * t);
            L1 += 0.00000000266 * Math.Cos(3.09977370364 + 69.3649725959 * t);
            L1 += 0.00000000266 * Math.Cos(1.21002824826 + 505.7850234584 * t);
            L1 += 0.00000000252 * Math.Cos(3.12745026026 + 460.5384408198 * t);
            L1 += 0.00000000328 * Math.Cos(0.50849285663 + 6206.8097787158 * t);
            L1 += 0.00000000257 * Math.Cos(3.64119914774 + 446.3113468182 * t);
            L1 += 0.00000000239 * Math.Cos(5.54080102299 + 911.042573332 * t);
            L1 += 0.00000000265 * Math.Cos(0.62702473701 + 253.5709950899 * t);
            L1 += 0.00000000287 * Math.Cos(2.44403568436 + 16.6747745564 * t);
            L1 += 0.00000000231 * Math.Cos(2.47026250085 + 454.9093665273 * t);
            L1 += 0.00000000230 * Math.Cos(3.24571542922 + 1066.49547719 * t);
            L1 += 0.00000000282 * Math.Cos(1.48595620175 + 983.1158589136 * t);
            L1 += 0.00000000212 * Math.Cos(5.41931177641 + 64.9597385808 * t);
            L1 += 0.00000000213 * Math.Cos(1.64175339637 + 1089.129394439 * t);
            L1 += 0.00000000238 * Math.Cos(2.69801319489 + 882.9438460018 * t);
            L1 += 0.00000000210 * Math.Cos(4.53976756699 + 1093.322180133 * t);
            L1 += 0.00000000220 * Math.Cos(2.30038816175 + 1052.2683831884 * t);
            L1 += 0.00000000256 * Math.Cos(0.42073598460 + 23.9059416362 * t);
            L1 += 0.00000000216 * Math.Cos(5.44225918870 + 39.0962434843 * t);
            L1 += 0.00000000201 * Math.Cos(2.58746514605 + 119.5069163441 * t);
            L1 += 0.00000000224 * Math.Cos(4.43751392203 + 639.897286314 * t);
            L1 += 0.00000000186 * Math.Cos(2.50651218075 + 487.3651437628 * t);
            L1 += 0.00000000189 * Math.Cos(4.05785534221 + 120.9913890524 * t);
            L1 += 0.00000000184 * Math.Cos(2.24245977278 + 815.0633461142 * t);
            L1 += 0.00000000202 * Math.Cos(3.43517732411 + 45.2465826386 * t);
            L1 += 0.00000000175 * Math.Cos(4.49165234532 + 171.2339065371 * t);
            L1 += 0.00000000171 * Math.Cos(5.50633466316 + 179.0982130633 * t);
            L1 += 0.00000000200 * Math.Cos(6.12663205401 + 14.2270940016 * t);
            L1 += 0.00000000173 * Math.Cos(2.61090344107 + 389.9496279465 * t);
            L1 += 0.00000000167 * Math.Cos(3.94754384833 + 77.2292791221 * t);
            L1 += 0.00000000166 * Math.Cos(3.41009128748 + 81.3738807063 * t);
            L1 += 0.00000000163 * Math.Cos(3.88198848446 + 556.5176680376 * t);
            L1 += 0.00000000164 * Math.Cos(1.49614763046 + 63.7358983034 * t);
            L1 += 0.00000000176 * Math.Cos(3.86129425367 + 148.3393568572 * t);
            L1 += 0.00000000161 * Math.Cos(2.22215642318 + 574.9375477332 * t);
            L1 += 0.00000000171 * Math.Cos(0.66899426684 + 179.3106613844 * t);
            L1 += 0.00000000161 * Math.Cos(1.21480182441 + 1024.4302882891 * t);
            L1 += 0.00000000155 * Math.Cos(3.25842414799 + 10251.4185818488 * t);
            L1 += 0.00000000183 * Math.Cos(5.45168150656 + 218.4069048687 * t);
            L1 += 0.00000000152 * Math.Cos(3.35145509017 + 285.3723810196 * t);
            L1 += 0.00000000152 * Math.Cos(0.42398786475 + 274.0660483248 * t);
            L1 += 0.00000000146 * Math.Cos(5.70714579127 + 419.4846438752 * t);
            L1 += 0.00000000156 * Math.Cos(0.64321524870 + 1029.8469142605 * t);
            L1 += 0.00000000147 * Math.Cos(4.30958930740 + 157.6399519819 * t);
            L1 += 0.00000000147 * Math.Cos(1.80689177510 + 377.4194549743 * t);
            L1 += 0.00000000140 * Math.Cos(1.49826604627 + 386.9806825299 * t);
            L1 += 0.00000000137 * Math.Cos(2.14480243915 + 563.6312150384 * t);
            L1 += 0.00000000127 * Math.Cos(3.98726599710 + 84.3428261229 * t);
            L1 += 0.00000000134 * Math.Cos(4.16039455079 + 169.5369855077 * t);
            L1 += 0.00000000121 * Math.Cos(0.29300927469 + 206.1855484372 * t);
            L1 += 0.00000000129 * Math.Cos(2.67625057010 + 180.7951340927 * t);
            L1 += 0.00000000134 * Math.Cos(3.18868986487 + 166.5680400911 * t);
            L1 += 0.00000000135 * Math.Cos(5.07517561780 + 426.598190876 * t);
            L1 += 0.00000000136 * Math.Cos(1.81672451740 + 151.0476698429 * t);
            L1 += 0.00000000129 * Math.Cos(3.64795525602 + 183.7640795093 * t);
            L1 += 0.00000000116 * Math.Cos(6.06435563172 + 220.4126424388 * t);
            L1 += 0.00000000123 * Math.Cos(4.46641157829 + 1022.7333672597 * t);
            L1 += 0.00000000112 * Math.Cos(4.34485256988 + 138.5174968707 * t);
            L1 += 0.00000000116 * Math.Cos(5.58946529961 + 35.685355083 * t);
            L1 += 0.00000000108 * Math.Cos(1.03796693383 + 488.5889840402 * t);
            L1 += 0.00000000108 * Math.Cos(2.10378485880 + 494.5268748734 * t);
            L1 += 0.00000000106 * Math.Cos(0.87068583107 + 1059.3819301892 * t);
            L1 += 0.00000000097 * Math.Cos(0.74486741478 + 485.9288551643 * t);
            L1 += 0.00000000095 * Math.Cos(5.54259914856 + 497.1870037493 * t);
            L1 += 0.00000000085 * Math.Cos(3.16062141266 + 522.5774180938 * t);
            L1 += 0.00000000097 * Math.Cos(6.05634803604 + 482.9599097477 * t);
            L1 += 0.00000000095 * Math.Cos(0.23111852730 + 500.1559491659 * t);
            L1 += 0.00000000084 * Math.Cos(2.64687252518 + 536.8045120954 * t);
            L1 += 0.00000000074 * Math.Cos(3.90678924318 + 1019.7644218431 * t);
            return L1 * t;
        }

        internal static double Neptune_L2(double t) // 57 terms of order 2
        {
            double L2 = 0;
            L2 += 0.00053892649;
            L2 += 0.00000281251 * Math.Cos(1.19084538887 + 38.1330356378 * t);
            L2 += 0.00000295693 * Math.Cos(1.85520292248 + 1.4844727083 * t);
            L2 += 0.00000270190 * Math.Cos(5.72143228148 + 76.2660712756 * t);
            L2 += 0.00000023023 * Math.Cos(1.21035596452 + 2.9689454166 * t);
            L2 += 0.00000007333 * Math.Cos(0.54033306830 + 2.4476805548 * t);
            L2 += 0.00000009057 * Math.Cos(4.42544992035 + 35.1640902212 * t);
            L2 += 0.00000005223 * Math.Cos(0.67427930044 + 168.0525127994 * t);
            L2 += 0.00000005201 * Math.Cos(3.02338671812 + 182.279606801 * t);
            L2 += 0.00000004288 * Math.Cos(3.84351844003 + 114.3991069134 * t);
            L2 += 0.00000003925 * Math.Cos(3.53214557374 + 484.444382456 * t);
            L2 += 0.00000003741 * Math.Cos(5.90238217874 + 498.6714764576 * t);
            L2 += 0.00000002966 * Math.Cos(0.31002477611 + 4.4534181249 * t);
            L2 += 0.00000003415 * Math.Cos(0.55971639038 + 74.7815985673 * t);
            L2 += 0.00000003255 * Math.Cos(1.84921884906 + 175.1660598002 * t);
            L2 += 0.00000002157 * Math.Cos(1.89135758747 + 388.4651552382 * t);
            L2 += 0.00000002211 * Math.Cos(4.37997092240 + 7.1135470008 * t);
            L2 += 0.00000001847 * Math.Cos(3.48574435762 + 9.5612275556 * t);
            L2 += 0.00000002451 * Math.Cos(4.68586840176 + 491.5579294568 * t);
            L2 += 0.00000001844 * Math.Cos(5.12281562096 + 33.6796175129 * t);
            L2 += 0.00000002204 * Math.Cos(1.69321574906 + 77.7505439839 * t);
            L2 += 0.00000001652 * Math.Cos(2.55859494053 + 36.6485629295 * t);
            L2 += 0.00000001309 * Math.Cos(4.52400192922 + 1021.2488945514 * t);
            L2 += 0.00000001124 * Math.Cos(0.38710602242 + 137.0330241624 * t);
            L2 += 0.00000000664 * Math.Cos(0.88101734307 + 4.192785694 * t);
            L2 += 0.00000000497 * Math.Cos(2.24615784762 + 395.578702239 * t);
            L2 += 0.00000000512 * Math.Cos(6.22609200672 + 381.3516082374 * t);
            L2 += 0.00000000582 * Math.Cos(5.25716719826 + 31.019488637 * t);
            L2 += 0.00000000446 * Math.Cos(0.36647221351 + 98.8999885246 * t);
            L2 += 0.00000000383 * Math.Cos(5.48585528762 + 5.9378908332 * t);
            L2 += 0.00000000375 * Math.Cos(4.61250246774 + 8.0767548473 * t);
            L2 += 0.00000000354 * Math.Cos(1.30783918287 + 601.7642506762 * t);
            L2 += 0.00000000259 * Math.Cos(5.66033623678 + 112.9146342051 * t);
            L2 += 0.00000000247 * Math.Cos(2.89695614593 + 189.3931538018 * t);
            L2 += 0.00000000245 * Math.Cos(4.26572913391 + 220.4126424388 * t);
            L2 += 0.00000000200 * Math.Cos(0.52604535784 + 64.9597385808 * t);
            L2 += 0.00000000191 * Math.Cos(4.88786653062 + 39.6175083461 * t);
            L2 += 0.00000000233 * Math.Cos(3.16423779113 + 41.1019810544 * t);
            L2 += 0.00000000248 * Math.Cos(5.85877831382 + 1059.3819301892 * t);
            L2 += 0.00000000194 * Math.Cos(2.37949641473 + 73.297125859 * t);
            L2 += 0.00000000227 * Math.Cos(0.20028518978 + 60.7669528868 * t);
            L2 += 0.00000000184 * Math.Cos(3.01962045713 + 1014.1353475506 * t);
            L2 += 0.00000000190 * Math.Cos(5.57500985081 + 343.2185725996 * t);
            L2 += 0.00000000172 * Math.Cos(3.66036463613 + 477.3308354552 * t);
            L2 += 0.00000000172 * Math.Cos(0.59550457102 + 46.2097904851 * t);
            L2 += 0.00000000182 * Math.Cos(1.92429384025 + 183.7640795093 * t);
            L2 += 0.00000000171 * Math.Cos(1.61368476689 + 357.4456666012 * t);
            L2 += 0.00000000173 * Math.Cos(6.23717119485 + 493.0424021651 * t);
            L2 += 0.00000000217 * Math.Cos(1.46218158211 + 71.8126531507 * t);
            L2 += 0.00000000178 * Math.Cos(0.34928799031 + 1028.3624415522 * t);
            L2 += 0.00000000169 * Math.Cos(4.91086673212 + 166.5680400911 * t);
            L2 += 0.00000000157 * Math.Cos(5.89200571154 + 169.5369855077 * t);
            L2 += 0.00000000182 * Math.Cos(2.33457064554 + 152.5321425512 * t);
            L2 += 0.00000000151 * Math.Cos(3.81621340568 + 146.594251718 * t);
            L2 += 0.00000000136 * Math.Cos(2.75150881988 + 144.1465711632 * t);
            L2 += 0.00000000104 * Math.Cos(6.03262825314 + 529.6909650946 * t);
            L2 += 0.00000000076 * Math.Cos(0.20932812381 + 453.424893819 * t);
            return L2 * t * t;
        }

        internal static double Neptune_L3(double t) // 15 terms of order 3
        {
            double L3 = 0;
            L3 += 0.00000031254;
            L3 += 0.00000012461 * Math.Cos(6.04431418812 + 1.4844727083 * t);
            L3 += 0.00000014541 * Math.Cos(1.35337075856 + 76.2660712756 * t);
            L3 += 0.00000011547 * Math.Cos(6.11257808366 + 38.1330356378 * t);
            L3 += 0.00000001351 * Math.Cos(4.93951495175 + 2.9689454166 * t);
            L3 += 0.00000000741 * Math.Cos(2.35936954597 + 168.0525127994 * t);
            L3 += 0.00000000715 * Math.Cos(1.27409542804 + 182.279606801 * t);
            L3 += 0.00000000537 * Math.Cos(5.23632185196 + 484.444382456 * t);
            L3 += 0.00000000523 * Math.Cos(4.16769839601 + 498.6714764576 * t);
            L3 += 0.00000000664 * Math.Cos(0.55871435877 + 31.019488637 * t);
            L3 += 0.00000000301 * Math.Cos(2.69253200796 + 7.1135470008 * t);
            L3 += 0.00000000194 * Math.Cos(2.05904114139 + 137.0330241624 * t);
            L3 += 0.00000000206 * Math.Cos(2.51012178002 + 74.7815985673 * t);
            L3 += 0.00000000160 * Math.Cos(5.63111039032 + 114.3991069134 * t);
            L3 += 0.00000000149 * Math.Cos(3.09327713923 + 35.1640902212 * t);
            return L3 * t * t * t;
        }

        internal static double Neptune_L4(double t) // 2 terms of order 4
        {
            double L4 = 0;
            L4 -= 0.00000113998;
            L4 += 0.00000000605 * Math.Cos(3.18211885677 + 76.2660712756 * t);
            return L4 * t * t * t * t;
        }

        internal static double Neptune_L5(double t) // 1 term of order 5
        {
            double L5 = 0;
            L5 -= 0.00000000874;
            return L5 * t * t * t * t * t;
        }

        internal static double Neptune_B0(double t) // 172 terms of order 0
        {
            double B0 = 0;
            B0 += 0.03088622933 * Math.Cos(1.44104372626 + 38.1330356378 * t);
            B0 += 0.00027780087 * Math.Cos(5.91271882843 + 76.2660712756 * t);
            B0 += 0.00027623609;
            B0 += 0.00015355490 * Math.Cos(2.52123799481 + 36.6485629295 * t);
            B0 += 0.00015448133 * Math.Cos(3.50877080888 + 39.6175083461 * t);
            B0 += 0.00001999919 * Math.Cos(1.50998669505 + 74.7815985673 * t);
            B0 += 0.00001967540 * Math.Cos(4.37778195768 + 1.4844727083 * t);
            B0 += 0.00001015137 * Math.Cos(3.21561035875 + 35.1640902212 * t);
            B0 += 0.00000605767 * Math.Cos(2.80246601405 + 73.297125859 * t);
            B0 += 0.00000594878 * Math.Cos(2.12892708114 + 41.1019810544 * t);
            B0 += 0.00000588805 * Math.Cos(3.18655882497 + 2.9689454166 * t);
            B0 += 0.00000401830 * Math.Cos(4.16883287237 + 114.3991069134 * t);
            B0 += 0.00000254333 * Math.Cos(3.27120499438 + 453.424893819 * t);
            B0 += 0.00000261647 * Math.Cos(3.76722704749 + 213.299095438 * t);
            B0 += 0.00000279964 * Math.Cos(1.68165309699 + 77.7505439839 * t);
            B0 += 0.00000205590 * Math.Cos(4.25652348864 + 529.6909650946 * t);
            B0 += 0.00000140455 * Math.Cos(3.52969556376 + 137.0330241624 * t);
            B0 += 0.00000098530 * Math.Cos(4.16774829927 + 33.6796175129 * t);
            B0 += 0.00000051257 * Math.Cos(1.95121181203 + 4.4534181249 * t);
            B0 += 0.00000067971 * Math.Cos(4.66970781659 + 71.8126531507 * t);
            B0 += 0.00000041931 * Math.Cos(5.41783694467 + 111.4301614968 * t);
            B0 += 0.00000041822 * Math.Cos(5.94832001477 + 112.9146342051 * t);
            B0 += 0.00000030637 * Math.Cos(0.93620571932 + 42.5864537627 * t);
            B0 += 0.00000011084 * Math.Cos(5.88898793049 + 108.4612160802 * t);
            B0 += 0.00000009620 * Math.Cos(0.03944255108 + 70.3281804424 * t);
            B0 += 0.00000009664 * Math.Cos(0.22455797403 + 79.2350166922 * t);
            B0 += 0.00000009728 * Math.Cos(5.30069593532 + 32.1951448046 * t);
            B0 += 0.00000007386 * Math.Cos(3.00684933642 + 426.598190876 * t);
            B0 += 0.00000007087 * Math.Cos(0.12535040656 + 109.9456887885 * t);
            B0 += 0.00000006021 * Math.Cos(6.20514068152 + 115.8835796217 * t);
            B0 += 0.00000006169 * Math.Cos(3.62098109648 + 983.1158589136 * t);
            B0 += 0.00000004777 * Math.Cos(0.75210194972 + 5.9378908332 * t);
            B0 += 0.00000006391 * Math.Cos(5.84646101060 + 148.0787244263 * t);
            B0 += 0.00000006251 * Math.Cos(2.41678769385 + 152.5321425512 * t);
            B0 += 0.00000004539 * Math.Cos(5.58182098700 + 175.1660598002 * t);
            B0 += 0.00000005006 * Math.Cos(4.60815664851 + 1059.3819301892 * t);
            B0 += 0.00000004289 * Math.Cos(4.19647392821 + 47.6942631934 * t);
            B0 += 0.00000005795 * Math.Cos(5.07516716087 + 415.2918581812 * t);
            B0 += 0.00000004749 * Math.Cos(2.51605725604 + 37.611770776 * t);
            B0 += 0.00000004119 * Math.Cos(1.72779509865 + 28.5718080822 * t);
            B0 += 0.00000004076 * Math.Cos(6.00252170354 + 145.1097790097 * t);
            B0 += 0.00000004429 * Math.Cos(5.65995321659 + 98.8999885246 * t);
            B0 += 0.00000003950 * Math.Cos(2.74104636753 + 350.3321196004 * t);
            B0 += 0.00000004091 * Math.Cos(1.61787956945 + 39.0962434843 * t);
            B0 += 0.00000004131 * Math.Cos(4.40682554313 + 37.1698277913 * t);
            B0 += 0.00000004710 * Math.Cos(3.50929350767 + 38.6543004996 * t);
            B0 += 0.00000004440 * Math.Cos(4.78977105547 + 38.084851528 * t);
            B0 += 0.00000004433 * Math.Cos(1.23386935925 + 38.1812197476 * t);
            B0 += 0.00000003762 * Math.Cos(4.83940791709 + 491.5579294568 * t);
            B0 += 0.00000002606 * Math.Cos(1.20956732792 + 451.9404211107 * t);
            B0 += 0.00000002537 * Math.Cos(2.18628045751 + 454.9093665273 * t);
            B0 += 0.00000002328 * Math.Cos(5.19779918719 + 72.0732855816 * t);
            B0 += 0.00000002502 * Math.Cos(0.85987904350 + 106.9767433719 * t);
            B0 += 0.00000002342 * Math.Cos(0.81387240947 + 4.192785694 * t);
            B0 += 0.00000001981 * Math.Cos(0.46617960831 + 184.7272873558 * t);
            B0 += 0.00000001963 * Math.Cos(6.01909114576 + 44.070926471 * t);
            B0 += 0.00000002180 * Math.Cos(0.70099749844 + 206.1855484372 * t);
            B0 += 0.00000001811 * Math.Cos(0.40456996647 + 40.5807161926 * t);
            B0 += 0.00000001814 * Math.Cos(3.64699555185 + 220.4126424388 * t);
            B0 += 0.00000001705 * Math.Cos(6.13551142362 + 181.7583419392 * t);
            B0 += 0.00000001855 * Math.Cos(5.61635630213 + 35.685355083 * t);
            B0 += 0.00000001595 * Math.Cos(2.97147156093 + 37.8724032069 * t);
            B0 += 0.00000001785 * Math.Cos(2.42154818096 + 388.4651552382 * t);
            B0 += 0.00000001595 * Math.Cos(3.05266110075 + 38.3936680687 * t);
            B0 += 0.00000001437 * Math.Cos(1.48678704605 + 135.5485514541 * t);
            B0 += 0.00000001387 * Math.Cos(2.46149266117 + 138.5174968707 * t);
            B0 += 0.00000001366 * Math.Cos(1.52026779665 + 68.8437077341 * t);
            B0 += 0.00000001575 * Math.Cos(3.58964541604 + 38.0211610532 * t);
            B0 += 0.00000001297 * Math.Cos(5.06156596196 + 33.9402499438 * t);
            B0 += 0.00000001487 * Math.Cos(0.20211121607 + 30.0562807905 * t);
            B0 += 0.00000001504 * Math.Cos(5.80298577327 + 46.2097904851 * t);
            B0 += 0.00000001192 * Math.Cos(0.87275514483 + 42.3258213318 * t);
            B0 += 0.00000001569 * Math.Cos(2.43405967107 + 38.2449102224 * t);
            B0 += 0.00000001207 * Math.Cos(1.84658687853 + 251.4321310758 * t);
            B0 += 0.00000001015 * Math.Cos(0.53439848924 + 129.9194771616 * t);
            B0 += 0.00000000999 * Math.Cos(2.47463873948 + 312.1990839626 * t);
            B0 += 0.00000000990 * Math.Cos(3.41514319052 + 144.1465711632 * t);
            B0 += 0.00000000963 * Math.Cos(4.31733242907 + 151.0476698429 * t);
            B0 += 0.00000001020 * Math.Cos(0.98226686775 + 143.6253063014 * t);
            B0 += 0.00000000941 * Math.Cos(1.02993053785 + 221.3758502853 * t);
            B0 += 0.00000000938 * Math.Cos(2.43648356625 + 567.8240007324 * t);
            B0 += 0.00000001111 * Math.Cos(0.65175024456 + 146.594251718 * t);
            B0 += 0.00000000777 * Math.Cos(0.00175975222 + 218.4069048687 * t);
            B0 += 0.00000000895 * Math.Cos(0.25123869620 + 30.7106720963 * t);
            B0 += 0.00000000795 * Math.Cos(5.80519741659 + 149.5631971346 * t);
            B0 += 0.00000000737 * Math.Cos(3.40060492866 + 446.3113468182 * t);
            B0 += 0.00000000719 * Math.Cos(1.43795191278 + 8.0767548473 * t);
            B0 += 0.00000000720 * Math.Cos(0.00651007550 + 460.5384408198 * t);
            B0 += 0.00000000766 * Math.Cos(4.03399506246 + 522.5774180938 * t);
            B0 += 0.00000000666 * Math.Cos(1.39457824982 + 84.3428261229 * t);
            B0 += 0.00000000584 * Math.Cos(1.01405548136 + 536.8045120954 * t);
            B0 += 0.00000000596 * Math.Cos(0.62390100715 + 35.212274331 * t);
            B0 += 0.00000000598 * Math.Cos(5.39946724188 + 41.0537969446 * t);
            B0 += 0.00000000475 * Math.Cos(5.80072248338 + 7.4223635415 * t);
            B0 += 0.00000000510 * Math.Cos(1.34478579740 + 258.0244132148 * t);
            B0 += 0.00000000458 * Math.Cos(5.25325523118 + 80.7194894005 * t);
            B0 += 0.00000000421 * Math.Cos(3.24496387889 + 416.7763308895 * t);
            B0 += 0.00000000446 * Math.Cos(1.19167306357 + 180.2738692309 * t);
            B0 += 0.00000000471 * Math.Cos(0.92632922375 + 44.7253177768 * t);
            B0 += 0.00000000387 * Math.Cos(1.68488418788 + 183.2428146475 * t);
            B0 += 0.00000000375 * Math.Cos(0.15223869165 + 255.0554677982 * t);
            B0 += 0.00000000354 * Math.Cos(4.21526988674 + 0.9632078465 * t);
            B0 += 0.00000000379 * Math.Cos(2.16947487177 + 105.4922706636 * t);
            B0 += 0.00000000341 * Math.Cos(4.79194051680 + 110.2063212194 * t);
            B0 += 0.00000000427 * Math.Cos(5.15774894584 + 31.5407534988 * t);
            B0 += 0.00000000302 * Math.Cos(3.45706306280 + 100.3844612329 * t);
            B0 += 0.00000000298 * Math.Cos(2.26790695187 + 639.897286314 * t);
            B0 += 0.00000000279 * Math.Cos(0.25689162963 + 39.5056337615 * t);
            B0 += 0.00000000320 * Math.Cos(3.58085653166 + 45.2465826386 * t);
            B0 += 0.00000000269 * Math.Cos(5.72024180826 + 36.7604375141 * t);
            B0 += 0.00000000247 * Math.Cos(0.61040148804 + 186.2117600641 * t);
            B0 += 0.00000000245 * Math.Cos(0.64173616273 + 419.4846438752 * t);
            B0 += 0.00000000235 * Math.Cos(0.73189197665 + 10213.285546211 * t);
            B0 += 0.00000000232 * Math.Cos(0.37399822852 + 490.0734567485 * t);
            B0 += 0.00000000230 * Math.Cos(5.76570492457 + 12.5301729722 * t);
            B0 += 0.00000000240 * Math.Cos(4.13447692727 + 0.5212648618 * t);
            B0 += 0.00000000279 * Math.Cos(1.62614865256 + 294.6729761443 * t);
            B0 += 0.00000000238 * Math.Cos(2.18528916550 + 219.891377577 * t);
            B0 += 0.00000000262 * Math.Cos(3.08384135298 + 6.592282139 * t);
            B0 += 0.00000000217 * Math.Cos(2.93214905312 + 27.0873353739 * t);
            B0 += 0.00000000217 * Math.Cos(4.69210602828 + 406.1031376411 * t);
            B0 += 0.00000000219 * Math.Cos(1.35212712560 + 216.9224321604 * t);
            B0 += 0.00000000200 * Math.Cos(2.35215465744 + 605.9570363702 * t);
            B0 += 0.00000000232 * Math.Cos(3.92583619589 + 1512.8068240082 * t);
            B0 += 0.00000000223 * Math.Cos(5.52392277606 + 187.6962327724 * t);
            B0 += 0.00000000190 * Math.Cos(0.29169556516 + 291.7040307277 * t);
            B0 += 0.00000000236 * Math.Cos(3.12464145036 + 563.6312150384 * t);
            B0 += 0.00000000193 * Math.Cos(0.53675942386 + 60.7669528868 * t);
            B0 += 0.00000000215 * Math.Cos(3.78391259001 + 103.0927742186 * t);
            B0 += 0.00000000172 * Math.Cos(5.63262770743 + 7.1135470008 * t);
            B0 += 0.00000000164 * Math.Cos(4.14700645532 + 77.2292791221 * t);
            B0 += 0.00000000162 * Math.Cos(0.72021213236 + 11.0457002639 * t);
            B0 += 0.00000000160 * Math.Cos(4.23490438166 + 487.3651437628 * t);
            B0 += 0.00000000191 * Math.Cos(0.37651439206 + 31.019488637 * t);
            B0 += 0.00000000157 * Math.Cos(1.02419759383 + 6283.0758499914 * t);
            B0 += 0.00000000157 * Math.Cos(4.42530429545 + 6206.8097787158 * t);
            B0 += 0.00000000178 * Math.Cos(6.24797160202 + 316.3918696566 * t);
            B0 += 0.00000000161 * Math.Cos(5.65988283675 + 343.2185725996 * t);
            B0 += 0.00000000153 * Math.Cos(5.58405022784 + 252.0865223816 * t);
            B0 += 0.00000000189 * Math.Cos(4.80791039970 + 641.1211265914 * t);
            B0 += 0.00000000166 * Math.Cos(5.50438043692 + 662.531203563 * t);
            B0 += 0.00000000146 * Math.Cos(5.08949604858 + 286.596221297 * t);
            B0 += 0.00000000145 * Math.Cos(2.13015521881 + 2042.4977891028 * t);
            B0 += 0.00000000156 * Math.Cos(2.19452173251 + 274.0660483248 * t);
            B0 += 0.00000000148 * Math.Cos(4.85696640135 + 442.7517005706 * t);
            B0 += 0.00000000187 * Math.Cos(4.96121139073 + 1589.0728952838 * t);
            B0 += 0.00000000155 * Math.Cos(2.28260574227 + 142.1408335931 * t);
            B0 += 0.00000000134 * Math.Cos(1.29277093566 + 456.3938392356 * t);
            B0 += 0.00000000126 * Math.Cos(5.59769497652 + 179.3588454942 * t);
            B0 += 0.00000000146 * Math.Cos(2.53359213478 + 256.5399405065 * t);
            B0 += 0.00000000140 * Math.Cos(1.57962199954 + 75.7448064138 * t);
            B0 += 0.00000000123 * Math.Cos(0.05442220184 + 944.9828232758 * t);
            B0 += 0.00000000122 * Math.Cos(1.90676379802 + 418.2608035978 * t);
            B0 += 0.00000000154 * Math.Cos(1.86865302773 + 331.3215390738 * t);
            B0 += 0.00000000144 * Math.Cos(5.52229258454 + 14.0146456805 * t);
            B0 += 0.00000000138 * Math.Cos(2.80728175526 + 82.8583534146 * t);
            B0 += 0.00000000107 * Math.Cos(0.66995358132 + 190.665178189 * t);
            B0 += 0.00000000114 * Math.Cos(1.48894980280 + 253.5709950899 * t);
            B0 += 0.00000000110 * Math.Cos(5.32587573069 + 240.125798381 * t);
            B0 += 0.00000000105 * Math.Cos(0.65548440578 + 173.6815870919 * t);
            B0 += 0.00000000102 * Math.Cos(2.58735617801 + 450.4559484024 * t);
            B0 += 0.00000000098 * Math.Cos(0.44044795266 + 328.3525936572 * t);
            B0 += 0.00000000101 * Math.Cos(4.71267656829 + 117.36805233 * t);
            B0 += 0.00000000094 * Math.Cos(0.54938580474 + 293.188503436 * t);
            B0 += 0.00000000095 * Math.Cos(2.17636214523 + 101.8689339412 * t);
            B0 += 0.00000000093 * Math.Cos(0.63687810471 + 377.1588225434 * t);
            B0 += 0.00000000091 * Math.Cos(5.84828809934 + 10137.0194749354 * t);
            B0 += 0.00000000089 * Math.Cos(1.02830167997 + 1021.2488945514 * t);
            B0 += 0.00000000094 * Math.Cos(1.79320597168 + 493.0424021651 * t);
            B0 += 0.00000000080 * Math.Cos(1.58140274465 + 69.1525242748 * t);
            B0 += 0.00000000075 * Math.Cos(0.23453373368 + 63.7358983034 * t);
            B0 += 0.00000000071 * Math.Cos(1.51961989690 + 488.5889840402 * t);
            return B0;
        }

        internal static double Neptune_B1(double t) // 82 terms of order 1
        {
            double B1 = 0;
            B1 += 0.00227279214 * Math.Cos(3.80793089870 + 38.1330356378 * t);
            B1 += 0.00001803120 * Math.Cos(1.97576485377 + 76.2660712756 * t);
            B1 += 0.00001385733 * Math.Cos(4.82555548018 + 36.6485629295 * t);
            B1 -= 0.00001433300;
            B1 += 0.00001073298 * Math.Cos(6.08054240712 + 39.6175083461 * t);
            B1 += 0.00000147903 * Math.Cos(3.85766231348 + 74.7815985673 * t);
            B1 += 0.00000136448 * Math.Cos(0.47764957338 + 1.4844727083 * t);
            B1 += 0.00000070285 * Math.Cos(6.18782052139 + 35.1640902212 * t);
            B1 += 0.00000051899 * Math.Cos(5.05221791891 + 73.297125859 * t);
            B1 += 0.00000037273 * Math.Cos(4.89476629246 + 41.1019810544 * t);
            B1 += 0.00000042568 * Math.Cos(0.30721737205 + 114.3991069134 * t);
            B1 += 0.00000037104 * Math.Cos(5.75999349109 + 2.9689454166 * t);
            B1 += 0.00000026399 * Math.Cos(5.21566335936 + 213.299095438 * t);
            B1 += 0.00000016949 * Math.Cos(4.26463671859 + 77.7505439839 * t);
            B1 += 0.00000018747 * Math.Cos(0.90426522185 + 453.424893819 * t);
            B1 += 0.00000012951 * Math.Cos(6.17709713139 + 529.6909650946 * t);
            B1 += 0.00000010502 * Math.Cos(1.20336443465 + 137.0330241624 * t);
            B1 += 0.00000004416 * Math.Cos(1.25478204684 + 111.4301614968 * t);
            B1 += 0.00000004383 * Math.Cos(6.14147099615 + 71.8126531507 * t);
            B1 += 0.00000003694 * Math.Cos(0.94837702528 + 33.6796175129 * t);
            B1 += 0.00000002957 * Math.Cos(4.77532871210 + 4.4534181249 * t);
            B1 += 0.00000002698 * Math.Cos(1.92435531119 + 112.9146342051 * t);
            B1 += 0.00000001989 * Math.Cos(3.96637567224 + 42.5864537627 * t);
            B1 += 0.00000001150 * Math.Cos(4.30568700024 + 37.611770776 * t);
            B1 += 0.00000000871 * Math.Cos(4.81775882249 + 152.5321425512 * t);
            B1 += 0.00000000944 * Math.Cos(2.21777772050 + 109.9456887885 * t);
            B1 += 0.00000000936 * Math.Cos(1.17054983940 + 148.0787244263 * t);
            B1 += 0.00000000925 * Math.Cos(2.40329074000 + 206.1855484372 * t);
            B1 += 0.00000000690 * Math.Cos(1.57381082857 + 38.6543004996 * t);
            B1 += 0.00000000624 * Math.Cos(2.79466003645 + 79.2350166922 * t);
            B1 += 0.00000000726 * Math.Cos(4.13829519132 + 28.5718080822 * t);
            B1 += 0.00000000640 * Math.Cos(2.46161252327 + 115.8835796217 * t);
            B1 += 0.00000000531 * Math.Cos(2.96991530500 + 98.8999885246 * t);
            B1 += 0.00000000537 * Math.Cos(1.95986772922 + 220.4126424388 * t);
            B1 += 0.00000000539 * Math.Cos(2.06690307827 + 40.5807161926 * t);
            B1 += 0.00000000716 * Math.Cos(0.55781847010 + 350.3321196004 * t);
            B1 += 0.00000000563 * Math.Cos(1.84072805158 + 983.1158589136 * t);
            B1 += 0.00000000533 * Math.Cos(1.34787677940 + 47.6942631934 * t);
            B1 += 0.00000000566 * Math.Cos(1.80111775954 + 175.1660598002 * t);
            B1 += 0.00000000449 * Math.Cos(1.62191691011 + 144.1465711632 * t);
            B1 += 0.00000000371 * Math.Cos(2.74239666472 + 415.2918581812 * t);
            B1 += 0.00000000381 * Math.Cos(6.11910193382 + 426.598190876 * t);
            B1 += 0.00000000366 * Math.Cos(2.39752585360 + 129.9194771616 * t);
            B1 += 0.00000000456 * Math.Cos(3.19611413854 + 108.4612160802 * t);
            B1 += 0.00000000327 * Math.Cos(3.62341506247 + 38.1812197476 * t);
            B1 += 0.00000000328 * Math.Cos(0.89613145346 + 38.084851528 * t);
            B1 += 0.00000000341 * Math.Cos(3.87265469070 + 35.685355083 * t);
            B1 += 0.00000000331 * Math.Cos(4.48858774501 + 460.5384408198 * t);
            B1 += 0.00000000414 * Math.Cos(1.03543720726 + 70.3281804424 * t);
            B1 += 0.00000000310 * Math.Cos(0.51297445145 + 37.1698277913 * t);
            B1 += 0.00000000287 * Math.Cos(2.18351651800 + 491.5579294568 * t);
            B1 += 0.00000000274 * Math.Cos(6.11504724934 + 522.5774180938 * t);
            B1 += 0.00000000281 * Math.Cos(3.81657117512 + 5.9378908332 * t);
            B1 += 0.00000000298 * Math.Cos(4.00532631258 + 39.0962434843 * t);
            B1 += 0.00000000265 * Math.Cos(5.26569823181 + 446.3113468182 * t);
            B1 += 0.00000000319 * Math.Cos(1.34097217817 + 184.7272873558 * t);
            B1 += 0.00000000203 * Math.Cos(6.02944475303 + 149.5631971346 * t);
            B1 += 0.00000000205 * Math.Cos(5.53935732020 + 536.8045120954 * t);
            B1 += 0.00000000226 * Math.Cos(6.17710997862 + 454.9093665273 * t);
            B1 += 0.00000000186 * Math.Cos(3.24302117645 + 4.192785694 * t);
            B1 += 0.00000000179 * Math.Cos(4.91458426239 + 451.9404211107 * t);
            B1 += 0.00000000198 * Math.Cos(2.30775852880 + 146.594251718 * t);
            B1 += 0.00000000166 * Math.Cos(1.16793600058 + 72.0732855816 * t);
            B1 += 0.00000000147 * Math.Cos(2.10574339673 + 44.070926471 * t);
            B1 += 0.00000000123 * Math.Cos(1.98250467171 + 46.2097904851 * t);
            B1 += 0.00000000159 * Math.Cos(3.46955908364 + 145.1097790097 * t);
            B1 += 0.00000000116 * Math.Cos(5.88971113590 + 38.0211610532 * t);
            B1 += 0.00000000115 * Math.Cos(4.73412534395 + 38.2449102224 * t);
            B1 += 0.00000000125 * Math.Cos(3.42713474801 + 251.4321310758 * t);
            B1 += 0.00000000128 * Math.Cos(1.51108932026 + 221.3758502853 * t);
            B1 += 0.00000000127 * Math.Cos(0.17176461812 + 138.5174968707 * t);
            B1 += 0.00000000124 * Math.Cos(5.85160407534 + 1059.3819301892 * t);
            B1 += 0.00000000091 * Math.Cos(2.38273591235 + 30.0562807905 * t);
            B1 += 0.00000000118 * Math.Cos(5.27114846878 + 37.8724032069 * t);
            B1 += 0.00000000117 * Math.Cos(5.35267669439 + 38.3936680687 * t);
            B1 += 0.00000000099 * Math.Cos(5.19920708255 + 135.5485514541 * t);
            B1 += 0.00000000114 * Math.Cos(4.37452353441 + 388.4651552382 * t);
            B1 += 0.00000000093 * Math.Cos(4.64183693718 + 106.9767433719 * t);
            B1 += 0.00000000084 * Math.Cos(1.35269684746 + 33.9402499438 * t);
            B1 += 0.00000000111 * Math.Cos(3.56226463770 + 181.7583419392 * t);
            B1 += 0.00000000082 * Math.Cos(3.18401661435 + 42.3258213318 * t);
            B1 += 0.00000000084 * Math.Cos(5.51669920239 + 8.0767548473 * t);
            return B1 * t;
        }

        internal static double Neptune_B2(double t) // 25 terms of order 2
        {
            double B2 = 0;
            B2 += 0.00009690766 * Math.Cos(5.57123750291 + 38.1330356378 * t);
            B2 += 0.00000078815 * Math.Cos(3.62705474219 + 76.2660712756 * t);
            B2 += 0.00000071523 * Math.Cos(0.45476688580 + 36.6485629295 * t);
            B2 -= 0.00000058646;
            B2 += 0.00000029915 * Math.Cos(1.60671721861 + 39.6175083461 * t);
            B2 += 0.00000006472 * Math.Cos(5.60736756575 + 74.7815985673 * t);
            B2 += 0.00000005800 * Math.Cos(2.25341847151 + 1.4844727083 * t);
            B2 += 0.00000004309 * Math.Cos(1.68126737666 + 35.1640902212 * t);
            B2 += 0.00000003502 * Math.Cos(2.39142672984 + 114.3991069134 * t);
            B2 += 0.00000002649 * Math.Cos(0.65061457644 + 73.297125859 * t);
            B2 += 0.00000001518 * Math.Cos(0.37600329684 + 213.299095438 * t);
            B2 += 0.00000001223 * Math.Cos(1.23116043030 + 2.9689454166 * t);
            B2 += 0.00000000766 * Math.Cos(5.45279753249 + 453.424893819 * t);
            B2 += 0.00000000779 * Math.Cos(2.07081431472 + 529.6909650946 * t);
            B2 += 0.00000000496 * Math.Cos(0.26552533921 + 41.1019810544 * t);
            B2 += 0.00000000469 * Math.Cos(5.87866293959 + 77.7505439839 * t);
            B2 += 0.00000000482 * Math.Cos(5.63056237954 + 137.0330241624 * t);
            B2 += 0.00000000345 * Math.Cos(1.80085651594 + 71.8126531507 * t);
            B2 += 0.00000000274 * Math.Cos(2.86650141006 + 33.6796175129 * t);
            B2 += 0.00000000158 * Math.Cos(4.63868656467 + 206.1855484372 * t);
            B2 += 0.00000000166 * Math.Cos(1.24877330835 + 220.4126424388 * t);
            B2 += 0.00000000153 * Math.Cos(2.87376446497 + 111.4301614968 * t);
            B2 += 0.00000000116 * Math.Cos(3.63838544843 + 112.9146342051 * t);
            B2 += 0.00000000085 * Math.Cos(0.43712705655 + 4.4534181249 * t);
            B2 += 0.00000000104 * Math.Cos(6.12597614674 + 144.1465711632 * t);
            return B2 * t * t;
        }

        internal static double Neptune_B3(double t) // 9 terms of order 3
        {
            double B3 = 0;
            B3 += 0.00000273423 * Math.Cos(1.01688979072 + 38.1330356378 * t);
            B3 += 0.00000002274 * Math.Cos(2.36805657126 + 36.6485629295 * t);
            B3 += 0.00000002029 * Math.Cos(5.33364321342 + 76.2660712756 * t);
            B3 += 0.00000002393;
            B3 += 0.00000000538 * Math.Cos(3.21934211365 + 39.6175083461 * t);
            B3 += 0.00000000242 * Math.Cos(4.52650721578 + 114.3991069134 * t);
            B3 += 0.00000000185 * Math.Cos(1.04913770083 + 74.7815985673 * t);
            B3 += 0.00000000155 * Math.Cos(3.62376309338 + 35.1640902212 * t);
            B3 += 0.00000000157 * Math.Cos(3.94195369610 + 1.4844727083 * t);
            return B3 * t * t * t;
        }

        internal static double Neptune_B4(double t) // 1 term of order 4
        {
            double B4 = 0;
            B4 += 0.00000005728 * Math.Cos(2.66872693322 + 38.1330356378 * t);
            return B4 * t * t * t * t;
        }

        internal static double Neptune_B5(double t) // 1 term of order 5
        {
            double B5 = 0;
            B5 += 0.00000000113 * Math.Cos(4.70646877989 + 38.1330356378 * t);
            return B5 * t * t * t * t * t;
        }

        internal static double Neptune_R0(double t) // 607 terms of order 0
        {
            double R0 = 0;
            R0 += 30.07013206102;
            R0 += 0.27062259490 * Math.Cos(1.32999458930 + 38.1330356378 * t);
            R0 += 0.01691764281 * Math.Cos(3.25186138896 + 36.6485629295 * t);
            R0 += 0.00807830737 * Math.Cos(5.18592836167 + 1.4844727083 * t);
            R0 += 0.00537760613 * Math.Cos(4.52113902845 + 35.1640902212 * t);
            R0 += 0.00495725642 * Math.Cos(1.57105654815 + 491.5579294568 * t);
            R0 += 0.00274571970 * Math.Cos(1.84552256801 + 175.1660598002 * t);
            R0 += 0.00135134095 * Math.Cos(3.37220607384 + 39.6175083461 * t);
            R0 += 0.00121801825 * Math.Cos(5.79754444303 + 76.2660712756 * t);
            R0 += 0.00100895397 * Math.Cos(0.37702748681 + 73.297125859 * t);
            R0 += 0.00069791722 * Math.Cos(3.79617226928 + 2.9689454166 * t);
            R0 += 0.00046687838 * Math.Cos(5.74937810094 + 33.6796175129 * t);
            R0 += 0.00024593778 * Math.Cos(0.50801728204 + 109.9456887885 * t);
            R0 += 0.00016939242 * Math.Cos(1.59422166991 + 71.8126531507 * t);
            R0 += 0.00014229686 * Math.Cos(1.07786112902 + 74.7815985673 * t);
            R0 += 0.00012011825 * Math.Cos(1.92062131635 + 1021.2488945514 * t);
            R0 += 0.00008394731 * Math.Cos(0.67816895547 + 146.594251718 * t);
            R0 += 0.00007571800 * Math.Cos(1.07149263431 + 388.4651552382 * t);
            R0 += 0.00005720852 * Math.Cos(2.59059512267 + 4.4534181249 * t);
            R0 += 0.00004839672 * Math.Cos(1.90685991070 + 41.1019810544 * t);
            R0 += 0.00004483492 * Math.Cos(2.90573457534 + 529.6909650946 * t);
            R0 += 0.00004270202 * Math.Cos(3.41343865825 + 453.424893819 * t);
            R0 += 0.00004353790 * Math.Cos(0.67985662370 + 32.1951448046 * t);
            R0 += 0.00004420804 * Math.Cos(1.74993796503 + 108.4612160802 * t);
            R0 += 0.00002881063 * Math.Cos(1.98600105123 + 137.0330241624 * t);
            R0 += 0.00002635535 * Math.Cos(3.09755943422 + 213.299095438 * t);
            R0 += 0.00003380930 * Math.Cos(0.84810683275 + 183.2428146475 * t);
            R0 += 0.00002878942 * Math.Cos(3.67415901855 + 350.3321196004 * t);
            R0 += 0.00002306293 * Math.Cos(2.80962935724 + 70.3281804424 * t);
            R0 += 0.00002530149 * Math.Cos(5.79839567009 + 490.0734567485 * t);
            R0 += 0.00002523132 * Math.Cos(0.48630800015 + 493.0424021651 * t);
            R0 += 0.00002087303 * Math.Cos(0.61858378281 + 33.9402499438 * t);
            R0 += 0.00001976522 * Math.Cos(5.11703044560 + 168.0525127994 * t);
            R0 += 0.00001905254 * Math.Cos(1.72186472126 + 182.279606801 * t);
            R0 += 0.00001654039 * Math.Cos(1.92782545887 + 145.1097790097 * t);
            R0 += 0.00001435072 * Math.Cos(1.70005157785 + 484.444382456 * t);
            R0 += 0.00001403029 * Math.Cos(4.58914203187 + 498.6714764576 * t);
            R0 += 0.00001499193 * Math.Cos(1.01623299513 + 219.891377577 * t);
            R0 += 0.00001398860 * Math.Cos(0.76220317620 + 176.6505325085 * t);
            R0 += 0.00001403377 * Math.Cos(6.07659416908 + 173.6815870919 * t);
            R0 += 0.00001128560 * Math.Cos(5.96661179805 + 9.5612275556 * t);
            R0 += 0.00001228304 * Math.Cos(1.59881465324 + 77.7505439839 * t);
            R0 += 0.00000835414 * Math.Cos(3.97066884218 + 114.3991069134 * t);
            R0 += 0.00000811186 * Math.Cos(3.00258880870 + 46.2097904851 * t);
            R0 += 0.00000731925 * Math.Cos(2.10447054189 + 181.7583419392 * t);
            R0 += 0.00000615781 * Math.Cos(2.97874625677 + 106.9767433719 * t);
            R0 += 0.00000704778 * Math.Cos(1.18738210880 + 256.5399405065 * t);
            R0 += 0.00000502040 * Math.Cos(1.38657803368 + 5.9378908332 * t);
            R0 += 0.00000530357 * Math.Cos(4.24059166485 + 111.4301614968 * t);
            R0 += 0.00000437096 * Math.Cos(2.27029212923 + 1550.939859646 * t);
            R0 += 0.00000400250 * Math.Cos(1.25609325435 + 8.0767548473 * t);
            R0 += 0.00000421011 * Math.Cos(1.89084929506 + 30.7106720963 * t);
            R0 += 0.00000382457 * Math.Cos(3.29965259685 + 983.1158589136 * t);
            R0 += 0.00000422485 * Math.Cos(5.53186169605 + 525.4981794006 * t);
            R0 += 0.00000355389 * Math.Cos(2.27847846648 + 218.4069048687 * t);
            R0 += 0.00000280062 * Math.Cos(1.54129714238 + 98.8999885246 * t);
            R0 += 0.00000314499 * Math.Cos(3.95932948594 + 381.3516082374 * t);
            R0 += 0.00000280556 * Math.Cos(4.54238271682 + 44.7253177768 * t);
            R0 += 0.00000267738 * Math.Cos(5.13323364247 + 112.9146342051 * t);
            R0 += 0.00000333311 * Math.Cos(5.75067616021 + 39.0962434843 * t);
            R0 += 0.00000291625 * Math.Cos(4.02398326341 + 68.8437077341 * t);
            R0 += 0.00000321429 * Math.Cos(1.50625025822 + 454.9093665273 * t);
            R0 += 0.00000309196 * Math.Cos(2.85452752153 + 72.0732855816 * t);
            R0 += 0.00000345094 * Math.Cos(1.35905860594 + 293.188503436 * t);
            R0 += 0.00000307439 * Math.Cos(0.31964571332 + 601.7642506762 * t);
            R0 += 0.00000251356 * Math.Cos(3.53992782846 + 312.1990839626 * t);
            R0 += 0.00000248152 * Math.Cos(3.41078346726 + 37.611770776 * t);
            R0 += 0.00000306000 * Math.Cos(2.72475094464 + 6244.9428143536 * t);
            R0 += 0.00000293532 * Math.Cos(4.89079857814 + 528.2064923863 * t);
            R0 += 0.00000234479 * Math.Cos(0.59231043427 + 42.5864537627 * t);
            R0 += 0.00000239628 * Math.Cos(3.16441455173 + 143.6253063014 * t);
            R0 += 0.00000214523 * Math.Cos(3.62480283040 + 278.2588340188 * t);
            R0 += 0.00000246198 * Math.Cos(1.01506302015 + 141.2258098564 * t);
            R0 += 0.00000174089 * Math.Cos(5.55011789988 + 567.8240007324 * t);
            R0 += 0.00000163934 * Math.Cos(2.10166491786 + 2.4476805548 * t);
            R0 += 0.00000162897 * Math.Cos(2.48946521653 + 4.192785694 * t);
            R0 += 0.00000193455 * Math.Cos(1.58425287580 + 138.5174968707 * t);
            R0 += 0.00000155323 * Math.Cos(3.28425127954 + 31.019488637 * t);
            R0 += 0.00000182469 * Math.Cos(2.45244890571 + 255.0554677982 * t);
            R0 += 0.00000177846 * Math.Cos(4.14773474853 + 10175.1525105732 * t);
            R0 += 0.00000174413 * Math.Cos(1.53042999914 + 329.8370663655 * t);
            R0 += 0.00000137649 * Math.Cos(3.34900537767 + 0.9632078465 * t);
            R0 += 0.00000161011 * Math.Cos(5.16655038482 + 211.8146227297 * t);
            R0 += 0.00000113473 * Math.Cos(4.96286007991 + 148.0787244263 * t);
            R0 += 0.00000128823 * Math.Cos(3.25521535448 + 24.1183899573 * t);
            R0 += 0.00000107363 * Math.Cos(3.26457701792 + 1059.3819301892 * t);
            R0 += 0.00000122732 * Math.Cos(5.39399536941 + 62.2514255951 * t);
            R0 += 0.00000120529 * Math.Cos(3.08050145518 + 184.7272873558 * t);
            R0 += 0.00000099356 * Math.Cos(1.92888554099 + 28.5718080822 * t);
            R0 += 0.00000097713 * Math.Cos(2.59474415429 + 6.592282139 * t);
            R0 += 0.00000124095 * Math.Cos(3.11516750340 + 221.3758502853 * t);
            R0 += 0.00000124693 * Math.Cos(2.97042405451 + 251.4321310758 * t);
            R0 += 0.00000114252 * Math.Cos(0.25039919123 + 594.6507036754 * t);
            R0 += 0.00000111006 * Math.Cos(3.34276426767 + 180.2738692309 * t);
            R0 += 0.00000120939 * Math.Cos(1.92914010593 + 25.6028626656 * t);
            R0 += 0.00000104667 * Math.Cos(0.94883561775 + 395.578702239 * t);
            R0 += 0.00000109779 * Math.Cos(5.43147520571 + 494.5268748734 * t);
            R0 += 0.00000096919 * Math.Cos(0.86184760695 + 1014.1353475506 * t);
            R0 += 0.00000098685 * Math.Cos(0.89577952710 + 488.5889840402 * t);
            R0 += 0.00000088968 * Math.Cos(4.78109764779 + 144.1465711632 * t);
            R0 += 0.00000107888 * Math.Cos(0.98700578434 + 1124.34166877 * t);
            R0 += 0.00000097067 * Math.Cos(2.62667400276 + 291.7040307277 * t);
            R0 += 0.00000075131 * Math.Cos(5.88936524779 + 43.2408450685 * t);
            R0 += 0.00000093718 * Math.Cos(6.09873565184 + 526.722019678 * t);
            R0 += 0.00000094822 * Math.Cos(0.20662943940 + 456.3938392356 * t);
            R0 += 0.00000070036 * Math.Cos(2.39683345663 + 426.598190876 * t);
            R0 += 0.00000077187 * Math.Cos(4.21076753240 + 105.4922706636 * t);
            R0 += 0.00000089874 * Math.Cos(3.25100749923 + 258.0244132148 * t);
            R0 += 0.00000069133 * Math.Cos(4.93031154435 + 1028.3624415522 * t);
            R0 += 0.00000090657 * Math.Cos(1.69466970587 + 366.485629295 * t);
            R0 += 0.00000074242 * Math.Cos(3.14479101276 + 82.8583534146 * t);
            R0 += 0.00000057995 * Math.Cos(0.86159785905 + 60.7669528868 * t);
            R0 += 0.00000078695 * Math.Cos(1.09307575550 + 700.6642392008 * t);
            R0 += 0.00000057230 * Math.Cos(0.81331949225 + 2.9207613068 * t);
            R0 += 0.00000063443 * Math.Cos(4.39590123005 + 149.5631971346 * t);
            R0 += 0.00000055698 * Math.Cos(3.89047249911 + 47.6942631934 * t);
            R0 += 0.00000056430 * Math.Cos(5.15003563302 + 0.5212648618 * t);
            R0 += 0.00000056174 * Math.Cos(5.42986960794 + 911.042573332 * t);
            R0 += 0.00000061746 * Math.Cos(6.16453667559 + 1019.7644218431 * t);
            R0 += 0.00000070503 * Math.Cos(0.08077330612 + 40.5807161926 * t);
            R0 += 0.00000074677 * Math.Cos(4.85904499980 + 186.2117600641 * t);
            R0 += 0.00000061861 * Math.Cos(4.78702599861 + 11.0457002639 * t);
            R0 += 0.00000061135 * Math.Cos(0.83712253227 + 1022.7333672597 * t);
            R0 += 0.00000061268 * Math.Cos(5.70228826765 + 178.1350052168 * t);
            R0 += 0.00000052887 * Math.Cos(0.37458943972 + 27.0873353739 * t);
            R0 += 0.00000056722 * Math.Cos(3.52318112447 + 216.9224321604 * t);
            R0 += 0.00000048819 * Math.Cos(5.10789123481 + 64.9597385808 * t);
            R0 += 0.00000063290 * Math.Cos(4.39424910030 + 807.9497991134 * t);
            R0 += 0.00000064062 * Math.Cos(6.28297531806 + 7.1135470008 * t);
            R0 += 0.00000046356 * Math.Cos(1.34735469284 + 451.9404211107 * t);
            R0 += 0.00000060540 * Math.Cos(3.40316162416 + 294.6729761443 * t);
            R0 += 0.00000046900 * Math.Cos(0.17048203552 + 7.4223635415 * t);
            R0 += 0.00000056766 * Math.Cos(0.45048868231 + 140.001969579 * t);
            R0 += 0.00000055887 * Math.Cos(1.06815733757 + 172.1971143836 * t);
            R0 += 0.00000053761 * Math.Cos(2.79644687008 + 328.3525936572 * t);
            R0 += 0.00000043828 * Math.Cos(6.04655696644 + 135.5485514541 * t);
            R0 += 0.00000049549 * Math.Cos(0.64106656292 + 41.0537969446 * t);
            R0 += 0.00000053960 * Math.Cos(2.91774494436 + 563.6312150384 * t);
            R0 += 0.00000042961 * Math.Cos(5.40175361431 + 487.3651437628 * t);
            R0 += 0.00000051508 * Math.Cos(0.09105540708 + 210.3301500214 * t);
            R0 += 0.00000041889 * Math.Cos(3.12343223889 + 29.226199388 * t);
            R0 += 0.00000047655 * Math.Cos(3.90701760087 + 63.7358983034 * t);
            R0 += 0.00000041639 * Math.Cos(6.26847783513 + 32.7164096664 * t);
            R0 += 0.00000041429 * Math.Cos(4.45464156759 + 37.1698277913 * t);
            R0 += 0.00000040745 * Math.Cos(0.16043648294 + 79.2350166922 * t);
            R0 += 0.00000048205 * Math.Cos(1.84198373010 + 403.1341922245 * t);
            R0 += 0.00000036912 * Math.Cos(0.44771386183 + 30.0562807905 * t);
            R0 += 0.00000047762 * Math.Cos(0.88083849566 + 3302.479391062 * t);
            R0 += 0.00000039465 * Math.Cos(3.50565484069 + 357.4456666012 * t);
            R0 += 0.00000042139 * Math.Cos(0.63375113663 + 343.2185725996 * t);
            R0 += 0.00000041275 * Math.Cos(1.36370496322 + 31.2319369581 * t);
            R0 += 0.00000042612 * Math.Cos(3.55270845713 + 38.6543004996 * t);
            R0 += 0.00000038931 * Math.Cos(5.26691753270 + 415.2918581812 * t);
            R0 += 0.00000038967 * Math.Cos(5.25866056502 + 386.9806825299 * t);
            R0 += 0.00000033734 * Math.Cos(5.24400184426 + 67.3592350258 * t);
            R0 += 0.00000040879 * Math.Cos(3.55292279438 + 331.3215390738 * t);
            R0 += 0.00000038768 * Math.Cos(1.12288359393 + 38.1812197476 * t);
            R0 += 0.00000037500 * Math.Cos(6.08687972441 + 35.4247226521 * t);
            R0 += 0.00000038831 * Math.Cos(4.67876780698 + 38.084851528 * t);
            R0 += 0.00000038231 * Math.Cos(6.26491054328 + 389.9496279465 * t);
            R0 += 0.00000029976 * Math.Cos(4.45759985804 + 22.633917249 * t);
            R0 += 0.00000031356 * Math.Cos(0.07746010366 + 12.5301729722 * t);
            R0 += 0.00000026341 * Math.Cos(4.59559782754 + 106.0135355254 * t);
            R0 += 0.00000027465 * Math.Cos(5.99541587890 + 206.1855484372 * t);
            R0 += 0.00000025152 * Math.Cos(4.49867760320 + 34.2008823747 * t);
            R0 += 0.00000024122 * Math.Cos(5.17089441917 + 129.9194771616 * t);
            R0 += 0.00000028997 * Math.Cos(3.64927210210 + 253.5709950899 * t);
            R0 += 0.00000027173 * Math.Cos(4.37944546475 + 142.1408335931 * t);
            R0 += 0.00000030634 * Math.Cos(1.59348806560 + 348.8476468921 * t);
            R0 += 0.00000031464 * Math.Cos(1.05065113524 + 100.3844612329 * t);
            R0 += 0.00000024056 * Math.Cos(1.02801635413 + 41.7563723602 * t);
            R0 += 0.00000022632 * Math.Cos(4.72511111292 + 81.3738807063 * t);
            R0 += 0.00000021942 * Math.Cos(3.48416607882 + 69.1525242748 * t);
            R0 += 0.00000026333 * Math.Cos(3.01556008632 + 365.0011565867 * t);
            R0 += 0.00000022355 * Math.Cos(3.92220883921 + 5.1078094307 * t);
            R0 += 0.00000022498 * Math.Cos(4.03487494425 + 19.1224551112 * t);
            R0 += 0.00000022885 * Math.Cos(1.58977064672 + 189.3931538018 * t);
            R0 += 0.00000026520 * Math.Cos(3.61427038042 + 367.9701020033 * t);
            R0 += 0.00000025496 * Math.Cos(2.43810518614 + 351.8165923087 * t);
            R0 += 0.00000019111 * Math.Cos(2.59694457001 + 2080.6308247406 * t);
            R0 += 0.00000019640 * Math.Cos(6.15701741238 + 35.212274331 * t);
            R0 += 0.00000025688 * Math.Cos(2.00512719767 + 439.782755154 * t);
            R0 += 0.00000021613 * Math.Cos(3.32354204724 + 119.5069163441 * t);
            R0 += 0.00000025389 * Math.Cos(4.74025836522 + 1474.6737883704 * t);
            R0 += 0.00000018107 * Math.Cos(5.35129342595 + 244.318584075 * t);
            R0 += 0.00000023295 * Math.Cos(5.93767742799 + 316.3918696566 * t);
            R0 += 0.00000022087 * Math.Cos(4.81594755148 + 84.3428261229 * t);
            R0 += 0.00000016972 * Math.Cos(3.05105149940 + 220.4126424388 * t);
            R0 += 0.00000020022 * Math.Cos(4.99276451168 + 179.0982130633 * t);
            R0 += 0.00000020370 * Math.Cos(1.86508317889 + 171.2339065371 * t);
            R0 += 0.00000019426 * Math.Cos(2.04829970231 + 5.4166259714 * t);
            R0 += 0.00000022628 * Math.Cos(0.27205783433 + 666.723989257 * t);
            R0 += 0.00000019072 * Math.Cos(3.70882976684 + 164.1203595363 * t);
            R0 += 0.00000017969 * Math.Cos(3.40425338171 + 69.3649725959 * t);
            R0 += 0.00000018716 * Math.Cos(0.90215956591 + 285.3723810196 * t);
            R0 += 0.00000015889 * Math.Cos(0.42011285882 + 697.743477894 * t);
            R0 += 0.00000014988 * Math.Cos(3.08544843665 + 704.8570248948 * t);
            R0 += 0.00000014774 * Math.Cos(3.36129613309 + 274.0660483248 * t);
            R0 += 0.00000015972 * Math.Cos(1.82864185268 + 477.3308354552 * t);
            R0 += 0.00000013892 * Math.Cos(2.94161501165 + 38.3936680687 * t);
            R0 += 0.00000013922 * Math.Cos(2.85574364078 + 37.8724032069 * t);
            R0 += 0.00000015481 * Math.Cos(4.94982954853 + 101.8689339412 * t);
            R0 += 0.00000017571 * Math.Cos(5.82317632469 + 35.685355083 * t);
            R0 += 0.00000015856 * Math.Cos(5.04973561582 + 36.9091953604 * t);
            R0 += 0.00000016414 * Math.Cos(3.63049397028 + 45.2465826386 * t);
            R0 += 0.00000017158 * Math.Cos(2.51251149482 + 20.6069278195 * t);
            R0 += 0.00000012941 * Math.Cos(3.03041555329 + 522.5774180938 * t);
            R0 += 0.00000015752 * Math.Cos(5.00292909214 + 247.2393453818 * t);
            R0 += 0.00000012679 * Math.Cos(0.20331109568 + 460.5384408198 * t);
            R0 += 0.00000016260 * Math.Cos(5.93480347217 + 815.0633461142 * t);
            R0 += 0.00000012903 * Math.Cos(3.51141502996 + 446.3113468182 * t);
            R0 += 0.00000013891 * Math.Cos(5.51064697670 + 31.5407534988 * t);
            R0 += 0.00000013668 * Math.Cos(5.45576135320 + 39.3568759152 * t);
            R0 += 0.00000013418 * Math.Cos(3.95805150079 + 290.2195580194 * t);
            R0 += 0.00000015368 * Math.Cos(2.45783892707 + 26.826702943 * t);
            R0 += 0.00000014246 * Math.Cos(3.18588280921 + 401.6497195162 * t);
            R0 += 0.00000012222 * Math.Cos(4.94370170146 + 14.0146456805 * t);
            R0 += 0.00000015484 * Math.Cos(3.79703715637 + 404.6186649328 * t);
            R0 += 0.00000013427 * Math.Cos(3.79527836573 + 151.0476698429 * t);
            R0 += 0.00000014450 * Math.Cos(4.93940408761 + 120.9913890524 * t);
            R0 += 0.00000014331 * Math.Cos(4.71117327722 + 738.7972748386 * t);
            R0 += 0.00000011566 * Math.Cos(5.91003539239 + 536.8045120954 * t);
            R0 += 0.00000015578 * Math.Cos(2.91836788254 + 875.830299001 * t);
            R0 += 0.00000013124 * Math.Cos(2.16056013419 + 152.5321425512 * t);
            R0 += 0.00000011744 * Math.Cos(2.94770244071 + 2.7083129857 * t);
            R0 += 0.00000012793 * Math.Cos(1.97868575679 + 1.3725981237 * t);
            R0 += 0.00000012969 * Math.Cos(0.00535826017 + 97.4155158163 * t);
            R0 += 0.00000013891 * Math.Cos(4.76435441820 + 0.2606324309 * t);
            R0 += 0.00000013729 * Math.Cos(2.32306473850 + 38.2449102224 * t);
            R0 += 0.00000010714 * Math.Cos(6.18129683877 + 115.8835796217 * t);
            R0 += 0.00000011610 * Math.Cos(4.61712859898 + 178.7893965226 * t);
            R0 += 0.00000011257 * Math.Cos(0.79300245838 + 42.3258213318 * t);
            R0 += 0.00000014500 * Math.Cos(5.44690193314 + 44.070926471 * t);
            R0 += 0.00000011534 * Math.Cos(5.26580538005 + 160.9389657986 * t);
            R0 += 0.00000013355 * Math.Cos(5.20849186729 + 32.4557772355 * t);
            R0 += 0.00000013658 * Math.Cos(2.15687632802 + 476.4313180835 * t);
            R0 += 0.00000013782 * Math.Cos(3.47865209163 + 38.0211610532 * t);
            R0 += 0.00000012714 * Math.Cos(2.09462988855 + 20.4950532349 * t);
            R0 += 0.00000013257 * Math.Cos(5.15138524813 + 103.0927742186 * t);
            R0 += 0.00000009715 * Math.Cos(0.74597883480 + 918.1561203328 * t);
            R0 += 0.00000010340 * Math.Cos(5.38977407079 + 222.8603229936 * t);
            R0 += 0.00000013357 * Math.Cos(5.89635739027 + 748.0978699633 * t);
            R0 += 0.00000012632 * Math.Cos(1.20306997433 + 16.1535096946 * t);
            R0 += 0.00000011437 * Math.Cos(1.58444114292 + 495.4900827199 * t);
            R0 += 0.00000011424 * Math.Cos(4.74142930795 + 487.6257761937 * t);
            R0 += 0.00000009098 * Math.Cos(5.19932138822 + 118.0224436358 * t);
            R0 += 0.00000009336 * Math.Cos(0.97313630925 + 662.531203563 * t);
            R0 += 0.00000009827 * Math.Cos(4.48170250645 + 505.7850234584 * t);
            R0 += 0.00000008585 * Math.Cos(0.20375451897 + 944.9828232758 * t);
            R0 += 0.00000008875 * Math.Cos(5.53111742265 + 17.5261078183 * t);
            R0 += 0.00000009957 * Math.Cos(4.03258125243 + 169.5369855077 * t);
            R0 += 0.00000011506 * Math.Cos(3.11649121817 + 17.6379824029 * t);
            R0 += 0.00000009818 * Math.Cos(5.20376439002 + 1.5963472929 * t);
            R0 += 0.00000010160 * Math.Cos(3.74441320429 + 457.617679513 * t);
            R0 += 0.00000008661 * Math.Cos(0.31247523804 + 1440.7335384266 * t);
            R0 += 0.00000008496 * Math.Cos(1.06445636872 + 55.7710180407 * t);
            R0 += 0.00000011162 * Math.Cos(1.92907800408 + 564.8550553158 * t);
            R0 += 0.00000008057 * Math.Cos(0.31116345866 + 377.4194549743 * t);
            R0 += 0.00000009851 * Math.Cos(4.23328578127 + 418.2608035978 * t);
            R0 += 0.00000007938 * Math.Cos(2.40417397694 + 488.3765357191 * t);
            R0 += 0.00000009894 * Math.Cos(0.63707319139 + 183.7640795093 * t);
            R0 += 0.00000009913 * Math.Cos(3.94049519088 + 441.2672278623 * t);
            R0 += 0.00000007867 * Math.Cos(3.87469522964 + 494.7393231945 * t);
            R0 += 0.00000007589 * Math.Cos(3.15909316566 + 416.7763308895 * t);
            R0 += 0.00000008496 * Math.Cos(5.38968698704 + 104.0077979553 * t);
            R0 += 0.00000009716 * Math.Cos(3.06038536864 + 166.5680400911 * t);
            R0 += 0.00000009377 * Math.Cos(0.56416645296 + 673.316271396 * t);
            R0 += 0.00000008771 * Math.Cos(5.24534141981 + 1057.8974574809 * t);
            R0 += 0.00000007990 * Math.Cos(1.55726966638 + 59.2824801785 * t);
            R0 += 0.00000009090 * Math.Cos(4.32953439022 + 29.7474642498 * t);
            R0 += 0.00000009667 * Math.Cos(5.89033222679 + 358.4088744477 * t);
            R0 += 0.00000007209 * Math.Cos(2.29464803358 + 79.1868325824 * t);
            R0 += 0.00000008062 * Math.Cos(0.44458003524 + 19.0105805266 * t);
            R0 += 0.00000008254 * Math.Cos(3.47304582051 + 156.1554792736 * t);
            R0 += 0.00000009804 * Math.Cos(6.06393995615 + 784.7464328928 * t);
            R0 += 0.00000008516 * Math.Cos(5.99060386955 + 180.7951340927 * t);
            R0 += 0.00000008090 * Math.Cos(1.38588221442 + 1654.0326338646 * t);
            R0 += 0.00000009074 * Math.Cos(4.03971490460 + 1017.0561088574 * t);
            R0 += 0.00000006908 * Math.Cos(1.41919832926 + 178.3474535379 * t);
            R0 += 0.00000008230 * Math.Cos(2.53750470473 + 518.3846323998 * t);
            R0 += 0.00000008594 * Math.Cos(5.29104206063 + 457.8783119439 * t);
            R0 += 0.00000006769 * Math.Cos(5.43380191356 + 171.9846660625 * t);
            R0 += 0.00000008571 * Math.Cos(0.35876828441 + 636.6677084665 * t);
            R0 += 0.00000008995 * Math.Cos(1.36992508507 + 6209.7787241324 * t);
            R0 += 0.00000006641 * Math.Cos(2.92327140872 + 0.0481841098 * t);
            R0 += 0.00000009278 * Math.Cos(3.80308677009 + 25558.2121764796 * t);
            R0 += 0.00000006567 * Math.Cos(4.01934954352 + 0.1118745846 * t);
            R0 += 0.00000006441 * Math.Cos(4.28250687347 + 36.1272980677 * t);
            R0 += 0.00000007257 * Math.Cos(4.09776235307 + 326.8681209489 * t);
            R0 += 0.00000008384 * Math.Cos(5.49363770202 + 532.6117264014 * t);
            R0 += 0.00000007471 * Math.Cos(4.62144262894 + 526.9826521089 * t);
            R0 += 0.00000007500 * Math.Cos(0.61545750834 + 485.9288551643 * t);
            R0 += 0.00000007716 * Math.Cos(1.04880632264 + 525.2375469697 * t);
            R0 += 0.00000008504 * Math.Cos(2.79350586429 + 10139.988420352 * t);
            R0 += 0.00000007466 * Math.Cos(5.07942174095 + 157.6399519819 * t);
            R0 += 0.00000007186 * Math.Cos(6.22833818429 + 77.2292791221 * t);
            R0 += 0.00000007784 * Math.Cos(1.89308880453 + 984.6003316219 * t);
            R0 += 0.00000006513 * Math.Cos(0.07498932215 + 79.889407998 * t);
            R0 += 0.00000006077 * Math.Cos(2.96673519667 + 36.6967470393 * t);
            R0 += 0.00000007706 * Math.Cos(5.70632580790 + 209.106309744 * t);
            R0 += 0.00000007265 * Math.Cos(4.94483532589 + 131.4039498699 * t);
            R0 += 0.00000006984 * Math.Cos(2.53239305821 + 497.1870037493 * t);
            R0 += 0.00000007824 * Math.Cos(2.31462643851 + 513.079881013 * t);
            R0 += 0.00000007175 * Math.Cos(3.69203633127 + 524.0137066923 * t);
            R0 += 0.00000006855 * Math.Cos(0.14076801572 + 283.6272758804 * t);
            R0 += 0.00000006922 * Math.Cos(3.36515011915 + 438.2982824457 * t);
            R0 += 0.00000007349 * Math.Cos(3.50406958122 + 500.1559491659 * t);
            R0 += 0.00000006301 * Math.Cos(0.14776691217 + 608.877797677 * t);
            R0 += 0.00000005892 * Math.Cos(4.24403528888 + 4.665866446 * t);
            R0 += 0.00000007613 * Math.Cos(5.14905171677 + 259.5088859231 * t);
            R0 += 0.00000007128 * Math.Cos(5.92696788834 + 482.9599097477 * t);
            R0 += 0.00000006829 * Math.Cos(1.01745137848 + 1543.8263126452 * t);
            R0 += 0.00000005981 * Math.Cos(4.79954091087 + 215.4379594521 * t);
            R0 += 0.00000005526 * Math.Cos(2.34003154732 + 65.2203710117 * t);
            R0 += 0.00000006817 * Math.Cos(6.12162829690 + 395.0574373772 * t);
            R0 += 0.00000005369 * Math.Cos(3.76855960849 + 52099.5402118728 * t);
            R0 += 0.00000005776 * Math.Cos(5.61434462641 + 987.5692770385 * t);
            R0 += 0.00000007523 * Math.Cos(5.60432148128 + 2810.9214616052 * t);
            R0 += 0.00000007329 * Math.Cos(3.76815551582 + 1512.8068240082 * t);
            R0 += 0.00000005616 * Math.Cos(2.13872867116 + 145.6310438715 * t);
            R0 += 0.00000005258 * Math.Cos(0.30850836910 + 36.6003788197 * t);
            R0 += 0.00000005688 * Math.Cos(1.82274388581 + 1227.4344429886 * t);
            R0 += 0.00000005658 * Math.Cos(2.35049199704 + 5.6290742925 * t);
            R0 += 0.00000006135 * Math.Cos(4.23390561816 + 496.0113475817 * t);
            R0 += 0.00000005128 * Math.Cos(2.89050864873 + 313.6835566709 * t);
            R0 += 0.00000006472 * Math.Cos(3.49494191669 + 552.6973893591 * t);
            R0 += 0.00000004983 * Math.Cos(3.91958511552 + 10135.5350022271 * t);
            R0 += 0.00000005217 * Math.Cos(0.40052635702 + 319.3126309634 * t);
            R0 += 0.00000004952 * Math.Cos(1.42482088612 + 49.1787359017 * t);
            R0 += 0.00000005964 * Math.Cos(5.70758449643 + 309.7995875176 * t);
            R0 += 0.00000005091 * Math.Cos(6.00974510144 + 1409.7140497896 * t);
            R0 += 0.00000005205 * Math.Cos(5.50271334510 + 238.9019581036 * t);
            R0 += 0.00000004800 * Math.Cos(1.13450310670 + 134.0640787458 * t);
            R0 += 0.00000004943 * Math.Cos(1.43051344597 + 422.405405182 * t);
            R0 += 0.00000005604 * Math.Cos(2.05669305961 + 207.3612046048 * t);
            R0 += 0.00000006310 * Math.Cos(5.22966882627 + 139.7413371481 * t);
            R0 += 0.00000004772 * Math.Cos(3.06668713747 + 464.7312265138 * t);
            R0 += 0.00000004919 * Math.Cos(3.57280542629 + 52175.8062831484 * t);
            R0 += 0.00000004762 * Math.Cos(5.90654311203 + 838.9692877504 * t);
            R0 += 0.00000004848 * Math.Cos(0.77467099227 + 1.6969210294 * t);
            R0 += 0.00000005694 * Math.Cos(0.77313415569 + 709.9648343255 * t);
            R0 += 0.00000005455 * Math.Cos(0.90289242792 + 208.8456773131 * t);
            R0 += 0.00000004901 * Math.Cos(3.79986913631 + 15.4991183888 * t);
            R0 += 0.00000004772 * Math.Cos(0.15755140037 + 39.5056337615 * t);
            R0 += 0.00000005673 * Math.Cos(2.68359159067 + 1127.2624300768 * t);
            R0 += 0.00000005477 * Math.Cos(0.53123497431 + 113.8778420516 * t);
            R0 += 0.00000005077 * Math.Cos(1.59268428609 + 1547.9709142294 * t);
            R0 += 0.00000004981 * Math.Cos(1.44584050478 + 1.2720243872 * t);
            R0 += 0.00000005813 * Math.Cos(5.85024085408 + 57.255490749 * t);
            R0 += 0.00000005520 * Math.Cos(5.06396698257 + 421.2297490144 * t);
            R0 += 0.00000005938 * Math.Cos(0.96886308551 + 6280.1069045748 * t);
            R0 += 0.00000005206 * Math.Cos(3.58003819370 + 474.9468453752 * t);
            R0 += 0.00000005256 * Math.Cos(0.61005270999 + 95.9792272178 * t);
            R0 += 0.00000005531 * Math.Cos(5.28764137194 + 36.7604375141 * t);
            R0 += 0.00000006158 * Math.Cos(5.73176703797 + 711.4493070338 * t);
            R0 += 0.00000005003 * Math.Cos(2.19048397989 + 501.6404218742 * t);
            R0 += 0.00000005150 * Math.Cos(5.58407480282 + 26049.7701059364 * t);
            R0 += 0.00000005138 * Math.Cos(4.55234158942 + 670.916774951 * t);
            R0 += 0.00000005609 * Math.Cos(4.37272759780 + 52.8020726241 * t);
            R0 += 0.00000005636 * Math.Cos(2.39183054397 + 10210.3166007944 * t);
            R0 += 0.00000004512 * Math.Cos(2.59978208967 + 1234.5479899894 * t);
            R0 += 0.00000005412 * Math.Cos(4.58813638089 + 179.6194779251 * t);
            R0 += 0.00000004314 * Math.Cos(3.38846714337 + 142.6620984549 * t);
            R0 += 0.00000004708 * Math.Cos(5.23537414423 + 3.6233367224 * t);
            R0 += 0.00000004471 * Math.Cos(3.94378336812 + 12566.1516999828 * t);
            R0 += 0.00000005296 * Math.Cos(1.12249063176 + 134.1122628556 * t);
            R0 += 0.00000004188 * Math.Cos(2.52490407427 + 6205.3253060075 * t);
            R0 += 0.00000004645 * Math.Cos(1.90644271528 + 13324.3166711614 * t);
            R0 += 0.00000004502 * Math.Cos(2.01956920977 + 315.1680293792 * t);
            R0 += 0.00000005346 * Math.Cos(2.94804816223 + 353.0404325861 * t);
            R0 += 0.00000004177 * Math.Cos(2.09489065926 + 803.7570134194 * t);
            R0 += 0.00000005296 * Math.Cos(3.88249567974 + 2118.7638603784 * t);
            R0 += 0.00000005325 * Math.Cos(4.28221258353 + 477.9157907918 * t);
            R0 += 0.00000005519 * Math.Cos(0.09960891963 + 600.019145537 * t);
            R0 += 0.00000005169 * Math.Cos(0.59948596687 + 6.9010986797 * t);
            R0 += 0.00000004179 * Math.Cos(0.14619703083 + 6644.5762904701 * t);
            R0 += 0.00000004490 * Math.Cos(1.07042724999 + 52139.15772021889 * t);
            R0 += 0.00000003970 * Math.Cos(6.13227798578 + 1553.9088050626 * t);
            R0 += 0.00000003970 * Math.Cos(4.69887237362 + 91.7864415238 * t);
            R0 += 0.00000004234 * Math.Cos(0.14478458924 + 65.8747623175 * t);
            R0 += 0.00000005183 * Math.Cos(3.52837189306 + 110.2063212194 * t);
            R0 += 0.00000005259 * Math.Cos(6.20809827528 + 142.7102825647 * t);
            R0 += 0.00000003869 * Math.Cos(5.25125030487 + 1558.0534066468 * t);
            R0 += 0.00000004457 * Math.Cos(2.10248126544 + 487.1045113319 * t);
            R0 += 0.00000004890 * Math.Cos(1.83606790269 + 46.5186070258 * t);
            R0 += 0.00000003875 * Math.Cos(5.60269278935 + 385.4962098216 * t);
            R0 += 0.00000003826 * Math.Cos(1.30946706974 + 2176.6100519584 * t);
            R0 += 0.00000004591 * Math.Cos(4.84657580441 + 1337.640764208 * t);
            R0 += 0.00000005111 * Math.Cos(1.18808079775 + 981.6313862053 * t);
            R0 += 0.00000004709 * Math.Cos(1.40878215308 + 52213.9393187862 * t);
            R0 += 0.00000003891 * Math.Cos(5.43661875415 + 154.6710065653 * t);
            R0 += 0.00000004145 * Math.Cos(4.32505910718 + 363.5166838784 * t);
            R0 += 0.00000004441 * Math.Cos(3.50158424570 + 187.6962327724 * t);
            R0 += 0.00000003703 * Math.Cos(2.48768949613 + 67.8804998876 * t);
            R0 += 0.00000004094 * Math.Cos(1.42347047260 + 310.7146112543 * t);
            R0 += 0.00000003681 * Math.Cos(5.70552661143 + 491.6698040414 * t);
            R0 += 0.00000004787 * Math.Cos(3.65822147476 + 589.3459522886 * t);
            R0 += 0.00000004020 * Math.Cos(5.45643059988 + 6641.6073450535 * t);
            R0 += 0.00000003656 * Math.Cos(0.57790726599 + 491.4460548722 * t);
            R0 += 0.00000004288 * Math.Cos(3.35265955957 + 203.2166030206 * t);
            R0 += 0.00000003843 * Math.Cos(4.61508898119 + 1025.7023126763 * t);
            R0 += 0.00000003767 * Math.Cos(0.05292047125 + 320.2758388099 * t);
            R0 += 0.00000004632 * Math.Cos(0.82011276589 + 3265.8308281325 * t);
            R0 += 0.00000004609 * Math.Cos(5.25443775917 + 296.1574488526 * t);
            R0 += 0.00000004555 * Math.Cos(5.30391170376 + 26013.1215430069 * t);
            R0 += 0.00000003556 * Math.Cos(4.80267245336 + 224.3447957019 * t);
            R0 += 0.00000004859 * Math.Cos(5.52756242256 + 487.4133278726 * t);
            R0 += 0.00000003626 * Math.Cos(1.44624342082 + 70.8494453042 * t);
            R0 += 0.00000004302 * Math.Cos(1.60914544159 + 12529.5031370533 * t);
            R0 += 0.00000003493 * Math.Cos(4.75315651083 + 12489.8856287072 * t);
            R0 += 0.00000003722 * Math.Cos(0.27433061822 + 949.4362414007 * t);
            R0 += 0.00000004234 * Math.Cos(5.25112033465 + 194.2885149114 * t);
            R0 += 0.00000003451 * Math.Cos(2.97409317928 + 499.6346843041 * t);
            R0 += 0.00000004796 * Math.Cos(6.21059766333 + 491.8185618877 * t);
            R0 += 0.00000003639 * Math.Cos(1.25605018211 + 2603.2082428344 * t);
            R0 += 0.00000004646 * Math.Cos(5.71392540144 + 321.7603115182 * t);
            R0 += 0.00000003702 * Math.Cos(2.08952561657 + 491.036664595 * t);
            R0 += 0.00000003672 * Math.Cos(2.87489628704 + 497.49582029 * t);
            R0 += 0.00000003965 * Math.Cos(1.05484988240 + 75.7448064138 * t);
            R0 += 0.00000003416 * Math.Cos(0.68584132933 + 305.0855369618 * t);
            R0 += 0.00000004513 * Math.Cos(4.38927002490 + 425.1137181677 * t);
            R0 += 0.00000003853 * Math.Cos(0.61321572401 + 12526.5341916367 * t);
            R0 += 0.00000003788 * Math.Cos(3.32221995840 + 3140.0127549298 * t);
            R0 += 0.00000003781 * Math.Cos(5.58125317044 + 1652.5481611563 * t);
            R0 += 0.00000003903 * Math.Cos(5.31609723466 + 408.1783111804 * t);
            R0 += 0.00000003945 * Math.Cos(3.60558877407 + 1589.0728952838 * t);
            R0 += 0.00000004084 * Math.Cos(0.83813879869 + 52.3601296394 * t);
            R0 += 0.00000004084 * Math.Cos(3.50290269471 + 23.9059416362 * t);
            R0 += 0.00000003694 * Math.Cos(1.03218855688 + 481.4754370394 * t);
            R0 += 0.00000003636 * Math.Cos(5.31068934607 + 141.4864422873 * t);
            R0 += 0.00000003345 * Math.Cos(3.94392179077 + 20389.92252949249 * t);
            R0 += 0.00000004639 * Math.Cos(6.24618220184 + 821.3949958223 * t);
            R0 += 0.00000003934 * Math.Cos(0.26992234338 + 1655.5171065729 * t);
            R0 += 0.00000004431 * Math.Cos(2.48647437800 + 549.7284439425 * t);
            R0 += 0.00000004168 * Math.Cos(5.39993754642 + 236.5024616586 * t);
            R0 += 0.00000004020 * Math.Cos(0.07393243012 + 52136.18877480229 * t);
            R0 += 0.00000004055 * Math.Cos(1.34004288978 + 1054.9285120643 * t);
            R0 += 0.00000003275 * Math.Cos(0.98533127454 + 1344.7543112088 * t);
            R0 += 0.00000003213 * Math.Cos(2.97105590703 + 20386.95358407589 * t);
            R0 += 0.00000004428 * Math.Cos(0.06728869735 + 491.2972970259 * t);
            R0 += 0.00000004063 * Math.Cos(0.06192838570 + 6168.676743078 * t);
            R0 += 0.00000003804 * Math.Cos(5.34897033476 + 523.7530742614 * t);
            R0 += 0.00000003917 * Math.Cos(5.67905809516 + 1131.1945833399 * t);
            R0 += 0.00000003833 * Math.Cos(0.87811168267 + 52.6901980395 * t);
            R0 += 0.00000004020 * Math.Cos(2.69209723289 + 1439.4615140394 * t);
            R0 += 0.00000004373 * Math.Cos(1.86209663434 + 73.5577582899 * t);
            R0 += 0.00000003159 * Math.Cos(1.04693380342 + 703.3725521865 * t);
            R0 += 0.00000003116 * Math.Cos(5.20159166840 + 449.232108125 * t);
            R0 += 0.00000003258 * Math.Cos(4.65131076542 + 696.2590051857 * t);
            R0 += 0.00000003427 * Math.Cos(0.27003884843 + 2389.9091473964 * t);
            R0 += 0.00000004349 * Math.Cos(0.07531141761 + 20426.571092422 * t);
            R0 += 0.00000003383 * Math.Cos(5.61838426864 + 699.2279506023 * t);
            R0 += 0.00000003305 * Math.Cos(1.41666877290 + 562.1467423301 * t);
            R0 += 0.00000003297 * Math.Cos(5.46677712589 + 1442.2180111349 * t);
            R0 += 0.00000003277 * Math.Cos(2.71815883511 + 980.146913497 * t);
            R0 += 0.00000003171 * Math.Cos(4.49510885866 + 1439.2490657183 * t);
            R0 += 0.00000004175 * Math.Cos(4.24327707038 + 381.6122406683 * t);
            R0 += 0.00000003155 * Math.Cos(3.40776789576 + 39.7293829307 * t);
            R0 += 0.00000004112 * Math.Cos(0.90309319273 + 1087.6931058405 * t);
            R0 += 0.00000003350 * Math.Cos(5.27474671017 + 80.7194894005 * t);
            R0 += 0.00000003725 * Math.Cos(1.52448613082 + 1058.109905802 * t);
            R0 += 0.00000003650 * Math.Cos(3.59798316565 + 192.8040422031 * t);
            R0 += 0.00000003837 * Math.Cos(1.48519528444 + 10098.8864392976 * t);
            R0 += 0.00000002959 * Math.Cos(1.23012121982 + 2500.1154686158 * t);
            R0 += 0.00000003330 * Math.Cos(6.12470287875 + 10172.1835651566 * t);
            R0 += 0.00000003361 * Math.Cos(4.31837298696 + 492.0791943186 * t);
            R0 += 0.00000003288 * Math.Cos(3.14692435376 + 347.3631741838 * t);
            R0 += 0.00000002992 * Math.Cos(5.01304660316 + 175.21424391 * t);
            R0 += 0.00000003294 * Math.Cos(2.52694043155 + 1692.1656695024 * t);
            R0 += 0.00000002984 * Math.Cos(1.81780659890 + 175.1178756904 * t);
            R0 += 0.00000003013 * Math.Cos(0.92957285991 + 1515.7757694248 * t);
            R0 += 0.00000003863 * Math.Cos(5.46044928570 + 332.8060117821 * t);
            R0 += 0.00000003403 * Math.Cos(1.10932483984 + 987.3086446076 * t);
            R0 += 0.00000003312 * Math.Cos(0.67710158807 + 977.4867846211 * t);
            R0 += 0.00000003030 * Math.Cos(1.77996261146 + 156489.28581380738 * t);
            R0 += 0.00000003605 * Math.Cos(4.89955108152 + 1043.8828118004 * t);
            R0 += 0.00000002937 * Math.Cos(0.60469671230 + 990.2294059144 * t);
            R0 += 0.00000003276 * Math.Cos(4.26765608367 + 1189.3014073508 * t);
            R0 += 0.00000002966 * Math.Cos(5.29808076929 + 31.9826964835 * t);
            R0 += 0.00000002994 * Math.Cos(2.58599359402 + 178.086821107 * t);
            R0 += 0.00000003905 * Math.Cos(1.87748122254 + 1158.2819187138 * t);
            R0 += 0.00000003110 * Math.Cos(3.09203517638 + 235.933012687 * t);
            R0 += 0.00000003313 * Math.Cos(2.70308129756 + 604.4725636619 * t);
            R0 += 0.00000003276 * Math.Cos(1.24440460327 + 874.6546428334 * t);
            R0 += 0.00000003276 * Math.Cos(5.58544609667 + 950.920714109 * t);
            R0 += 0.00000003746 * Math.Cos(0.33859914037 + 913.9633346388 * t);
            R0 += 0.00000003552 * Math.Cos(3.07180917863 + 240.3864308119 * t);
            R0 += 0.00000002885 * Math.Cos(6.01130634957 + 1097.514965827 * t);
            R0 += 0.00000003643 * Math.Cos(5.11977873355 + 452.2010535416 * t);
            R0 += 0.00000002768 * Math.Cos(4.38396269009 + 391.4341006548 * t);
            R0 += 0.00000002776 * Math.Cos(5.01821594830 + 8.9068362498 * t);
            R0 += 0.00000002990 * Math.Cos(5.62911695857 + 140.6563608848 * t);
            R0 += 0.00000002761 * Math.Cos(4.05534163807 + 6283.0758499914 * t);
            R0 += 0.00000003226 * Math.Cos(4.76711354367 + 6241.973868937 * t);
            R0 += 0.00000003748 * Math.Cos(4.84009347869 + 341.7340998913 * t);
            R0 += 0.00000002752 * Math.Cos(4.53621078796 + 6206.8097787158 * t);
            R0 += 0.00000003847 * Math.Cos(2.40982343643 + 26086.4186688659 * t);
            R0 += 0.00000002727 * Math.Cos(3.28234198801 + 483.4811746095 * t);
            R0 += 0.00000002884 * Math.Cos(4.05452029151 + 1.2238402774 * t);
            R0 += 0.00000002702 * Math.Cos(3.72061244391 + 946.4672959841 * t);
            R0 += 0.00000002723 * Math.Cos(4.37517047024 + 15.1903018481 * t);
            R0 += 0.00000002847 * Math.Cos(5.22951186538 + 661.0467308547 * t);
            R0 += 0.00000002680 * Math.Cos(4.19379121323 + 13.184564278 * t);
            R0 += 0.00000003269 * Math.Cos(0.43119778520 + 496.9745554282 * t);
            R0 += 0.00000003489 * Math.Cos(3.82213189319 + 625.9945152181 * t);
            R0 += 0.00000003757 * Math.Cos(3.88223872147 + 495.702531041 * t);
            R0 += 0.00000002872 * Math.Cos(5.00345974886 + 252.0865223816 * t);
            R0 += 0.00000003742 * Math.Cos(2.03372773652 + 8.5980197091 * t);
            R0 += 0.00000003172 * Math.Cos(1.11135762382 + 260.9933586314 * t);
            R0 += 0.00000003341 * Math.Cos(2.91360557418 + 304.2342036999 * t);
            R0 += 0.00000002915 * Math.Cos(2.63627684599 + 6681.2248533996 * t);
            R0 += 0.00000002915 * Math.Cos(1.43773625890 + 6604.958782124 * t);
            R0 += 0.00000002629 * Math.Cos(2.09824407450 + 2713.4145640538 * t);
            R0 += 0.00000002901 * Math.Cos(3.33924800230 + 515.463871093 * t);
            R0 += 0.00000002803 * Math.Cos(1.23584865903 + 6643.0918177618 * t);
            R0 += 0.00000003045 * Math.Cos(3.33515866438 + 921.0768816396 * t);
            R0 += 0.00000002699 * Math.Cos(5.42597794650 + 925.2696673336 * t);
            R0 += 0.00000002808 * Math.Cos(5.77870303237 + 1024.217839968 * t);
            R0 += 0.00000003028 * Math.Cos(3.75501312393 + 511.5954083047 * t);
            R0 += 0.00000003090 * Math.Cos(2.49453093252 + 14.6690369863 * t);
            R0 += 0.00000002913 * Math.Cos(4.83296711477 + 515.936951845 * t);
            R0 += 0.00000003139 * Math.Cos(5.99134254710 + 570.7447620392 * t);
            R0 += 0.00000002752 * Math.Cos(3.08268180744 + 853.196381752 * t);
            R0 += 0.00000002779 * Math.Cos(3.74527347899 + 494.0056100116 * t);
            R0 += 0.00000002643 * Math.Cos(1.99093797444 + 470.2172884544 * t);
            R0 += 0.00000002763 * Math.Cos(4.01095972177 + 448.9714756941 * t);
            R0 += 0.00000002643 * Math.Cos(5.24970673655 + 249.9476583675 * t);
            R0 += 0.00000003426 * Math.Cos(4.73955481174 + 1050.9963588012 * t);
            R0 += 0.00000002573 * Math.Cos(2.01267457287 + 1514.2912967165 * t);
            R0 += 0.00000002633 * Math.Cos(1.63640090603 + 170.7126416753 * t);
            R0 += 0.00000003034 * Math.Cos(4.48979734509 + 560.7104537316 * t);
            R0 += 0.00000003025 * Math.Cos(5.51446170055 + 369.4545747116 * t);
            R0 += 0.00000003095 * Math.Cos(4.01459691667 + 1615.8995982268 * t);
            R0 += 0.00000002490 * Math.Cos(0.15301603966 + 78187.44335344699 * t);
            R0 += 0.00000002589 * Math.Cos(0.79196093766 + 1228.9189156969 * t);
            R0 += 0.00000003143 * Math.Cos(5.33170343283 + 1542.3418399369 * t);
            R0 += 0.00000003138 * Math.Cos(4.50785484172 + 461.7622810972 * t);
            R0 += 0.00000002812 * Math.Cos(3.74246594120 + 2.0057375701 * t);
            R0 += 0.00000003062 * Math.Cos(4.88018345098 + 227.9681324243 * t);
            R0 += 0.00000002553 * Math.Cos(4.85437812287 + 488.8496164711 * t);
            R0 += 0.00000002971 * Math.Cos(1.27359129352 + 530.914805372 * t);
            R0 += 0.00000002646 * Math.Cos(3.64828423565 + 335.7749571987 * t);
            R0 += 0.00000003329 * Math.Cos(2.71693827722 + 171.021458216 * t);
            R0 += 0.00000002648 * Math.Cos(0.60243117586 + 70.5888128733 * t);
            R0 += 0.00000003061 * Math.Cos(5.05044834864 + 378.6432952517 * t);
            R0 += 0.00000002738 * Math.Cos(4.75405645015 + 151.260118164 * t);
            R0 += 0.00000002728 * Math.Cos(5.89052930055 + 213.9534867438 * t);
            R0 += 0.00000003411 * Math.Cos(2.24137878065 + 734.4557312983 * t);
            R0 += 0.00000002623 * Math.Cos(0.54340876464 + 1586.1039498672 * t);
            R0 += 0.00000003169 * Math.Cos(5.84871429991 + 1049.5118860929 * t);
            R0 += 0.00000002430 * Math.Cos(2.34595493263 + 450.4559484024 * t);
            R0 += 0.00000002907 * Math.Cos(5.58085498481 + 597.5714649822 * t);
            R0 += 0.00000003300 * Math.Cos(0.94221473935 + 58.1705144857 * t);
            R0 += 0.00000002543 * Math.Cos(5.30426930256 + 419.4846438752 * t);
            R0 += 0.00000003175 * Math.Cos(2.32600231924 + 339.2864193365 * t);
            R0 += 0.00000002858 * Math.Cos(2.36621678719 + 32.5039613453 * t);
            R0 += 0.00000002712 * Math.Cos(5.79983621237 + 1587.5884225755 * t);
            R0 += 0.00000003340 * Math.Cos(1.36950315448 + 384.2723695442 * t);
            R0 += 0.00000003301 * Math.Cos(5.83023910521 + 51.7751743028 * t);
            R0 += 0.00000002415 * Math.Cos(0.69446923670 + 489.5521918867 * t);
            R0 += 0.00000002736 * Math.Cos(5.74320864965 + 1167.8431462694 * t);
            R0 += 0.00000002956 * Math.Cos(5.22962139507 + 199.8538987291 * t);
            R0 += 0.00000003262 * Math.Cos(0.01501002027 + 1545.3107853535 * t);
            R0 += 0.00000002506 * Math.Cos(4.84043333582 + 943.4983505675 * t);
            R0 += 0.00000003240 * Math.Cos(2.46676155925 + 1016.7954764265 * t);
            R0 += 0.00000003148 * Math.Cos(4.62079057738 + 233.533516242 * t);
            R0 += 0.00000002327 * Math.Cos(4.10421417326 + 70.1157321213 * t);
            R0 += 0.00000002371 * Math.Cos(4.79963943424 + 271.145287018 * t);
            R0 += 0.00000003006 * Math.Cos(3.66877796077 + 1476.1582610787 * t);
            R0 += 0.00000002537 * Math.Cos(5.66681769885 + 21.1494445407 * t);
            R0 += 0.00000003006 * Math.Cos(0.93048909480 + 21.9795259432 * t);
            R0 += 0.00000003033 * Math.Cos(0.67157488690 + 292.4859280204 * t);
            R0 += 0.00000002344 * Math.Cos(1.83547256266 + 492.3086889822 * t);
            R0 += 0.00000003117 * Math.Cos(2.76268894894 + 1473.1893156621 * t);
            R0 += 0.00000002323 * Math.Cos(2.88799980853 + 533.6231183577 * t);
            R0 += 0.00000002340 * Math.Cos(4.44862573253 + 490.8071699314 * t);
            R0 += 0.00000002511 * Math.Cos(0.99467349084 + 266.1011680621 * t);
            R0 += 0.00000002919 * Math.Cos(4.75889516601 + 1511.3223512999 * t);
            R0 += 0.00000002493 * Math.Cos(6.10541658597 + 1225.9499702803 * t);
            R0 += 0.00000002798 * Math.Cos(3.06162629894 + 419.7452763061 * t);
            R0 += 0.00000002691 * Math.Cos(3.20679023131 + 463.5073862364 * t);
            R0 += 0.00000002291 * Math.Cos(5.81534758547 + 246.9787129509 * t);
            R0 += 0.00000002319 * Math.Cos(6.05514281470 + 525.7588118315 * t);
            R0 += 0.00000003112 * Math.Cos(0.89712836583 + 314.9073969483 * t);
            R0 += 0.00000003085 * Math.Cos(5.84605938859 + 1192.2221686576 * t);
            R0 += 0.00000002897 * Math.Cos(0.54747024257 + 20350.3050211464 * t);
            R0 += 0.00000003067 * Math.Cos(2.22206306288 + 248.4631856592 * t);
            R0 += 0.00000002252 * Math.Cos(0.87483094907 + 61.0275853177 * t);
            R0 += 0.00000002392 * Math.Cos(3.62837597194 + 439.1977998174 * t);
            R0 += 0.00000002817 * Math.Cos(2.73562306571 + 16.6747745564 * t);
            R0 += 0.00000002379 * Math.Cos(6.17876088396 + 467.6519878206 * t);
            R0 += 0.00000002598 * Math.Cos(4.82643304253 + 384.5811860849 * t);
            R0 += 0.00000002718 * Math.Cos(1.01823841209 + 215.9592243139 * t);
            R0 += 0.00000002998 * Math.Cos(1.09755715300 + 1964.7472451189 * t);
            R0 += 0.00000002884 * Math.Cos(2.97813466834 + 383.0967133766 * t);
            R0 += 0.00000002231 * Math.Cos(4.48841493844 + 4.1446015842 * t);
            R0 += 0.00000002203 * Math.Cos(2.23336308907 + 481.2629887183 * t);
            R0 += 0.00000002260 * Math.Cos(2.35404913660 + 659.6104422562 * t);
            R0 += 0.00000002491 * Math.Cos(1.70236357070 + 445.3481389717 * t);
            R0 += 0.00000003041 * Math.Cos(5.55577674116 + 674.8007441043 * t);
            R0 += 0.00000002289 * Math.Cos(1.18497528002 + 1552.4243323543 * t);
            R0 += 0.00000002975 * Math.Cos(0.48272389481 + 1052.4808315095 * t);
            R0 += 0.00000002339 * Math.Cos(0.75318738767 + 478.8153081635 * t);
            R0 += 0.00000003011 * Math.Cos(0.16359500858 + 54.2865453324 * t);
            R0 += 0.00000002820 * Math.Cos(6.18522693724 + 556.5176680376 * t);
            R0 += 0.00000002266 * Math.Cos(5.91286000054 + 3.4902102784 * t);
            R0 += 0.00000002231 * Math.Cos(1.45038594906 + 196.5067008026 * t);
            return R0;
        }

        internal static double Neptune_R1(double t) // 250 terms of order 1
        {
            double R1 = 0;
            R1 += 0.00236338502 * Math.Cos(0.70498011235 + 38.1330356378 * t);
            R1 += 0.00013220279 * Math.Cos(3.32015499895 + 1.4844727083 * t);
            R1 += 0.00008621863 * Math.Cos(6.21628951630 + 35.1640902212 * t);
            R1 += 0.00002701740 * Math.Cos(1.88140666779 + 39.6175083461 * t);
            R1 += 0.00002153150 * Math.Cos(5.16873840979 + 76.2660712756 * t);
            R1 += 0.00002154735 * Math.Cos(2.09431198086 + 2.9689454166 * t);
            R1 += 0.00001463924 * Math.Cos(1.18417031047 + 33.6796175129 * t);
            R1 += 0.00001603165;
            R1 += 0.00001135773 * Math.Cos(3.91891199655 + 36.6485629295 * t);
            R1 += 0.00000897650 * Math.Cos(5.24122933533 + 388.4651552382 * t);
            R1 += 0.00000789908 * Math.Cos(0.53315484580 + 168.0525127994 * t);
            R1 += 0.00000760030 * Math.Cos(0.02051033644 + 182.279606801 * t);
            R1 += 0.00000607183 * Math.Cos(1.07706500350 + 1021.2488945514 * t);
            R1 += 0.00000571622 * Math.Cos(3.40060785432 + 484.444382456 * t);
            R1 += 0.00000560790 * Math.Cos(2.88685815667 + 498.6714764576 * t);
            R1 += 0.00000490190 * Math.Cos(3.46830928696 + 137.0330241624 * t);
            R1 += 0.00000264093 * Math.Cos(0.86220057976 + 4.4534181249 * t);
            R1 += 0.00000270526 * Math.Cos(3.27355867939 + 71.8126531507 * t);
            R1 += 0.00000203524 * Math.Cos(2.41820674409 + 32.1951448046 * t);
            R1 += 0.00000155438 * Math.Cos(0.36537064534 + 41.1019810544 * t);
            R1 += 0.00000132766 * Math.Cos(3.60157672619 + 9.5612275556 * t);
            R1 += 0.00000093626 * Math.Cos(0.66670888163 + 46.2097904851 * t);
            R1 += 0.00000083317 * Math.Cos(3.25992461673 + 98.8999885246 * t);
            R1 += 0.00000072205 * Math.Cos(4.47717435693 + 601.7642506762 * t);
            R1 += 0.00000068983 * Math.Cos(1.46326969479 + 74.7815985673 * t);
            R1 += 0.00000086953 * Math.Cos(5.77228651853 + 381.3516082374 * t);
            R1 += 0.00000068717 * Math.Cos(4.52563942435 + 70.3281804424 * t);
            R1 += 0.00000064724 * Math.Cos(3.85477388838 + 73.297125859 * t);
            R1 += 0.00000068377 * Math.Cos(3.39509945953 + 108.4612160802 * t);
            R1 += 0.00000053375 * Math.Cos(5.43650770516 + 395.578702239 * t);
            R1 += 0.00000044453 * Math.Cos(3.61409723545 + 2.4476805548 * t);
            R1 += 0.00000041243 * Math.Cos(4.73866592865 + 8.0767548473 * t);
            R1 += 0.00000048331 * Math.Cos(1.98568593981 + 175.1660598002 * t);
            R1 += 0.00000041744 * Math.Cos(4.94257598763 + 31.019488637 * t);
            R1 += 0.00000044102 * Math.Cos(1.41744904844 + 1550.939859646 * t);
            R1 += 0.00000041170 * Math.Cos(1.41999374753 + 490.0734567485 * t);
            R1 += 0.00000041099 * Math.Cos(4.86312637841 + 493.0424021651 * t);
            R1 += 0.00000036267 * Math.Cos(5.30764043577 + 312.1990839626 * t);
            R1 += 0.00000036284 * Math.Cos(0.38187812797 + 77.7505439839 * t);
            R1 += 0.00000040619 * Math.Cos(2.27237172464 + 529.6909650946 * t);
            R1 += 0.00000032360 * Math.Cos(5.91123007786 + 5.9378908332 * t);
            R1 += 0.00000031197 * Math.Cos(2.70549944134 + 1014.1353475506 * t);
            R1 += 0.00000032730 * Math.Cos(5.22147683115 + 41.0537969446 * t);
            R1 += 0.00000036079 * Math.Cos(4.87817494829 + 491.5579294568 * t);
            R1 += 0.00000030181 * Math.Cos(3.63273193845 + 30.7106720963 * t);
            R1 += 0.00000029991 * Math.Cos(3.30769367603 + 1028.3624415522 * t);
            R1 += 0.00000027048 * Math.Cos(1.77647060739 + 44.7253177768 * t);
            R1 += 0.00000027756 * Math.Cos(4.55583165091 + 7.1135470008 * t);
            R1 += 0.00000027475 * Math.Cos(0.97228280623 + 33.9402499438 * t);
            R1 += 0.00000024944 * Math.Cos(3.10083391185 + 144.1465711632 * t);
            R1 += 0.00000025958 * Math.Cos(2.99724758632 + 60.7669528868 * t);
            R1 += 0.00000021369 * Math.Cos(4.71270048898 + 278.2588340188 * t);
            R1 += 0.00000021283 * Math.Cos(0.68957829113 + 251.4321310758 * t);
            R1 += 0.00000023727 * Math.Cos(5.12044184469 + 176.6505325085 * t);
            R1 += 0.00000021392 * Math.Cos(0.86286397645 + 4.192785694 * t);
            R1 += 0.00000023373 * Math.Cos(1.64955088447 + 173.6815870919 * t);
            R1 += 0.00000024163 * Math.Cos(3.56602004577 + 145.1097790097 * t);
            R1 += 0.00000020238 * Math.Cos(5.61479765982 + 24.1183899573 * t);
            R1 += 0.00000026958 * Math.Cos(4.14294870704 + 453.424893819 * t);
            R1 += 0.00000024048 * Math.Cos(1.00718363213 + 213.299095438 * t);
            R1 += 0.00000018322 * Math.Cos(1.98028683488 + 72.0732855816 * t);
            R1 += 0.00000018266 * Math.Cos(6.17260374467 + 189.3931538018 * t);
            R1 += 0.00000019201 * Math.Cos(4.65162168927 + 106.9767433719 * t);
            R1 += 0.00000017606 * Math.Cos(1.60307551767 + 62.2514255951 * t);
            R1 += 0.00000016545 * Math.Cos(1.69931816587 + 357.4456666012 * t);
            R1 += 0.00000020132 * Math.Cos(3.29520553529 + 114.3991069134 * t);
            R1 += 0.00000015425 * Math.Cos(4.38812302799 + 25.6028626656 * t);
            R1 += 0.00000019173 * Math.Cos(2.20014267311 + 343.2185725996 * t);
            R1 += 0.00000015077 * Math.Cos(3.66802659382 + 0.5212648618 * t);
            R1 += 0.00000014029 * Math.Cos(0.55336333290 + 129.9194771616 * t);
            R1 += 0.00000013361 * Math.Cos(5.85751083720 + 68.8437077341 * t);
            R1 += 0.00000015357 * Math.Cos(4.20731277007 + 567.8240007324 * t);
            R1 += 0.00000012746 * Math.Cos(3.52815836608 + 477.3308354552 * t);
            R1 += 0.00000011724 * Math.Cos(5.57647263460 + 31.2319369581 * t);
            R1 += 0.00000011533 * Math.Cos(0.89138506506 + 594.6507036754 * t);
            R1 += 0.00000010508 * Math.Cos(4.35552732772 + 32.7164096664 * t);
            R1 += 0.00000010826 * Math.Cos(5.21826226871 + 26.826702943 * t);
            R1 += 0.00000010085 * Math.Cos(1.98102855874 + 40.5807161926 * t);
            R1 += 0.00000010518 * Math.Cos(5.27281360238 + 2.9207613068 * t);
            R1 += 0.00000009207 * Math.Cos(0.50092534158 + 64.9597385808 * t);
            R1 += 0.00000009231 * Math.Cos(0.68180977710 + 160.9389657986 * t);
            R1 += 0.00000008735 * Math.Cos(5.80657503476 + 6.592282139 * t);
            R1 += 0.00000010114 * Math.Cos(4.51164596694 + 28.5718080822 * t);
            R1 += 0.00000010392 * Math.Cos(5.18877536013 + 42.5864537627 * t);
            R1 += 0.00000009873 * Math.Cos(3.76512158080 + 181.7583419392 * t);
            R1 += 0.00000008350 * Math.Cos(2.82449631025 + 43.2408450685 * t);
            R1 += 0.00000009838 * Math.Cos(1.49438763600 + 47.6942631934 * t);
            R1 += 0.00000007645 * Math.Cos(4.07503370297 + 389.9496279465 * t);
            R1 += 0.00000008004 * Math.Cos(2.78082277326 + 505.7850234584 * t);
            R1 += 0.00000007440 * Math.Cos(2.35731983047 + 11.0457002639 * t);
            R1 += 0.00000007342 * Math.Cos(1.62279119952 + 135.5485514541 * t);
            R1 += 0.00000009450 * Math.Cos(0.27241261915 + 426.598190876 * t);
            R1 += 0.00000007192 * Math.Cos(0.82841201068 + 911.042573332 * t);
            R1 += 0.00000006979 * Math.Cos(1.86753914872 + 206.1855484372 * t);
            R1 += 0.00000006874 * Math.Cos(0.83802906828 + 82.8583534146 * t);
            R1 += 0.00000007897 * Math.Cos(1.86554246391 + 38.6543004996 * t);
            R1 += 0.00000006729 * Math.Cos(3.98338053636 + 12.5301729722 * t);
            R1 += 0.00000006357 * Math.Cos(0.90093123522 + 487.3651437628 * t);
            R1 += 0.00000006720 * Math.Cos(1.33936040700 + 220.4126424388 * t);
            R1 += 0.00000007695 * Math.Cos(5.13312500855 + 23.9059416362 * t);
            R1 += 0.00000007059 * Math.Cos(5.99832463494 + 639.897286314 * t);
            R1 += 0.00000008302 * Math.Cos(3.85960902325 + 37.611770776 * t);
            R1 += 0.00000006412 * Math.Cos(2.41743702679 + 1059.3819301892 * t);
            R1 += 0.00000006751 * Math.Cos(1.96860894470 + 45.2465826386 * t);
            R1 += 0.00000006431 * Math.Cos(4.07813226506 + 35.685355083 * t);
            R1 += 0.00000005517 * Math.Cos(3.81325790890 + 815.0633461142 * t);
            R1 += 0.00000005562 * Math.Cos(0.41619602150 + 563.6312150384 * t);
            R1 += 0.00000006115 * Math.Cos(2.10934525342 + 697.743477894 * t);
            R1 += 0.00000006216 * Math.Cos(4.79301628209 + 143.6253063014 * t);
            R1 += 0.00000005346 * Math.Cos(3.13071964722 + 386.9806825299 * t);
            R1 += 0.00000005245 * Math.Cos(6.06245070403 + 171.2339065371 * t);
            R1 += 0.00000005129 * Math.Cos(0.79394555531 + 179.0982130633 * t);
            R1 += 0.00000005168 * Math.Cos(4.73765992885 + 522.5774180938 * t);
            R1 += 0.00000006422 * Math.Cos(0.64684316894 + 350.3321196004 * t);
            R1 += 0.00000005006 * Math.Cos(2.37645082899 + 77.2292791221 * t);
            R1 += 0.00000005005 * Math.Cos(4.70632786971 + 460.5384408198 * t);
            R1 += 0.00000005167 * Math.Cos(5.20246616570 + 446.3113468182 * t);
            R1 += 0.00000005119 * Math.Cos(2.17338058771 + 494.7393231945 * t);
            R1 += 0.00000005025 * Math.Cos(4.21265519856 + 536.8045120954 * t);
            R1 += 0.00000004722 * Math.Cos(6.22814313946 + 63.7358983034 * t);
            R1 += 0.00000005125 * Math.Cos(5.38138329172 + 179.3106613844 * t);
            R1 += 0.00000004918 * Math.Cos(4.09031782903 + 488.3765357191 * t);
            R1 += 0.00000004652 * Math.Cos(5.10765073368 + 274.0660483248 * t);
            R1 += 0.00000004711 * Math.Cos(5.56542374115 + 42.3258213318 * t);
            R1 += 0.00000004459 * Math.Cos(1.30784829830 + 69.3649725959 * t);
            R1 += 0.00000005485 * Math.Cos(3.88088464259 + 218.4069048687 * t);
            R1 += 0.00000004416 * Math.Cos(3.05353893868 + 27.0873353739 * t);
            R1 += 0.00000004559 * Math.Cos(4.92224120952 + 285.3723810196 * t);
            R1 += 0.00000004393 * Math.Cos(4.18047835584 + 5.4166259714 * t);
            R1 += 0.00000004687 * Math.Cos(2.21401153210 + 1029.8469142605 * t);
            R1 += 0.00000004644 * Math.Cos(1.87902594973 + 1433.6199914258 * t);
            R1 += 0.00000005639 * Math.Cos(3.05596737234 + 983.1158589136 * t);
            R1 += 0.00000006045 * Math.Cos(5.68817982786 + 351.8165923087 * t);
            R1 += 0.00000004430 * Math.Cos(3.37768805833 + 377.4194549743 * t);
            R1 += 0.00000004683 * Math.Cos(2.14346624864 + 97.4155158163 * t);
            R1 += 0.00000005845 * Math.Cos(4.62301099402 + 1024.217839968 * t);
            R1 += 0.00000004536 * Math.Cos(2.45860473853 + 496.0113475817 * t);
            R1 += 0.00000004398 * Math.Cos(5.65312496227 + 3.9321532631 * t);
            R1 += 0.00000004287 * Math.Cos(0.66340266603 + 1012.6508748423 * t);
            R1 += 0.00000004086 * Math.Cos(0.14551174994 + 385.2837615005 * t);
            R1 += 0.00000004029 * Math.Cos(5.98399329775 + 178.3474535379 * t);
            R1 += 0.00000004276 * Math.Cos(3.68205082970 + 348.8476468921 * t);
            R1 += 0.00000005257 * Math.Cos(3.75263242432 + 379.8671355291 * t);
            R1 += 0.00000004012 * Math.Cos(0.42559540783 + 104313.47953065898 * t);
            R1 += 0.00000004025 * Math.Cos(2.40645188238 + 84.3428261229 * t);
            R1 += 0.00000003957 * Math.Cos(0.86846121055 + 171.9846660625 * t);
            R1 += 0.00000003961 * Math.Cos(3.04953080906 + 1017.3167412883 * t);
            R1 += 0.00000005559 * Math.Cos(0.77714806229 + 1447.8470854274 * t);
            R1 += 0.00000005071 * Math.Cos(2.61075526868 + 1536.7127656444 * t);
            R1 += 0.00000004052 * Math.Cos(5.00014006312 + 391.6465489759 * t);
            R1 += 0.00000005182 * Math.Cos(4.73444634983 + 382.8360809457 * t);
            R1 += 0.00000003763 * Math.Cos(4.29449373755 + 313.6835566709 * t);
            R1 += 0.00000004038 * Math.Cos(2.82857942788 + 1661.1461808654 * t);
            R1 += 0.00000004067 * Math.Cos(5.73169928960 + 169.5369855077 * t);
            R1 += 0.00000003841 * Math.Cos(1.62580928420 + 0.9632078465 * t);
            R1 += 0.00000003901 * Math.Cos(2.70874386576 + 14.0146456805 * t);
            R1 += 0.00000003721 * Math.Cos(1.20062375429 + 1026.8779688439 * t);
            R1 += 0.00000003911 * Math.Cos(3.01809123569 + 100.3844612329 * t);
            R1 += 0.00000003489 * Math.Cos(4.28865448963 + 1025.1810478145 * t);
            R1 += 0.00000003714 * Math.Cos(5.05021268365 + 292.4859280204 * t);
            R1 += 0.00000003816 * Math.Cos(3.93084933114 + 39.0962434843 * t);
            R1 += 0.00000003988 * Math.Cos(2.82832650224 + 134.1122628556 * t);
            R1 += 0.00000003745 * Math.Cos(4.24728135115 + 180.7951340927 * t);
            R1 += 0.00000003836 * Math.Cos(1.02685786071 + 1018.2799491348 * t);
            R1 += 0.00000003941 * Math.Cos(5.21895739331 + 183.7640795093 * t);
            R1 += 0.00000004669 * Math.Cos(4.38080962573 + 1066.49547719 * t);
            R1 += 0.00000003780 * Math.Cos(6.03723468132 + 1022.7333672597 * t);
            R1 += 0.00000003647 * Math.Cos(3.98130320367 + 608.877797677 * t);
            R1 += 0.00000003456 * Math.Cos(5.54052355058 + 846.0828347512 * t);
            R1 += 0.00000004047 * Math.Cos(3.71041480907 + 1018.0675008137 * t);
            R1 += 0.00000003865 * Math.Cos(4.76002199091 + 166.5680400911 * t);
            R1 += 0.00000003629 * Math.Cos(3.29053233846 + 447.7958195265 * t);
            R1 += 0.00000003564 * Math.Cos(4.36703678321 + 397.0631749473 * t);
            R1 += 0.00000003304 * Math.Cos(1.49289552229 + 1505.6932770074 * t);
            R1 += 0.00000003976 * Math.Cos(2.42476188945 + 106.0135355254 * t);
            R1 += 0.00000004217 * Math.Cos(4.21677652639 + 1052.2683831884 * t);
            R1 += 0.00000003294 * Math.Cos(0.42088065654 + 22.633917249 * t);
            R1 += 0.00000003615 * Math.Cos(3.68096122231 + 494.5268748734 * t);
            R1 += 0.00000003230 * Math.Cos(5.10786091356 + 69.1525242748 * t);
            R1 += 0.00000003280 * Math.Cos(3.62226152032 + 531.1754378029 * t);
            R1 += 0.00000003337 * Math.Cos(2.72502876320 + 481.4754370394 * t);
            R1 += 0.00000003187 * Math.Cos(0.08677634706 + 399.5108555021 * t);
            R1 += 0.00000003389 * Math.Cos(1.79454271219 + 1519.920371009 * t);
            R1 += 0.00000003179 * Math.Cos(3.40418030121 + 423.6292454594 * t);
            R1 += 0.00000003154 * Math.Cos(3.69356460843 + 470.2172884544 * t);
            R1 += 0.00000003706 * Math.Cos(2.79048710497 + 462.0229135281 * t);
            R1 += 0.00000003136 * Math.Cos(4.38015969606 + 385.4962098216 * t);
            R1 += 0.00000003122 * Math.Cos(0.48346644637 + 79.1868325824 * t);
            R1 += 0.00000003392 * Math.Cos(0.48037804731 + 521.0929453855 * t);
            R1 += 0.00000003465 * Math.Cos(0.93152295589 + 2183.7235989592 * t);
            R1 += 0.00000003735 * Math.Cos(0.98809808606 + 487.4133278726 * t);
            R1 += 0.00000003998 * Math.Cos(3.38773325131 + 6283.0758499914 * t);
            R1 += 0.00000002998 * Math.Cos(2.61728063127 + 487.6257761937 * t);
            R1 += 0.00000003295 * Math.Cos(2.53821501556 + 4.665866446 * t);
            R1 += 0.00000002964 * Math.Cos(3.66274645375 + 495.4900827199 * t);
            R1 += 0.00000003901 * Math.Cos(1.65463523144 + 210.3301500214 * t);
            R1 += 0.00000002950 * Math.Cos(1.99904237956 + 872.9095376942 * t);
            R1 += 0.00000002948 * Math.Cos(2.90769224206 + 391.4341006548 * t);
            R1 += 0.00000002971 * Math.Cos(0.31626092637 + 5.1078094307 * t);
            R1 += 0.00000003085 * Math.Cos(0.95725590904 + 109.9456887885 * t);
            R1 += 0.00000002995 * Math.Cos(3.34433305798 + 394.0942295307 * t);
            R1 += 0.00000003126 * Math.Cos(5.89472116854 + 105.4922706636 * t);
            R1 += 0.00000003904 * Math.Cos(3.01022809543 + 556.5176680376 * t);
            R1 += 0.00000003388 * Math.Cos(6.24936444215 + 535.3200393871 * t);
            R1 += 0.00000002930 * Math.Cos(6.15005257333 + 164.1203595363 * t);
            R1 += 0.00000003267 * Math.Cos(4.19718045293 + 518.3846323998 * t);
            R1 += 0.00000003946 * Math.Cos(2.88842759670 + 151.260118164 * t);
            R1 += 0.00000003076 * Math.Cos(6.04134449219 + 142.1408335931 * t);
            R1 += 0.00000002823 * Math.Cos(0.60712626756 + 214.7835681463 * t);
            R1 += 0.00000002917 * Math.Cos(2.74502617182 + 138.5174968707 * t);
            R1 += 0.00000003347 * Math.Cos(6.09373507569 + 6246.4272870619 * t);
            R1 += 0.00000003659 * Math.Cos(5.12211619716 + 79.2350166922 * t);
            R1 += 0.00000003010 * Math.Cos(0.24656411754 + 91.7864415238 * t);
            R1 += 0.00000002861 * Math.Cos(6.17465663902 + 422.405405182 * t);
            R1 += 0.00000002989 * Math.Cos(2.31620917965 + 485.9288551643 * t);
            R1 += 0.00000003088 * Math.Cos(2.29186342974 + 110.2063212194 * t);
            R1 += 0.00000003030 * Math.Cos(3.69866149100 + 532.6117264014 * t);
            R1 += 0.00000003020 * Math.Cos(2.36422658177 + 290.2195580194 * t);
            R1 += 0.00000003170 * Math.Cos(1.23078934548 + 10176.6369832815 * t);
            R1 += 0.00000002652 * Math.Cos(3.35836234807 + 148.0787244263 * t);
            R1 += 0.00000002673 * Math.Cos(6.03366372927 + 196.5067008026 * t);
            R1 += 0.00000002630 * Math.Cos(0.46957619348 + 1970.4245035212 * t);
            R1 += 0.00000002599 * Math.Cos(4.86022081674 + 439.1977998174 * t);
            R1 += 0.00000002878 * Math.Cos(2.61946597178 + 488.5889840402 * t);
            R1 += 0.00000002720 * Math.Cos(1.71836225398 + 364.559213602 * t);
            R1 += 0.00000003333 * Math.Cos(3.25126857354 + 30.0562807905 * t);
            R1 += 0.00000003053 * Math.Cos(2.49346960035 + 6243.4583416453 * t);
            R1 += 0.00000003062 * Math.Cos(6.23776299963 + 419.4846438752 * t);
            R1 += 0.00000002786 * Math.Cos(0.83078219939 + 497.1870037493 * t);
            R1 += 0.00000002834 * Math.Cos(3.52926079424 + 457.8783119439 * t);
            R1 += 0.00000002932 * Math.Cos(1.80245810977 + 500.1559491659 * t);
            R1 += 0.00000003030 * Math.Cos(5.10152500393 + 367.9701020033 * t);
            R1 += 0.00000002956 * Math.Cos(5.76230870725 + 986.0848043302 * t);
            R1 += 0.00000003116 * Math.Cos(2.20042242739 + 495.702531041 * t);
            R1 += 0.00000002554 * Math.Cos(0.65945973992 + 67.3592350258 * t);
            R1 += 0.00000002901 * Math.Cos(3.91891656185 + 10173.6680378649 * t);
            R1 += 0.00000002840 * Math.Cos(1.34453183591 + 482.9599097477 * t);
            R1 += 0.00000002458 * Math.Cos(1.20012815574 + 489.110248902 * t);
            R1 += 0.00000002556 * Math.Cos(3.86921927085 + 487.1045113319 * t);
            R1 += 0.00000002614 * Math.Cos(1.51881085312 + 463.5073862364 * t);
            R1 += 0.00000002386 * Math.Cos(4.58400538443 + 615.9913446778 * t);
            R1 += 0.00000002438 * Math.Cos(5.19827220476 + 501.1191570124 * t);
            R1 += 0.00000002537 * Math.Cos(1.64802783144 + 519.6084726772 * t);
            R1 += 0.00000002444 * Math.Cos(3.87859489652 + 185.2485522176 * t);
            R1 += 0.00000002795 * Math.Cos(4.04265752580 + 255.0554677982 * t);
            R1 += 0.00000002895 * Math.Cos(3.26202698812 + 1646.9190868638 * t);
            R1 += 0.00000002225 * Math.Cos(5.75197574692 + 605.9570363702 * t);
            R1 += 0.00000002324 * Math.Cos(3.99503920129 + 481.2629887183 * t);
            R1 += 0.00000002962 * Math.Cos(1.74151265966 + 2080.6308247406 * t);
            R1 += 0.00000002621 * Math.Cos(1.74442251671 + 35.212274331 * t);
            return R1 * t;
        }

        internal static double Neptune_R2(double t) // 72 terms of order 2
        {
            double R2 = 0;
            R2 += 0.00004247412 * Math.Cos(5.89910679117 + 38.1330356378 * t);
            R2 += 0.00000217570 * Math.Cos(0.34581829080 + 1.4844727083 * t);
            R2 += 0.00000163025 * Math.Cos(2.23872947130 + 168.0525127994 * t);
            R2 += 0.00000156285 * Math.Cos(4.59414467342 + 182.279606801 * t);
            R2 += 0.00000117940 * Math.Cos(5.10295026024 + 484.444382456 * t);
            R2 += 0.00000112429 * Math.Cos(1.19000583596 + 498.6714764576 * t);
            R2 += 0.00000127141 * Math.Cos(2.84786298079 + 35.1640902212 * t);
            R2 += 0.00000099467 * Math.Cos(3.41578558739 + 175.1660598002 * t);
            R2 += 0.00000064814 * Math.Cos(3.46214064840 + 388.4651552382 * t);
            R2 += 0.00000077286 * Math.Cos(0.01659281785 + 491.5579294568 * t);
            R2 += 0.00000049509 * Math.Cos(4.06995509133 + 76.2660712756 * t);
            R2 += 0.00000039330 * Math.Cos(6.09521855958 + 1021.2488945514 * t);
            R2 += 0.00000036450 * Math.Cos(5.17130059988 + 137.0330241624 * t);
            R2 += 0.00000037080 * Math.Cos(5.97288967681 + 2.9689454166 * t);
            R2 += 0.00000030484 * Math.Cos(3.58259801313 + 33.6796175129 * t);
            R2 += 0.00000021099 * Math.Cos(0.76843555176 + 36.6485629295 * t);
            R2 += 0.00000013886 * Math.Cos(3.59248623971 + 395.578702239 * t);
            R2 += 0.00000013117 * Math.Cos(5.09263515697 + 98.8999885246 * t);
            R2 += 0.00000011379 * Math.Cos(1.18060018898 + 381.3516082374 * t);
            R2 += 0.00000009132 * Math.Cos(2.34787658568 + 601.7642506762 * t);
            R2 += 0.00000008527 * Math.Cos(5.25134685897 + 2.4476805548 * t);
            R2 += 0.00000008136 * Math.Cos(4.96270726986 + 4.4534181249 * t);
            R2 += 0.00000007417 * Math.Cos(4.46775409796 + 189.3931538018 * t);
            R2 += 0.00000007225 * Math.Cos(1.92287508629 + 9.5612275556 * t);
            R2 += 0.00000007289 * Math.Cos(1.65519525780 + 1028.3624415522 * t);
            R2 += 0.00000008076 * Math.Cos(5.84268048311 + 220.4126424388 * t);
            R2 += 0.00000009654;
            R2 += 0.00000006554 * Math.Cos(0.69397520733 + 144.1465711632 * t);
            R2 += 0.00000007782 * Math.Cos(1.14341656235 + 1059.3819301892 * t);
            R2 += 0.00000005665 * Math.Cos(6.25378258571 + 74.7815985673 * t);
            R2 += 0.00000005628 * Math.Cos(5.23383764266 + 46.2097904851 * t);
            R2 += 0.00000005523 * Math.Cos(4.59041448911 + 1014.1353475506 * t);
            R2 += 0.00000005177 * Math.Cos(5.23116646157 + 477.3308354552 * t);
            R2 += 0.00000005503 * Math.Cos(3.49522319102 + 183.7640795093 * t);
            R2 += 0.00000004878 * Math.Cos(3.52934357721 + 39.6175083461 * t);
            R2 += 0.00000004787 * Math.Cos(2.08260524745 + 41.1019810544 * t);
            R2 += 0.00000005055 * Math.Cos(0.19949888617 + 166.5680400911 * t);
            R2 += 0.00000004751 * Math.Cos(1.18054948270 + 169.5369855077 * t);
            R2 += 0.00000004747 * Math.Cos(1.50608965076 + 73.297125859 * t);
            R2 += 0.00000006113 * Math.Cos(6.18326155595 + 71.8126531507 * t);
            R2 += 0.00000004606 * Math.Cos(3.91970908886 + 587.5371566746 * t);
            R2 += 0.00000005756 * Math.Cos(2.23667359233 + 176.6505325085 * t);
            R2 += 0.00000004536 * Math.Cos(2.84337336954 + 7.1135470008 * t);
            R2 += 0.00000004338 * Math.Cos(0.51553847388 + 446.3113468182 * t);
            R2 += 0.00000003891 * Math.Cos(0.26338839265 + 1550.939859646 * t);
            R2 += 0.00000004465 * Math.Cos(3.01487041298 + 129.9194771616 * t);
            R2 += 0.00000003727 * Math.Cos(2.37977930658 + 160.9389657986 * t);
            R2 += 0.00000003840 * Math.Cos(3.79290381880 + 111.4301614968 * t);
            R2 += 0.00000004142 * Math.Cos(1.70293820961 + 983.1158589136 * t);
            R2 += 0.00000003296 * Math.Cos(1.07748822909 + 505.7850234584 * t);
            R2 += 0.00000004008 * Math.Cos(0.30663868827 + 494.7393231945 * t);
            R2 += 0.00000003974 * Math.Cos(5.97351783840 + 488.3765357191 * t);
            R2 += 0.00000003925 * Math.Cos(4.85736421123 + 60.7669528868 * t);
            R2 += 0.00000002966 * Math.Cos(2.01608546009 + 822.176893115 * t);
            R2 += 0.00000003972 * Math.Cos(1.07780371834 + 374.2380612366 * t);
            R2 += 0.00000003843 * Math.Cos(5.23002047199 + 350.3321196004 * t);
            R2 += 0.00000002848 * Math.Cos(6.17799253802 + 704.8570248948 * t);
            R2 += 0.00000003527 * Math.Cos(0.79317138165 + 274.0660483248 * t);
            R2 += 0.00000002828 * Math.Cos(1.32275775835 + 386.9806825299 * t);
            R2 += 0.00000002773 * Math.Cos(5.37132330836 + 251.4321310758 * t);
            R2 += 0.00000003113 * Math.Cos(5.12622288690 + 426.598190876 * t);
            R2 += 0.00000003344 * Math.Cos(5.61433537548 + 1124.34166877 * t);
            R2 += 0.00000002597 * Math.Cos(0.67759426519 + 312.1990839626 * t);
            R2 += 0.00000002581 * Math.Cos(3.55847612121 + 567.8240007324 * t);
            R2 += 0.00000002578 * Math.Cos(1.45603792456 + 1035.475988553 * t);
            R2 += 0.00000002541 * Math.Cos(5.19427579702 + 1227.4344429886 * t);
            R2 += 0.00000002510 * Math.Cos(4.12148891512 + 171.2339065371 * t);
            R2 += 0.00000002511 * Math.Cos(2.71606957319 + 179.0982130633 * t);
            R2 += 0.00000002342 * Math.Cos(0.96469916587 + 1019.7644218431 * t);
            R2 += 0.00000002500 * Math.Cos(0.70282276030 + 707.7777862016 * t);
            R2 += 0.00000002480 * Math.Cos(4.59623030219 + 693.55069220 * t);
            R2 += 0.00000002253 * Math.Cos(0.74334306011 + 976.0023119128 * t);
            return R2 * t * t;
        }

        internal static double Neptune_R3(double t) // 22 terms of order 3
        {
            double R3 = 0;
            R3 += 0.00000166297 * Math.Cos(4.55243893489 + 38.1330356378 * t);
            R3 += 0.00000022380 * Math.Cos(3.94830879358 + 168.0525127994 * t);
            R3 += 0.00000021348 * Math.Cos(2.86296778794 + 182.279606801 * t);
            R3 += 0.00000016233 * Math.Cos(0.54226725872 + 484.444382456 * t);
            R3 += 0.00000015623 * Math.Cos(5.75702251906 + 498.6714764576 * t);
            R3 += 0.00000011867 * Math.Cos(4.40280192710 + 1.4844727083 * t);
            R3 += 0.00000006448 * Math.Cos(5.19003066847 + 31.019488637 * t);
            R3 += 0.00000003655 * Math.Cos(5.91335292846 + 1007.0218005498 * t);
            R3 += 0.00000003681 * Math.Cos(1.62865545676 + 388.4651552382 * t);
            R3 += 0.00000003198 * Math.Cos(0.70197118575 + 1558.0534066468 * t);
            R3 += 0.00000003243 * Math.Cos(1.88035665980 + 522.5774180938 * t);
            R3 += 0.00000003269 * Math.Cos(2.94301808574 + 76.2660712756 * t);
            R3 += 0.00000002688 * Math.Cos(1.87062743473 + 402.6922492398 * t);
            R3 += 0.00000003246 * Math.Cos(0.79381356193 + 536.8045120954 * t);
            R3 += 0.00000002650 * Math.Cos(5.76858449026 + 343.2185725996 * t);
            R3 += 0.00000002644 * Math.Cos(4.64542905401 + 500.1559491659 * t);
            R3 += 0.00000002541 * Math.Cos(4.79217120822 + 482.9599097477 * t);
            R3 += 0.00000002523 * Math.Cos(1.72869889780 + 395.578702239 * t);
            R3 += 0.00000002690 * Math.Cos(2.21096415618 + 446.3113468182 * t);
            R3 += 0.00000002355 * Math.Cos(5.77381398401 + 485.9288551643 * t);
            R3 += 0.00000002874 * Math.Cos(6.19643340540 + 815.0633461142 * t);
            R3 += 0.00000002278 * Math.Cos(3.66579603119 + 497.1870037493 * t);
            return R3 * t * t * t;
        }

        internal static double Neptune_R4(double t) // 7 terms of order 4
        {
            double R4 = 0;
            R4 += 0.00000004227 * Math.Cos(2.40375758563 + 477.3308354552 * t);
            R4 += 0.00000004333 * Math.Cos(0.10459484545 + 395.578702239 * t);
            R4 += 0.00000003545 * Math.Cos(4.78431259422 + 1028.3624415522 * t);
            R4 += 0.00000003154 * Math.Cos(3.88192942366 + 505.7850234584 * t);
            R4 += 0.00000003016 * Math.Cos(1.03609346831 + 189.3931538018 * t);
            R4 += 0.00000002294 * Math.Cos(1.10879658603 + 182.279606801 * t);
            R4 += 0.00000002295 * Math.Cos(5.67776133184 + 168.0525127994 * t);
            return R4 * t * t * t * t;
        }

        internal static double Neptune_R5(double t) // 1 term of order 5
        {
            double R5 = 0;
            R5 += 0.00000000000;
            return R5 * t * t * t * t * t;
        }

        #endregion
    }
}
