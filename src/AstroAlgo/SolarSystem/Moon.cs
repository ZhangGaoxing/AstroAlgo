using AstroAlgo.Basic;
using AstroAlgo.Models;

using System;

namespace AstroAlgo.SolarSystem
{
    /// <summary>
    /// 月球
    /// </summary>
    public class Moon
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
                return CoordinateSystem.Ecliptic2Equator(Ecliptic, DateTime.Now, true);
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
                var e = CoordinateSystem.Ecliptic2Equator(e0, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, 0.125, latitude, longitude, DateTime.Now, localZone);

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
                var e = CoordinateSystem.Ecliptic2Equator(e0, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, 0.125, latitude, longitude, DateTime.Now, localZone);

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
                var e = CoordinateSystem.Ecliptic2Equator(e0, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), true);
                var time = CoordinateSystem.ElevationAngle2Time(e, 0.125, latitude, longitude, DateTime.Now, localZone);

                var span = TimeSpan.FromHours(time[1] / 15.0);

                return new TimeSpan(span.Hours, span.Minutes, span.Seconds);
            }
        }

        /// <summary>
        /// 当前月地距离(km)
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
        /// 初始化一个月球实例
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <param name="localZone">时区</param>
        public Moon(double latitude, double longitude, TimeZoneInfo localZone)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.localZone = localZone;
        }

        #region Method

        /// <summary>
        /// 计算指定时间的月球黄经与黄纬
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="isApparent">是否为视黄道坐标</param>
        /// <returns>黄道坐标</returns>
        public static Ecliptic EclipticalCoordinate(DateTime time, bool isApparent = false)
        {
            double T = (Julian.ToJulianDay(time) - 2451545) / 36525.0;
            // 月球平黄经
            double L1 = 218.3164591 + 481267.88134236 * T - 0.0013268 * T * T + T * T * T / 538841.0 - T * T * T * T / 65194000;
            // 月日距离
            double D = 297.8502042 + 445267.1115168 * T - 0.0016300 * T * T + T * T * T / 545868.0 - T * T * T * T / 113065000.0;
            // 太阳平近点角
            double M = 357.5291092 + 35999.0502909 * T - 0.0001536 * T * T + T * T * T / 24490000.0;
            // 月球平近点角
            double M1 = 134.9634114 + 477198.8676313 * T + 0.0089970 * T * T + T * T * T / 69699.0 - T * T * T * T / 14712000.0;
            // 月球精度参数
            double F = 93.2720993 + 483202.0175273 * T - 0.0034029 * T * T - T * T * T / 3526000.0 + T * T * T * T / 863310000.0;
            // 修正参数
            double A1 = 119.75 + 131.849 * T;
            double A2 = 53.09 + 479264.290 * T;
            double A3 = 313.45 + 481266.484 * T;
            double E = 1 - 0.002516 * T - 0.0000074 * T * T;

            #region 化简
            if (L1 <= 0)
            {
                while (!(L1 >= 0 && L1 <= 360))
                {
                    L1 += 360;
                }
            }
            else
            {
                while (!(L1 >= 0 && L1 <= 360))
                {
                    L1 -= 360;
                }
            }

            if (D <= 0)
            {
                while (!(D >= 0 && D <= 360))
                {
                    D += 360;
                }
            }
            else
            {
                while (!(D >= 0 && D <= 360))
                {
                    D -= 360;
                }
            }

            if (M <= 0)
            {
                while (!(M >= 0 && M <= 360))
                {
                    M += 360;
                }
            }
            else
            {
                while (!(M >= 0 && M <= 360))
                {
                    M -= 360;
                }
            }

            if (M1 <= 0)
            {
                while (!(M1 >= 0 && M1 <= 360))
                {
                    M1 += 360;
                }
            }
            else
            {
                while (!(M1 >= 0 && M1 <= 360))
                {
                    M1 -= 360;
                }
            }

            if (F <= 0)
            {
                while (!(F >= 0 && F <= 360))
                {
                    F += 360;
                }
            }
            else
            {
                while (!(F >= 0 && F <= 360))
                {
                    F -= 360;
                }
            }

            if (A1 <= 0)
            {
                while (!(A1 >= 0 && A1 <= 360))
                {
                    A1 += 360;
                }
            }
            else
            {
                while (!(A1 >= 0 && A1 <= 360))
                {
                    A1 -= 360;
                }
            }

            if (A2 <= 0)
            {
                while (!(A2 >= 0 && A2 <= 360))
                {
                    A2 += 360;
                }
            }
            else
            {
                while (!(A2 >= 0 && A2 <= 360))
                {
                    A2 -= 360;
                }
            }

            if (A3 <= 0)
            {
                while (!(A3 >= 0 && A3 <= 360))
                {
                    A3 += 360;
                }
            }
            else
            {
                while (!(A3 >= 0 && A3 <= 360))
                {
                    A3 -= 360;
                }
            }
            #endregion

            #region SigmaI
            double SigmaI = 6288744 * Math.Sin(M1 * (Math.PI / 180.0)) +
                1274027 * Math.Sin((2 * D - M1) * (Math.PI / 180.0)) +
                658314 * Math.Sin((2 * D) * (Math.PI / 180.0)) +
                213618 * Math.Sin((2 * M1) * (Math.PI / 180.0)) -
                185116 * E * Math.Sin(M * (Math.PI / 180.0)) -
                114332 * Math.Sin((2 * F) * (Math.PI / 180.0)) +
                58793 * Math.Sin((2 * D - 2 * M1) * (Math.PI / 180.0)) +
                57066 * E * Math.Sin((2 * D - M - M1) * (Math.PI / 180.0)) +
                53322 * Math.Sin((2 * D + M1) * (Math.PI / 180.0)) +
                45758 * E * Math.Sin((2 * D - M) * (Math.PI / 180.0)) -
                40923 * E * Math.Sin((M - M1) * (Math.PI / 180.0)) -
                34720 * Math.Sin(D * (Math.PI / 180.0)) -
                30383 * E * Math.Sin((M + M1) * (Math.PI / 180.0)) +
                15327 * Math.Sin((2 * D - 2 * F) * (Math.PI / 180.0)) -
                12528 * Math.Sin((M1 + 2 * F) * (Math.PI / 180.0)) +
                10980 * Math.Sin((M1 - 2 * F) * (Math.PI / 180.0)) +
                10675 * Math.Sin((4 * D - M1) * (Math.PI / 180.0)) +
                10034 * Math.Sin((3 * M1) * (Math.PI / 180.0)) +
                8548 * Math.Sin((4 * D - 2 * M1) * (Math.PI / 180.0)) -
                7888 * E * Math.Sin((2 * D + M - M1) * (Math.PI / 180.0)) -
                6766 * E * Math.Sin((2 * D + M) * (Math.PI / 180.0)) -
                5163 * Math.Sin((D - M1) * (Math.PI / 180.0)) +
                4987 * E * Math.Sin((D + M) * (Math.PI / 180.0)) +
                4036 * E * Math.Sin((2 * D - M + M1) * (Math.PI / 180.0)) +
                3994 * Math.Sin((2 * D + 2 * M1) * (Math.PI / 180.0)) +
                3861 * Math.Sin((4 * D) * (Math.PI / 180.0)) +
                3665 * Math.Sin((2 * D - 3 * M1) * (Math.PI / 180.0)) -
                2689 * E * Math.Sin((M - 2 * M1) * (Math.PI / 180.0)) -
                2602 * Math.Sin((2 * D - M1 + 2 * F) * (Math.PI / 180.0)) +
                2390 * E * Math.Sin((2 * D - M - 2 * M1) * (Math.PI / 180.0)) -
                2348 * Math.Sin((D + M1) * (Math.PI / 180.0)) +
                2236 * E * E * Math.Sin((2 * D - 2 * M) * (Math.PI / 180.0)) -
                2120 * E * Math.Sin((M + 2 * M1) * (Math.PI / 180.0)) -
                2069 * E * E * Math.Sin((2 * M) * (Math.PI / 180.0)) +
                2048 * E * E * Math.Sin((2 * D - 2 * M - M1) * (Math.PI / 180.0)) -
                1773 * Math.Sin((2 * D + M1 - 2 * F) * (Math.PI / 180.0)) -
                1595 * Math.Sin((2 * D + 2 * F) * (Math.PI / 180.0)) +
                1215 * E * Math.Sin((4 * D - M - M1) * (Math.PI / 180.0)) -
                1110 * Math.Sin((2 * M1 + 2 * F) * (Math.PI / 180.0)) -
                892 * Math.Sin((3 * D - M1) * (Math.PI / 180.0)) -
                810 * E * Math.Sin((2 * D + M + M1) * (Math.PI / 180.0)) +
                759 * E * Math.Sin((4 * D - M - 2 * M1) * (Math.PI / 180.0)) -
                713 * E * Math.Sin((2 * M - M1) * (Math.PI / 180.0)) -
                700 * E * Math.Sin((2 * D + 2 * M - M1) * (Math.PI / 180.0)) +
                691 * E * Math.Sin((2 * D + M - 2 * M1) * (Math.PI / 180.0)) +
                596 * E * Math.Sin((2 * D - M - 2 * F) * (Math.PI / 180.0)) +
                549 * Math.Sin((4 * D + M1) * (Math.PI / 180.0)) +
                537 * Math.Sin((4 * M1) * (Math.PI / 180.0)) +
                520 * E * Math.Sin((4 * D - M) * (Math.PI / 180.0)) -
                487 * Math.Sin((D - 2 * M1) * (Math.PI / 180.0)) -
                399 * E * Math.Sin((2 * D + M - 2 * F) * (Math.PI / 180.0)) -
                381 * Math.Sin((2 * M1 - 2 * F) * (Math.PI / 180.0)) +
                351 * E * Math.Sin((D + M + M1) * (Math.PI / 180.0)) -
                340 * Math.Sin((3 * D - 2 * M1) * (Math.PI / 180.0)) +
                330 * Math.Sin((4 * D - 3 * M1) * (Math.PI / 180.0)) +
                327 * E * Math.Sin((2 * D - M + 2 * M1) * (Math.PI / 180.0)) -
                323 * E * Math.Sin((2 * M + M1) * (Math.PI / 180.0)) +
                299 * E * Math.Sin((D + M - M1) * (Math.PI / 180.0)) +
                294 * Math.Sin((2 * D + 3 * M1) * (Math.PI / 180.0)) +
                0 * Math.Sin((2 * D - M1 - 2 * F) * (Math.PI / 180.0)) +
                (3958 * Math.Sin(A1 * (Math.PI / 180.0)) + 1962 * Math.Sin((L1 - F) * (Math.PI / 180.0)) + 318 * Math.Sin(A2 * (Math.PI / 180.0)));
            #endregion

            #region SigmaB
            double SigmaB = 5128122 * Math.Sin(F * (Math.PI / 180.0)) +
                280602 * Math.Sin((M1 + F) * (Math.PI / 180.0)) +
                277693 * Math.Sin((M1 - F) * (Math.PI / 180.0)) +
                173237 * Math.Sin((2 * D - F) * (Math.PI / 180.0)) +
                55413 * Math.Sin((2 * D - M1 + F) * (Math.PI / 180.0)) +
                46271 * Math.Sin((2 * D - M1 - F) * (Math.PI / 180.0)) +
                32573 * Math.Sin((2 * D + F) * (Math.PI / 180.0)) +
                17198 * Math.Sin((2 * M1 + F) * (Math.PI / 180.0)) +
                9266 * Math.Sin((2 * D + M1 - F) * (Math.PI / 180.0)) +
                8822 * Math.Sin((2 * M1 - F) * (Math.PI / 180.0)) +
                8216 * E * Math.Sin((2 * D - M - F) * (Math.PI / 180.0)) +
                4324 * Math.Sin((2 * D - 2 * M1 - F) * (Math.PI / 180.0)) +
                4200 * Math.Sin((2 * D + M1 + F) * (Math.PI / 180.0)) -
                3359 * E * Math.Sin((2 * D + M - F) * (Math.PI / 180.0)) +
                2463 * E * Math.Sin((2 * D - M - M1 + F) * (Math.PI / 180.0)) +
                2211 * E * Math.Sin((2 * D - M + F) * (Math.PI / 180.0)) +
                2065 * E * Math.Sin((2 * D - M - M1 - F) * (Math.PI / 180.0)) -
                1870 * E * Math.Sin((M - M1 - F) * (Math.PI / 180.0)) +
                1828 * Math.Sin((4 * D - M1 - F) * (Math.PI / 180.0)) -
                1794 * E * Math.Sin((M + F) * (Math.PI / 180.0)) -
                1749 * Math.Sin((3 * F) * (Math.PI / 180.0)) -
                1565 * E * Math.Sin((M - M1 + F) * (Math.PI / 180.0)) -
                1491 * Math.Sin((D + F) * (Math.PI / 180.0)) -
                1475 * E * Math.Sin((M + M1 + F) * (Math.PI / 180.0)) -
                1410 * E * Math.Sin((M + M1 - F) * (Math.PI / 180.0)) -
                1344 * E * Math.Sin((M - F) * (Math.PI / 180.0)) -
                1335 * Math.Sin((D - F) * (Math.PI / 180.0)) +
                1107 * Math.Sin((3 * M1 + F) * (Math.PI / 180.0)) +
                1021 * Math.Sin((4 * D - F) * (Math.PI / 180.0)) +
                833 * Math.Sin((4 * D - M1 + F) * (Math.PI / 180.0)) +
                777 * Math.Sin((M1 - 3 * F) * (Math.PI / 180.0)) +
                671 * Math.Sin((4 * D - 2 * M1+F) * (Math.PI / 180.0)) +
                607 * Math.Sin((2 * D - 3 * F) * (Math.PI / 180.0)) +
                596 * Math.Sin((2 * D + 2 * M1 - F) * (Math.PI / 180.0)) +
                491 * E * Math.Sin((2 * D - M + M1 - F) * (Math.PI / 180.0)) -
                451 * Math.Sin((2 * D - 2 * M1 + F) * (Math.PI / 180.0)) +
                439 * Math.Sin((3 * M1 - F) * (Math.PI / 180.0)) +
                422 * Math.Sin((2 * D + 2 * M1 + F) * (Math.PI / 180.0)) +
                421 * Math.Sin((2 * D - 3 * M1 - F) * (Math.PI / 180.0)) -
                366 * E * Math.Sin((2 * D + M - M1 + F) * (Math.PI / 180.0)) -
                351 * E * Math.Sin((2 * D + M + F) * (Math.PI / 180.0)) +
                331 * Math.Sin((4 * D + F) * (Math.PI / 180.0)) +
                315 * E * Math.Sin((2 * D - M + M1 + F) * (Math.PI / 180.0)) +
                302 * E * E * Math.Sin((2 * D - 2 * M - F) * (Math.PI / 180.0)) -
                283 * Math.Sin((M1 + 3 * F) * (Math.PI / 180.0)) -
                229 * E * Math.Sin((2 * D + M + M1 - F) * (Math.PI / 180.0)) +
                223 * E * Math.Sin((D + M - F) * (Math.PI / 180.0)) +
                223 * E * Math.Sin((D + M + F) * (Math.PI / 180.0)) -
                220 * E * Math.Sin((M - 2 * M1 - F) * (Math.PI / 180.0)) -
                220 * E * Math.Sin((2 * D + M - M1 - F) * (Math.PI / 180.0)) -
                185 * Math.Sin((D + M1 + F) * (Math.PI / 180.0)) +
                181 * E * Math.Sin((2 * D - M - 2 * M1 - F) * (Math.PI / 180.0)) -
                177 * E * Math.Sin((M + 2 * M1 + F) * (Math.PI / 180.0)) +
                176 * Math.Sin((4 * D - 2 * M1 - F) * (Math.PI / 180.0)) +
                166 * E * Math.Sin((4 * D - M - M1 - F) * (Math.PI / 180.0)) -
                164 * Math.Sin((D + M1 - F) * (Math.PI / 180.0)) +
                132 * Math.Sin((4 * D + M1 - F) * (Math.PI / 180.0)) -
                119 * Math.Sin((D - M1 - F) * (Math.PI / 180.0)) +
                115 * E * Math.Sin((4 * D - M - F) * (Math.PI / 180.0)) +
                107 * E * E * Math.Sin((2 * D - 2 * M + F) * (Math.PI / 180.0)) +
                (-2235 * Math.Sin(L1 * (Math.PI / 180.0)) + 382 * Math.Sin(A3 * (Math.PI / 180.0)) + 175 * Math.Sin((A1 - F) * (Math.PI / 180.0)) + 175 * Math.Sin((A1 + F) * (Math.PI / 180.0)) + 127 * Math.Sin((L1 - M1) * (Math.PI / 180.0)) - 115 * Math.Sin((L1 + M1) * (Math.PI / 180.0)));
            #endregion

            Ecliptic e;

            if (!isApparent)
            {
               double longitude = L1 + SigmaI / 1000000.0;
               double latitude = SigmaB / 1000000.0;

               if (longitude < 0)
               {
                   longitude = 360 + longitude;
               }

                e = new Ecliptic()
                {
                    Longitude = longitude,
                    Latitude = latitude
                };
            }
            else
            {
                var n = CoordinateSystem.GetNutation(time);

                double longitude = (L1 + SigmaI / 1000000.0) + n.Longitude;
                double latitude = SigmaB / 1000000.0;

                if (longitude < 0)
                {
                    longitude = 360 + longitude;
                }

                e = new Ecliptic()
                {
                    Longitude = longitude,
                    Latitude = latitude
                };
            }

            return e;
        }

        /// <summary>
        /// 计算指定时间的地月距离（km）
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>距离，单位为千米</returns>
        public static double ToEarthDistance(DateTime time)
        {
            double T = (Julian.ToJulianDay(time) - 2451545) / 36525.0;
            // 月球平黄经
            double L1 = 218.3164591 + 481267.88134236 * T - 0.0013268 * T * T + T * T * T / 538841.0 - T * T * T * T / 65194000;
            // 月日距离
            double D = 297.8502042 + 445267.1115168 * T - 0.0016300 * T * T + T * T * T / 545868.0 - T * T * T * T / 113065000.0;
            // 太阳平近点角
            double M = 357.5291092 + 35999.0502909 * T - 0.0001536 * T * T + T * T * T / 24490000.0;
            // 月球平近点角
            double M1 = 134.9634114 + 477198.8676313 * T + 0.0089970 * T * T + T * T * T / 69699.0 - T * T * T * T / 14712000.0;
            // 月球精度参数
            double F = 93.2720993 + 483202.0175273 * T - 0.0034029 * T * T - T * T * T / 3526000.0 + T * T * T * T / 863310000.0;
            // 修正参数
            double E = 1 - 0.002516 * T - 0.0000074 * T * T;

            #region SigmaR
            double SigmaR = -20905355 * Math.Cos(M1 * (Math.PI / 180.0)) -
                3699111 * Math.Cos((2 * D - M1) * (Math.PI / 180.0)) -
                2955968 * Math.Cos((2 * D) * (Math.PI / 180.0)) -
                569925 * Math.Cos((2 * M1) * (Math.PI / 180.0)) +
                48888 * E * Math.Cos(M * (Math.PI / 180.0)) -
                3149 * Math.Cos((2 * F) * (Math.PI / 180.0)) +
                246158 * Math.Cos((2 * D - 2 * M1) * (Math.PI / 180.0)) -
                152138 * E * Math.Cos((2 * D - M - M1) * (Math.PI / 180.0)) -
                170733 * Math.Cos((2 * D + M1) * (Math.PI / 180.0)) -
                204586 * E * Math.Cos((2 * D - M) * (Math.PI / 180.0)) -
                129620 * E * Math.Cos((M - M1) * (Math.PI / 180.0)) +
                108743 * Math.Cos(D * (Math.PI / 180.0)) +
                104755 * E * Math.Cos((M + M1) * (Math.PI / 180.0)) +
                10321 * Math.Cos((2 * D - 2 * F) * (Math.PI / 180.0)) +
                0 * Math.Cos((M1 + 2 * F) * (Math.PI / 180.0)) +
                79661 * Math.Cos((M1 - 2 * F) * (Math.PI / 180.0)) -
                34782 * Math.Cos((4 * D - M1) * (Math.PI / 180.0)) -
                23210 * Math.Cos((3 * M1) * (Math.PI / 180.0)) -
                21636 * Math.Cos((4 * D - 2 * M1) * (Math.PI / 180.0)) +
                24208 * E * Math.Cos((2 * D + M - M1) * (Math.PI / 180.0)) +
                30824 * E * Math.Cos((2 * D + M) * (Math.PI / 180.0)) -
                8379 * Math.Cos((D - M1) * (Math.PI / 180.0)) -
                16675 * E * Math.Cos((D + M) * (Math.PI / 180.0)) -
                12831 * E * Math.Cos((2 * D - M + M1) * (Math.PI / 180.0)) -
                10445 * Math.Cos((2 * D + 2 * M1) * (Math.PI / 180.0)) -
                11650 * Math.Cos((4 * D) * (Math.PI / 180.0)) +
                14403 * Math.Cos((2 * D - 3 * M1) * (Math.PI / 180.0)) -
                7003 * E * Math.Cos((M - 2 * M1) * (Math.PI / 180.0)) +
                0 * Math.Cos((2 * D - M1 + 2 * F) * (Math.PI / 180.0)) +
                10056 * E * Math.Cos((2 * D - M - 2 * M1) * (Math.PI / 180.0)) +
                6322 * Math.Cos((D + M1) * (Math.PI / 180.0)) -
                9884 * E * E * Math.Cos((2 * D - 2 * M) * (Math.PI / 180.0)) +
                5751 * E * Math.Cos((M + 2 * M1) * (Math.PI / 180.0)) +
                0 * E * E * Math.Cos((2 * M) * (Math.PI / 180.0)) -
                4950 * E * E * Math.Cos((2 * D - 2 * M - M1) * (Math.PI / 180.0)) +
                4130 * Math.Cos((2 * D + M1 - 2 * F) * (Math.PI / 180.0)) +
                0 * Math.Cos((2 * D + 2 * F) * (Math.PI / 180.0)) -
                3958 * E * Math.Cos((4 * D - M - M1) * (Math.PI / 180.0)) +
                0 * Math.Cos((2 * M1 + 2 * F) * (Math.PI / 180.0)) +
                3258 * Math.Cos((3 * D - M1) * (Math.PI / 180.0)) +
                2616 * E * Math.Cos((2 * D + M + M1) * (Math.PI / 180.0)) -
                1897 * E * Math.Cos((4 * D - M - 2 * M1) * (Math.PI / 180.0)) -
                2117 * E * E * Math.Cos((2 * M - M1) * (Math.PI / 180.0)) +
                2354 * E * E * Math.Cos((2 * D + 2 * M - M1) * (Math.PI / 180.0)) +
                0 * E * Math.Cos((2 * D + M - 2 * M1) * (Math.PI / 180.0)) +
                0 * E * Math.Cos((2 * D - M - 2 * F) * (Math.PI / 180.0)) -
                1423 * Math.Cos((4 * D + M1) * (Math.PI / 180.0)) -
                1117 * Math.Cos((4 * M1) * (Math.PI / 180.0)) -
                1571 * E * Math.Cos((4 * D - M) * (Math.PI / 180.0)) -
                1739 * Math.Cos((D - 2 * M1) * (Math.PI / 180.0)) +
                0 * E * Math.Cos((2 * D + M - 2 * F) * (Math.PI / 180.0)) -
                4421 * Math.Cos((2 * M1 - 2 * F) * (Math.PI / 180.0)) +
                0 * E * Math.Cos((D + M + M1) * (Math.PI / 180.0)) +
                0 * Math.Cos((3 * D - 2 * M1) * (Math.PI / 180.0)) +
                0 * Math.Cos((4 * D - 3 * M1) * (Math.PI / 180.0)) +
                0 * E * Math.Cos((2 * D - M + 2 * M1) * (Math.PI / 180.0)) +
                1165 * E * E * Math.Cos((2 * M + M1) * (Math.PI / 180.0)) +
                0 * E * Math.Cos((D + M - M1) * (Math.PI / 180.0)) +
                0 * Math.Cos((2 * D + 3 * M1) * (Math.PI / 180.0)) +
                8752 * Math.Cos((2 * D - M1 - 2 * F) * (Math.PI / 180.0));
            #endregion

            double distance = 385000.56 + SigmaR / 1000.0;

            return distance;
        }

        #endregion
    }
}
