using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using System.Security.Claims;

namespace SampleApi2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResourceController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet]
        [HttpGet("private")]
        public IActionResult Private()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return BadRequest();
            }

            return Content($"You have authorized access to resources belonging to {identity.Name} on Zirku.Api2.");
        }

        [HttpGet]
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Content("This is a public endpoint that is at Zirku.Api2; it does not require authorization.");
        }
    }
}
