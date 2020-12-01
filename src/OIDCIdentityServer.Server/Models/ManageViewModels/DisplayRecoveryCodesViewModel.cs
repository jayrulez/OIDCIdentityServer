using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OIDCIdentityServer.Server.Models.ManageViewModels
{
    public class DisplayRecoveryCodesViewModel
    {
        [Required]
        public IEnumerable<string> Codes { get; set; }

    }
}
