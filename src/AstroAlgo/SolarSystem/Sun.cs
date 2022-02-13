using AstroAlgo.Basic;
using AstroAlgo.Models;
using System;

namespace AstroAlgo.SolarSystem
{
    /// <summary>
    /// The star at the center of the solar system.
    /// </summary>
    public class Sun : Star
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sun"/>.
        /// </summary>
        public Sun() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sun"/>.
        /// </summary>
        /// <param name="latitude">Latitude of observation site.</param>
        /// <param name="longitude">Longitude of observation site.</param>
        public Sun(double latitude, double longitude) : base(latitude, longitude) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sun"/>.
        /// </summary>
        /// <param name="latitude">Latitude of observation site.</param>
        /// <param name="longitude">Longitude of observation site.</param>
        /// <param name="dateTime">Time of observation site.</param>
        /// <param name="localTimeZone">Time zone of observation site.</param>
        public Sun(double latitude, double longitude, DateTime dateTime, TimeZoneInfo localTimeZone) : base(latitude, longitude, dateTime, localTimeZone) { }

        /// <inheritdoc/>
        public override Ecliptic GetEclipticCoordinate(DateTime time, bool isApparent = false)
        {
            var e = GetEquatorCoordinate(time, isApparent);
            return CoordinateSystem.Equator2Ecliptic(e, time, isApparent);
        }

        /// <inheritdoc/>
        public override Equator GetEquatorCoordinate(DateTime time, bool isApparent = false)
        {
            double T = (Julian.ToJulianDay(time) - 2451545.0) / 36525.0;

            double L0 = 280.46645 + 36000.76983 * T + 0.0003030 * T * T;
            double M = 357.52910 + 35999.05030 * T - 0.0001559 * T * T - 0.00000048 * T * T * T;
            double e = 0.016708617 - 0.000042037 * T - 0.0000001236 * T * T;

            L0 = BasicTools.SimplifyAngle(L0);
            M = BasicTools.SimplifyAngle(M);
            e = BasicTools.SimplifyAngle(e);

            double C = Math.Abs((1.914600 - 0.004817 * T - 0.000014 * T * T)) * Math.Sin(M * (Math.PI / 180.0)) + (0.019993 - 0.000101 * T) * Math.Sin(2 * M * (Math.PI / 180.0)) + 0.000290 * Math.Sin(3 * M * (Math.PI / 180.0));
            C = BasicTools.SimplifyAngle(C);

            double theta = L0 + C;

            double theta2000 = theta - 0.01397 * (time.Year - 2000);

            double omega = 125.04 - 1934.136 * T;
            omega = BasicTools.SimplifyAngle(omega);
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

        /// <inheritdoc/>
        public override double GetToEarthDistance(DateTime time)
        {
            double T = (Julian.ToJulianDay(time) - 2451545.0) / 36525.0;

            double M = 357.52910 + 35999.05030 * T - 0.0001559 * T * T - 0.00000048 * T * T * T;
            double e = 0.016708617 - 0.000042037 * T - 0.0000001236 * T * T;

            M = BasicTools.SimplifyAngle(M);
            e = BasicTools.SimplifyAngle(e);

            double C = Math.Abs((1.914600 - 0.004817 * T - 0.000014 * T * T)) * Math.Sin(M * (Math.PI / 180.0)) + (0.019993 - 0.000101 * T) * Math.Sin(2 * M * (Math.PI / 180.0)) + 0.000290 * Math.Sin(3 * M * (Math.PI / 180.0));
            C = BasicTools.SimplifyAngle(C);

            double v = M + C;
            v = BasicTools.SimplifyAngle(v);

            double distance = 1.0000011018 * (1 - e * e) / (1 + e * Math.Cos(v * (Math.PI / 180.0)));

            return distance;
        }

        /// <summary>
        /// Calculate the equinox and solstice.
        /// </summary>
        /// <param name="year">Year.</param>
        /// <param name="localTimeZone">Local time zone.</param>
        /// <returns><see cref="DateTime"/> array with length of 4 and contents of vernal equinox, summer solstice, autumnal equinox and winter solstice.</returns>
        public DateTime[] GetEquinoxAndSolstice(int year, TimeZoneInfo localTimeZone)
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

                result[i] = TimeZoneInfo.ConvertTime(Julian.ToCalendarDay(JD), TimeZoneInfo.Utc, localTimeZone);
            }

            return result;
        }

        /// <summary>
        /// Calculate 24 solar terms.
        /// </summary>
        /// <param name="year">Year.</param>
        /// <param name="term">Solar term.</param>
        /// <param name="localTimeZone">Local time zone.</param>
        /// <returns>The <see cref="DateTime"/> of solar term.</returns>
        /// <remarks>
        /// If you want to calculate the equinox and solstice, please use the method <see cref="GetEquinoxAndSolstice"/>.
        /// </remarks>
        public DateTime GetSolarTerms(int year, SolarTerm term, TimeZoneInfo localTimeZone)
        {
            // Estimate iteration start time interval
            EstimateSTtimeScope(year, term, out double lJD, out double rJD);

            double solarTermsJD = 0.0;
            double longitude = 0.0;
            DateTime solarTermsTime = new DateTime();

            do
            {
                solarTermsJD = ((rJD - lJD) * 0.618) + lJD;
                solarTermsTime = Julian.ToCalendarDay(solarTermsJD);
                longitude = CoordinateSystem.Equator2Ecliptic(GetEquatorCoordinate(solarTermsTime, true), solarTermsTime, true).Longitude;

                // In the iterative approximation of 0 degree of ecliptic longitude, the estimated value of ecliptic longitude may be in the interval of (345,360] and [0,15) due to the 360 degree circularity of the angle. If the value falls into the previous interval, it needs to be corrected.
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

            // Min +2 correction
            return TimeZoneInfo.ConvertTime(solarTermsTime.AddMinutes(2), TimeZoneInfo.Utc, localTimeZone);
        }

        internal void EstimateSTtimeScope(int year, SolarTerm term, out double lJD, out double rJD)
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
                case SolarTerm.WakingOfInsects:
                    lJD = Julian.ToJulianDay(new DateTime(year, 3, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 3, 9));
                    break;
                case SolarTerm.SpringEquinox:
                    lJD = Julian.ToJulianDay(new DateTime(year, 3, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 3, 24));
                    break;
                case SolarTerm.PureBrightness:
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
                case SolarTerm.GrainInBeard:
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
                case SolarTerm.AutumnEquinox:
                    lJD = Julian.ToJulianDay(new DateTime(year, 9, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 9, 24));
                    break;
                case SolarTerm.ColdDew:
                    lJD = Julian.ToJulianDay(new DateTime(year, 10, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 10, 9));
                    break;
                case SolarTerm.FrostsDescent:
                    lJD = Julian.ToJulianDay(new DateTime(year, 10, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 10, 24));
                    break;
                case SolarTerm.BeginningOfWinter:
                    lJD = Julian.ToJulianDay(new DateTime(year, 11, 4));
                    rJD = Julian.ToJulianDay(new DateTime(year, 11, 9));
                    break;
                case SolarTerm.LesserSnow:
                    lJD = Julian.ToJulianDay(new DateTime(year, 11, 16));
                    rJD = Julian.ToJulianDay(new DateTime(year, 11, 24));
                    break;
                case SolarTerm.GreaterSnow:
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
    }
}