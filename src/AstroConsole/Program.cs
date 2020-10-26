using AstroAlgo.Basic;
using AstroAlgo.SolarSystem;
using System;

namespace AstroConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Sun s = new Sun(34, 115, TimeZoneInfo.Local);
            Console.WriteLine(s.Rise);
        }
    }
}
