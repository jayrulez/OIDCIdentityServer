using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SampleMvcClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SampleMvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet("~/")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize, HttpPost("~/")]
        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            var token = await HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectParameterNames.AccessToken);
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("The access token cannot be found in the authentication ticket. " +
                                                    "Make sure that SaveTokens is set to true in the OIDC options.");
            }

            using var client = _httpClientFactory.CreateClient();

            using var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5001/test_api/message");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return View("Index", model: await response.Content.ReadAsStringAsync());
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
