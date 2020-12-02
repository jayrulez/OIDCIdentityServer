using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using System.Security.Claims;

namespace SampleApi1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResourceController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("private")]
        public IActionResult Private()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return BadRequest();
            }

            return Content($"You have authorized access to resources belonging to {identity.Name} on Zirku.Api1.");
        }

        [HttpGet("public")]
        public IActionResult Public()
        {
            return Content("This is a public endpoint that is at Zirku.Api1; it does not require authorization.");
        }
    }
}
