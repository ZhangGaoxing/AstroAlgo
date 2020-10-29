using System;

namespace AstroAlgo.Basic
{
    /// <summary>
    /// Basic tools for calculation.
    /// </summary>
    public class BasicTools
    {
        /// <summary>
        /// Used to simplify the angle.
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>
        /// <returns>The simplified angle.</returns>
        /// <remarks>Because it involves accuracy problems, the modulo operation is not used.</remarks>
        public static double SimplifyAngle(double angle)
        {
            decimal a = Convert.ToDecimal(angle);

            if (a <= 0)
            {
                while (!(a >= 0 && a <= 360))
                {
                    a += 360;
                }
            }
            else
            {
                while (!(a >= 0 && a <= 360))
                {
                    a -= 360;
                }
            }

            return Convert.ToDouble(a);
        }

        /// <summary>
        /// Angle converted to degrees-minutes-seconds.
        /// </summary>
        /// <param name="angle">The angel.</param>
        /// <returns>The degrees-minutes-seconds.</returns>
        public static (int degrees, int minutes, double seconds) Angle2DMS(double angle)
        {
            decimal a = Convert.ToDecimal(angle);

            int degrees = (int)a;
            int minutes = (int)((a - degrees) * 60);
            decimal seconds = (((a - degrees) * 60) - minutes) * 60;

            return (degrees, minutes, Convert.ToDouble(seconds));
        }

        /// <summary>
        /// Angle converted to hours-minutes-seconds.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>The hours-minutes-seconds.</returns>
        public static (int hours, int minutes, double seconds) Angle2HMS(double angle)
        {
            var time = TimeSpan.FromHours(angle / 15.0);

            int hours = time.Hours;
            int minutes = time.Minutes;
            double seconds = time.TotalSeconds - hours * 3600 - minutes * 60;

            return (hours, minutes, seconds);
        }
    }
}
