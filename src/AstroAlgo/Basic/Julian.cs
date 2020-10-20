using System;

namespace AstroAlgo.Basic
{
    /// <summary>
    /// Julian day.
    /// </summary>
    public class Julian
    {
        /// <summary>
        /// Convert <see cref="DateTime"/> to Julian day.
        /// </summary>
        /// <param name="time">Time to convert.</param>
        /// <param name="isModified">Is it a modified Julian day.</param>
        /// <returns>Julian day.</returns>
        public static double ToJulianDay(DateTime time, bool isModified = false)
        {
            int Y, M, A, B;
            double D, JD;

            if (time.Month == 1 || time.Month == 2)
            {
                Y = time.Year - 1;
                M = time.Month + 12;
                D = ((time.Second / 60 + time.Minute) / 60.0 + time.Hour) / 24.0 + time.Day;
            }
            else
            {
                Y = time.Year;
                M = time.Month;
                D = ((time.Second / 60 + time.Minute) / 60.0 + time.Hour) / 24.0 + time.Day;
            }

            A = (int)(Y / 100.0);

            if ((Y <= 1582 && M <= 10 && D < 15) || Y < 1582)
            {
                B = 0;
            }
            else
            {
                B = 2 - A + (int)(A / 4.0);
            }

            JD = (int)(365.25 * (Y + 4716)) + (int)(30.6 * (M + 1)) + D + B - 1524.5;

            if (isModified)
            {
                return JD - 2400000.5;
            }
            else
            {
                return JD;
            }           
        }

        /// <summary>
        /// Convert Julian day to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="julianDay">Julian day to convert.</param>
        /// <returns><see cref="DateTime"/></returns>
        public static DateTime ToCalendarDay(double julianDay)
        {
            double JD = julianDay + 0.5;
            int Z = (int)JD;
            double F = JD - Z;
            int A, B, C, D, E;

            if (Z < 2299161)
            {
                A = Z;
            }
            else
            {
                int a = (int)((Z - 1867216.25) / 36524.25);
                A = Z + 1 + a - (int)(a / 4.0);
            }

            B = A + 1524;
            C = (int)((B - 122.1) / 365.25);
            D = (int)(365.25 * C);
            E = (int)((B - D) / 30.6001);

            double day = B - D - (int)(30.6001 * E) + F;
            int dayL = (int)day;
            double dayR = day - dayL;

            double hour = dayR * 24;
            int hourL = (int)hour;
            double hourR = hour - hourL;

            double min = hourR * 60;
            int minL = (int)min;
            double minR = min - minL;

            int sec = (int)(minR * 60);

            int month, year;

            if (E == 14 || E == 15)
            {
                month = E - 13;
            }
            else
            {
                month = E - 1;
            }

            if (month == 1 || month == 2)
            {
                year = C - 4715;
            }
            else
            {
                year = C - 4716;
            }

            return new DateTime(year, month, dayL, hourL, minL, sec);
        }

        /// <summary>
        /// Calculate the number of days between the given date.
        /// </summary>
        /// <param name="date1">Date 1.</param>
        /// <param name="date2">Date 2.</param>
        /// <returns>Number of days between.</returns>
        public static double Date2Interval(DateTime date1, DateTime date2)
        {
            double jd1 = ToJulianDay(date1);
            double jd2 = ToJulianDay(date2);

            return Math.Abs(jd1 - jd2);
        }

        /// <summary>
        /// Calculate the date for the given interval of days.
        /// </summary>
        /// <param name="date">Start date.</param>
        /// <param name="interval">Number of days between.</param>
        /// <returns>End date.</returns>
        public static DateTime Interval2Date(DateTime date, double interval)
        {
            double jd1 = ToJulianDay(date);
            double jd2;

            jd2 = jd1 + interval;

            return ToCalendarDay(jd2);
        }

        /// <summary>
        /// Calculate the day of the week for the given date.
        /// </summary>
        /// <param name="date">The given date.</param>
        /// <returns>The day of the week.</returns>
        public static DayOfWeek GetDayOfWeek(DateTime date)
        {
            double jd = ToJulianDay(date, false) + 1.5;

            return (DayOfWeek)(int)(jd % 7);
        }

        /// <summary>
        /// Calculate the day of the year (ordinal day) for the given date.
        /// </summary>
        /// <param name="date">The given date.</param>
        /// <returns>The day of the year.</returns>
        public static int GetDayOfYear(DateTime date)
        {
            int year = date.Year, month = date.Month, day = date.Day;

            int K, num;

            if (year % 4 == 0 && year % 100 != 0)
            {
                K = 1;
            }
            else if (year % 400 == 0)
            {
                K = 1;
            }
            else
            {
                K = 2;
            }

            num = (int)(275 * month / 9.0) - K * (int)((month + 9) / 12.0) + day - 30;

            return num;
        }
    }
}