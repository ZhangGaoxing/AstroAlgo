using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AstroAlgo.SolarSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarSysInfo.Models;

namespace SolarSysInfo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(double latitude = 39.56, double longitude = 116.23, DateTime time = default, string zone = default)
        {
            if (time == default)
            {
                time = DateTime.Now;
            }

            TimeZoneInfo timeZone;
            if (string.IsNullOrEmpty(zone))
            {
                timeZone = TimeZoneInfo.Local;
            }
            else
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(zone);
            }
            
            Sun sun = new Sun(latitude, longitude, time, timeZone);
            Mercury mercury = new Mercury(latitude, longitude, time, timeZone);
            Venus venus = new Venus(latitude, longitude, time, timeZone);
            Mars mars = new Mars(latitude, longitude, time, timeZone);
            Jupiter jupiter = new Jupiter(latitude, longitude, time, timeZone);
            Saturn saturn = new Saturn(latitude, longitude, time, timeZone);
            Uranus uranus = new Uranus(latitude, longitude, time, timeZone);
            Neptune neptune = new Neptune(latitude, longitude, time, timeZone);

            double[] ra = new[] { sun.Equator.RA, mercury.Equator.RA, venus.Equator.RA, mars.Equator.RA,
                jupiter.Equator.RA, saturn.Equator.RA, uranus.Equator.RA, neptune.Equator.RA};
            double[] dec = new[] { sun.Equator.Dec, mercury.Equator.Dec , venus.Equator.Dec , mars.Equator.Dec,
                jupiter.Equator.Dec, saturn.Equator.Dec, uranus.Equator.Dec, neptune.Equator.Dec };
            TimeSpan[] rising = new[] { sun.Rising, mercury.Rising, venus.Rising, mars.Rising,
                jupiter.Rising, saturn.Rising, uranus.Rising, neptune.Rising };
            TimeSpan[] culmination = new[] { sun.Culmination, mercury.Culmination, venus.Culmination, mars.Culmination,
                jupiter.Culmination, saturn.Culmination, uranus.Culmination, neptune.Culmination };
            TimeSpan[] setting = new[] { sun.Setting, mercury.Setting, venus.Setting, mars.Setting,
                jupiter.Setting, saturn.Setting, uranus.Setting, neptune.Setting };
            double[] toEarth = new[] { sun.ToEarth, mercury.ToEarth, venus.ToEarth, mars.ToEarth,
                jupiter.ToEarth, saturn.ToEarth, uranus.ToEarth, neptune.ToEarth };
            double[] toSun = new[] { 0, mercury.ToSun, venus.ToSun, mars.ToSun,
                jupiter.ToSun, saturn.ToSun, uranus.ToSun, neptune.ToSun };
            double[] ea = new[] { sun.ElevationAngle, mercury.ElevationAngle, venus.ElevationAngle, mars.ElevationAngle,
                jupiter.ElevationAngle, saturn.ElevationAngle, uranus.ElevationAngle, neptune.ElevationAngle };
            double[] az = new[] { sun.Azimuth, mercury.Azimuth, venus.Azimuth, mars.Azimuth,
                jupiter.Azimuth, saturn.Azimuth, uranus.Azimuth, neptune.Azimuth };

            ViewData["Latitude"] = latitude;
            ViewData["Longitude"] = longitude;
            ViewData["Time"] = time;
            ViewData["Zone"] = timeZone.Id;

            ViewData["RA"] = ra;
            ViewData["Dec"] = dec;
            ViewData["Rising"] = rising;
            ViewData["Culmination"] = culmination;
            ViewData["Setting"] = setting;
            ViewData["ToEarth"] = toEarth;
            ViewData["ToSun"] = toSun;
            ViewData["EA"] = ea;
            ViewData["Az"] = az;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
