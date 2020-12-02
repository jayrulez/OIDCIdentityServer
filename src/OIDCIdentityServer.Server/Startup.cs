using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OIDCIdentityServer.Server.Data;
using OIDCIdentityServer.Server.Services;
using System;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OIDCIdentityServer.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.

            services.AddOptions();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));

                // Register the entity sets needed by OpenIddict.
                // Note: use the generic overload if you need
                // to replace the default OpenIddict entities.
                options.UseOpenIddict();
            });
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDataProtection()
                .PersistKeysToDbContext<ApplicationDbContext>();

            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            // Configure Identity to use the same JWT claims as OpenIddict instead
            // of the legacy WS-Federation claims it uses by default (ClaimTypes),
            // which saves you from doing the mapping in your authorization controller.
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = Claims.Role;
            });

            services.AddOpenIddict()

                // Register the OpenIddict core components.
                .AddCore(options =>
                {
                    // Configure OpenIddict to use the Entity Framework Core stores and models.
                    // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
                    options.UseEntityFrameworkCore()
                           .UseDbContext<ApplicationDbContext>();

                    // Developers who prefer using MongoDB can remove the previous lines
                    // and configure OpenIddict to use the specified MongoDB database:
                    // options.UseMongoDb()
                    //        .UseDatabase(new MongoClient().GetDatabase("openiddict"));
                })// Register the OpenIddict server components.
                .AddServer(options =>
                {
                    // Enable the authorization, device, logout, token, userinfo and verification endpoints.
                    options.SetAuthorizationEndpointUris("/connect/authorize")
                           .SetDeviceEndpointUris("/connect/device")
                           .SetLogoutEndpointUris("/connect/logout")
                           .SetTokenEndpointUris("/connect/token")
                           .SetIntrospectionEndpointUris("/connect/introspect")
                           .SetUserinfoEndpointUris("/connect/userinfo")
                           .SetVerificationEndpointUris("/connect/verify");

                    // Note: this sample uses the code, device code, password and refresh token flows, but you
                    // can enable the other flows if you need to support implicit or client credentials.
                    options.AllowAuthorizationCodeFlow()
                           .AllowDeviceCodeFlow()
                           .AllowPasswordFlow()
                           .AllowRefreshTokenFlow()
                           .AllowImplicitFlow();

                    // Mark the supported scopes.
                    options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, "oidc_admin_server_api", "test_api", "api1", "api2");

                    // Register the signing and encryption credentials.
                    options.AddDevelopmentEncryptionCertificate() // todo: check if this can be used along with symmetric encryption
                           .AddDevelopmentSigningCertificate();

                    // Register the encryption credentials. This sample uses a symmetric
                    // encryption key that is shared between the server and the Api2 sample
                    // (that performs local token validation instead of using introspection).
                    //
                    // Note: in a real world application, this encryption key should be
                    // stored in a safe place (e.g in Azure KeyVault, stored as a secret).
                    options.AddEncryptionKey(new SymmetricSecurityKey(
                        Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

                    // Force client applications to use Proof Key for Code Exchange (PKCE).
                    options.RequireProofKeyForCodeExchange();

                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    options.UseAspNetCore()
                           .EnableStatusCodePagesIntegration()
                           .EnableAuthorizationEndpointPassthrough()
                           .EnableLogoutEndpointPassthrough()
                           .EnableTokenEndpointPassthrough()
                           .EnableUserinfoEndpointPassthrough()
                           .EnableVerificationEndpointPassthrough()
                           .DisableTransportSecurityRequirement(); // During development, you can disable the HTTPS requirement.

                    // Note: if you don't want to specify a client_id when sending
                    // a token or revocation request, uncomment the following line:
                    //
                    // options.AcceptAnonymousClients();

                    // Note: if you want to process authorization and token requests
                    // that specify non-registered scopes, uncomment the following line:
                    //
                    // options.DisableScopeValidation();

                    // Note: if you don't want to use permissions, you can disable
                    // permission enforcement by uncommenting the following lines:
                    //
                    // options.IgnoreEndpointPermissions()
                    //        .IgnoreGrantTypePermissions()
                    //        .IgnoreResponseTypePermissions()
                    //        .IgnoreScopePermissions();

                    // Note: when issuing access tokens used by third-party APIs
                    // you don't own, you can disable access token encryption:
                    //
                    // options.DisableAccessTokenEncryption();
                })

                // Register the OpenIddict validation components.
                .AddValidation(options =>
                {
                    // Configure the audience accepted by this resource server.
                    // The value MUST match the audience associated with the
                    // "demo_api" scope, which is used by ResourceController.
                    options.AddAudiences("oidc_admin_server");
                    options.AddAudiences("test_server");
                    options.AddAudiences("sample_api1");
                    options.AddAudiences("sample_api2");

                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();

                    // For applications that need immediate access token or authorization
                    // revocation, the database entry of the received tokens and their
                    // associated authorizations can be validated for each API call.
                    // Enabling these options may have a negative impact on performance.
                    //
                    // options.EnableAuthorizationEntryValidation();
                    // options.EnableTokenEntryValidation();
                });

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies(o => { });

            services.AddCors();
            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<Startup>();
            logger.LogInformation($"Environment: {env.EnvironmentName}");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseCors(builder =>
            //{
            //    builder.WithOrigins("https://localhost:44398");
            //    builder.WithMethods("GET");
            //    builder.WithHeaders("Authorization");
            //});

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();

                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapRazorPages();
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dataInitializer = new DataInitializer(scope.ServiceProvider);

                dataInitializer.Run().Wait();
            }
        }
    }
}
