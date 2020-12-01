using Microsoft.AspNetCore.Mvc;

namespace OIDCIdentityServer.Server.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}