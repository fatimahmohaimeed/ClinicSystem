using ClinicSystemTest.Data;
using ClinicSystemTest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace ClinicSystemTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ILogger<HomeController> _logger;//

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager)//
        {
            _logger = logger;
            this.userManager = userManager;
        }


        public IActionResult Index()
        {

            return View();
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


        #region Valdiation error page
        public IActionResult ValdiationErrorr()
        {
            return View();
        }

        #endregion
    }
}