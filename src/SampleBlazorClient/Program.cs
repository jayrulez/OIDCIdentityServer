using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SampleBlazorClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("SampleApi1")
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://localhost:9001"))
                .AddHttpMessageHandler(sp =>
                {
                    var handler = sp.GetRequiredService<AuthorizationMessageHandler>()
                    .ConfigureHandler(
                        authorizedUrls: new[] { "https://localhost:9001" },
                        scopes: new[] { "api1" });

                    return handler;
                });


            builder.Services.AddHttpClient("SampleApi2")
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://localhost:9002"))
                .AddHttpMessageHandler(sp =>
                {
                    var handler = sp.GetRequiredService<AuthorizationMessageHandler>()
                    .ConfigureHandler(
                        authorizedUrls: new[] { "https://localhost:9002" },
                        scopes: new[] { "api2" });

                    return handler;
                });

            //builder.Services.AddScoped(sp =>
            //{
            //    var factory = sp.GetRequiredService<IHttpClientFactory>()
            //    return factory.CreateClient("SampleApi1");
            //});

            //builder.Services.AddScoped(sp =>
            //{
            //    var factory = sp.GetRequiredService<IHttpClientFactory>();
            //    return factory.CreateClient("SampleApi2");
            //});

            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.ClientId = "blazor_client";
                options.ProviderOptions.Authority = "https://localhost:5001/";
                options.ProviderOptions.ResponseType = "code";
                options.ProviderOptions.ResponseMode = "fragment";
                options.ProviderOptions.DefaultScopes.Add("api1");
                options.ProviderOptions.DefaultScopes.Add("api2");
                options.AuthenticationPaths.RemoteRegisterPath = "https://localhost:5001/Account/Register";
            });

            await builder.Build().RunAsync();
        }
    }
}
