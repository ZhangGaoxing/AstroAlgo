using System;
using System.Collections.Generic;
using System.Text;

namespace AstroAlgo.Models
{
    /// <summary>
    /// 二十四节气
    /// </summary>
    public enum SolarTerm
    {
        /// <summary>
        /// 立春：太阳黄经为315度。
        /// </summary>
        BeginningOfSpring = 315,

        /// <summary>
        /// 雨水：太阳黄经为330°
        /// </summary>
        RainWater = 330,

        /// <summary>
        /// 惊蛰：太阳黄经为345°
        /// </summary>
        InsectsAwakening = 345,

        /// <summary>
        /// 春分：太阳黄经为0°。
        /// </summary>
        SpringEquinox = 0,

        /// <summary>
        /// 清明：太阳黄经为15°。
        /// </summary>
        FreshGreen = 15,

        /// <summary>
        /// 谷雨：太阳黄经为30°。
        /// </summary>
        GrainRain = 30,

        /// <summary>
        /// 立夏：斗指东南。太阳黄经为45°。
        /// </summary>
        BeginningOfSummer = 45,

        /// <summary>
        /// 小满：太阳黄经为60°。
        /// </summary>
        LesserFullness = 60,

        /// <summary>
        /// 芒种：太阳黄经为75°。
        /// </summary>
        GrainInEar = 75,

        /// <summary>
        /// 夏至：太阳黄经为90°。
        /// </summary>
        SummerSolstice = 90,

        /// <summary>
        /// 小暑：太阳黄经为105°。
        /// </summary>
        LesserHeat = 105,

        /// <summary>
        /// 大暑：太阳黄经为120°。
        /// </summary>
        GreaterHeat = 120,

        /// <summary>
        /// 立秋：太阳黄经为135°。
        /// </summary>
        BeginningOfAutumn = 135,

        /// <summary>
        /// 处暑：太阳黄经为150°。
        /// </summary>
        EndOfHeat = 150,

        /// <summary>
        /// 白露：太阳黄经为165°。
        /// </summary>
        WhiteDew = 165,

        /// <summary>
        /// 秋分：太阳黄经为180°
        /// </summary>
        AutumnalEquinox = 180,

        /// <summary>
        /// 寒露：太阳黄经为195°。
        /// </summary>
        ColdDew = 195,

        /// <summary>
        /// 霜降：太阳黄经为210°。
        /// </summary>
        FirstFrost = 210,

        /// <summary>
        /// 立冬：太阳黄经为225°。
        /// </summary>
        BeginningOfWinter = 225,

        /// <summary>
        /// 小雪：太阳黄经为240°。
        /// </summary>
        LightSnow = 240,

        /// <summary>
        /// 大雪：太阳黄经为255°。
        /// </summary>
        HeavySnow = 255,

        /// <summary>
        /// 冬至：太阳黄经为270°。
        /// </summary>
        WinterSolstice = 270,

        /// <summary>
        /// 小寒：太阳黄经为285°。
        /// </summary>
        LesserCold = 285,

        /// <summary>
        /// 大寒：太阳黄经为300°。
        /// </summary>
        GreaterCold = 300
    }
}
