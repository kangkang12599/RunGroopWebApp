 using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RunGroopWebApp.Helpers;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace RunGroopWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IClubRepository _clubRepository;

        public HomeController(ILogger<HomeController> logger, IClubRepository clubRepository)
        {
            _logger = logger;
            _clubRepository = clubRepository;
        }

        public async Task<IActionResult> Index()
        {
            var ipInfo = new IPInfo();
            var homeVM = new HomeVM();
            homeVM.Clubs = new List<Club>();
            try
            {
                string url = "https://ipinfo.io?token=88b64b31c9b6e3";
                var info = new WebClient().DownloadString(url);
                ipInfo = JsonConvert.DeserializeObject<IPInfo>(info);
                RegionInfo myRegion = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRegion.EnglishName;
                homeVM.City = ipInfo.City;
                homeVM.State = ipInfo.Region;
                if (homeVM.City != null)
                {
                    homeVM.Clubs = (List<Club>) await _clubRepository.GetClubByCity(homeVM.City);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Fail to load the club, please try again later.");
            }
            return View(homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}