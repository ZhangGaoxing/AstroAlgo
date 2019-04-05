using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AstroAlgo.SolarSystem;
using AstroAlgo.Base;
using AstroAlgo.Model;

namespace AstroAlgoTest
{
    class Program
    {
        static void Main(string[] args)
        { 
            var array = Enum.GetValues(typeof(SolarTerm)).OfType<SolarTerm>()
                        .OrderBy(x => x);

            foreach (var item in array)
            {
                Console.WriteLine($"{item.ToString()} : {Sun.SolarTerms(2019, item, TimeZoneInfo.Local)}");
            }
        }
    }
}
