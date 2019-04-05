using System;

namespace AstroAlgo.Base
{
    /// <summary>
    /// 基础工具
    /// </summary>
    public class BasicTools
    {
        internal static double AngleSimplification(double angle)
        {
            double a = angle;

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

            return a;
        }

        /// <summary>
        /// 角度转度分秒
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>度分秒</returns>
        public static string Angle2DMS(double angle)
        {
            int degree = (int)angle;
            int minute = (int)((angle - degree) * 60);
            double second = (((angle - degree) * 60) - minute) * 60;

            return degree + "° " + minute + "′ " + second.ToString("0.00") + "″ ";
        }

        /// <summary>
        /// 角度转时分秒
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>时分秒</returns>
        public static string Angle2HMS(double angle)
        {
            var time = TimeSpan.FromHours(angle / 15.0);

            int hour = time.Hours;
            int minute = time.Minutes;
            double second = time.TotalSeconds - hour * 3600 - minute * 60;

            return hour + "h " + minute + "m " + second.ToString("0.00") + "s";
        }
    }
}
