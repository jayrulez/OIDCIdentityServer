using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using System;
using System.Threading.Tasks;

namespace OIDCIdentityServer.Server
{
    public class DataInitializer
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly ILogger _logger;

        public DataInitializer(IServiceProvider serviceProvider)
        {
            _applicationManager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            _scopeManager = serviceProvider.GetRequiredService<IOpenIddictScopeManager>();
            _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<DataInitializer>();
        }

        private async Task RegisterApplications()
        {
            foreach (var descriptor in SeedData.GetApplications())
            {
                if (await _applicationManager.FindByClientIdAsync(descriptor.ClientId) is null)
                {
                    await _applicationManager.CreateAsync(descriptor);
                    _logger.LogInformation($"Created client application: '{descriptor.ClientId}'.");
                }
            }
        }

        private async Task RegisterScopes()
        {
            foreach (var descriptor in SeedData.GetScopes())
            {
                if (await _scopeManager.FindByNameAsync(descriptor.Name) is null)
                {
                    await _scopeManager.CreateAsync(descriptor);
                    _logger.LogInformation($"Created scope: '{descriptor.Name}'.");
                }
            }
        }

        public async Task Run()
        {
            await RegisterApplications();
            await RegisterScopes();
        }
    }
}
