using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OIDCIdentityServer.Server
{
    public static class SeedData
    {
        public static List<OpenIddictApplicationDescriptor> GetApplications()
        {
            var applications = new List<OpenIddictApplicationDescriptor>();

            applications.Add(new OpenIddictApplicationDescriptor
            {
                ClientId = "oidc_admin_client",
                ClientSecret = "secret_secret_secret",
                ConsentType = ConsentTypes.Explicit,
                DisplayName = "OIDC admin client application",
                PostLogoutRedirectUris = { new Uri("https://localhost:8001/signout-callback-oidc") },
                RedirectUris = { new Uri("https://localhost:8001/signin-oidc") },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Logout,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "oidc_admin_server_api"
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            });

            applications.Add(new OpenIddictApplicationDescriptor
            {
                ClientId = "sample_mvc_client",
                ClientSecret = "secret_secret_secret",
                ConsentType = ConsentTypes.Explicit,
                DisplayName = "Sample MVC client application",
                PostLogoutRedirectUris = { new Uri("https://localhost:6001/signout-callback-oidc") },
                RedirectUris = { new Uri("https://localhost:6001/signin-oidc") },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Logout,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "test_api"
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            });

            // To test this sample with Postman, use the following settings:
            //
            // * Authorization URL: https://localhost:44395/connect/authorize
            // * Access token URL: https://localhost:44395/connect/token
            // * Client ID: postman
            // * Client secret: [blank] (not used with public clients)
            // * Scope: openid email profile roles
            // * Grant type: authorization code
            // * Request access token locally: yes
            applications.Add(new OpenIddictApplicationDescriptor
            {
                ClientId = "postman",
                ConsentType = ConsentTypes.Systematic,
                DisplayName = "Postman",
                RedirectUris =
                {
                    new Uri("urn:postman")
                },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Device,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.DeviceCode,
                    Permissions.GrantTypes.Password,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles
                }
            });

            applications.Add(new OpenIddictApplicationDescriptor
            {
                ClientId = "sample_api1",
                ClientSecret = "secret_secret_secret",
                Permissions =
                {
                    Permissions.Endpoints.Introspection
                }
            });

            applications.Add(new OpenIddictApplicationDescriptor
            {
                ClientId = "blazor_client",
                ConsentType = ConsentTypes.Explicit,
                DisplayName = "Blazor client application",
                Type = ClientTypes.Public,
                PostLogoutRedirectUris =
                {
                    new Uri("https://localhost:9101/authentication/logout-callback")
                },
                RedirectUris =
                {
                    new Uri("https://localhost:9101/authentication/login-callback")
                },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Logout,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "api1",
                    Permissions.Prefixes.Scope + "api2",
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            });

            return applications;
        }

        public static List<OpenIddictScopeDescriptor> GetScopes()
        {
            var scopes = new List<OpenIddictScopeDescriptor>();


            scopes.Add(new OpenIddictScopeDescriptor
            {
                DisplayName = "OIDC Server Administration",
                Name = "oidc_admin_server_api",
                Resources =
                {
                    "oidc_admin_server"
                }
            });

            scopes.Add(new OpenIddictScopeDescriptor
            {
                DisplayName = "Test API access",
                Name = "test_api",
                Resources =
                {
                    "test_server"
                }
            });

            scopes.Add(new OpenIddictScopeDescriptor
            {
                Name = "api1",
                Resources =
                {
                    "sample_api1"
                }
            });

            scopes.Add(new OpenIddictScopeDescriptor
            {
                Name = "api2",
                Resources =
                {
                    "sample_api2"
                }
            });

            return scopes;
        }
    }
}
