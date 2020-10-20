using AstroAlgo.Basic;
using System;

namespace AstroConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new DateTime(2008, 12, 31).DayOfYear);
            Console.WriteLine(Julian.GetDayOfYear(new DateTime(2008, 12, 31)));
        }
    }
}
