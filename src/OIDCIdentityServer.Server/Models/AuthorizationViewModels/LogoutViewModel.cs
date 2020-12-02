using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OIDCIdentityServer.Server.Models.AuthorizationViewModels
{
    public class LogoutViewModel
    {
        [BindNever]
        public string RequestId { get; set; }
    }
}
