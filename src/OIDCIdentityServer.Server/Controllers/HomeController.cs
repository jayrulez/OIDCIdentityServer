using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OIDCIdentityServer.Server.Models;
using System.Diagnostics;

namespace OIDCIdentityServer.Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}