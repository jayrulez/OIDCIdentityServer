using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OIDCIdentityServer.Server.Models;
using System.Diagnostics;

namespace OIDCIdentityServer.Server.Controllers
{
    public class ErrorController : Controller
    {

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet, HttpPost, Route("~/error")]
        public IActionResult Error()
        {
            // If the error was not caused by an invalid
            // OIDC request, display a generic error page.
            var response = HttpContext.GetOpenIddictServerResponse();
            if (response is null)
            {
                return View(new ErrorViewModel()
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }

            return View(new ErrorViewModel()
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Error = response.Error,
                ErrorDescription = response.ErrorDescription
            });
        }
    }
}
