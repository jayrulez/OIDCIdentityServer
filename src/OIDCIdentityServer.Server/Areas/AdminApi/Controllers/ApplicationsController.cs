using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OIDCIdentityServer.Server.Data;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OIDCIdentityServer.Server.Areas.AdminApi.Controllers
{
    //[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = "OIDCAdmin")]
    [Route("admin_api/applications")]
    public class ApplicationsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOpenIddictApplicationManager _applicationManager;

        public ApplicationsController(UserManager<ApplicationUser> userManager, IOpenIddictApplicationManager applicationManager)
        {
            _userManager = userManager;
            _applicationManager = applicationManager;
        }

        public IActionResult GetApplications(int? page = 1)
        {
            int pageSize = 20;
            int pageNumber = page != null && page > 0 ? page.Value : 1;

            var applications = _applicationManager.ListAsync(pageSize, (pageNumber -1) * pageSize);

            return Ok(applications);
        }

        void GetApplication()
        {

        }

        void CreateApplication()
        {

        }

        void UpdateApplication()
        {

        }

        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("message")]
        public async Task<IActionResult> GetMessage()
        {
            // This demo action requires that the client application be granted the "demo_api" scope.
            // If it was not granted, a detailed error is returned to the client application to inform it
            // that the authorization process must be restarted with the specified scope to access this API.
            if (!User.HasScope("oidc_admin_server_api"))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictValidationAspNetCoreConstants.Properties.Scope] = "test_api",
                        [OpenIddictValidationAspNetCoreConstants.Properties.Error] = Errors.InsufficientScope,
                        [OpenIddictValidationAspNetCoreConstants.Properties.ErrorDescription] =
                            "The 'demo_api' scope is required to perform this action."
                    }));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Challenge(
                    authenticationSchemes: OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictValidationAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                        [OpenIddictValidationAspNetCoreConstants.Properties.ErrorDescription] =
                            "The specified access token is bound to an account that no longer exists."
                    }));
            }

            return Content($"{user.UserName} has been successfully authenticated.");
        }
    }
}
