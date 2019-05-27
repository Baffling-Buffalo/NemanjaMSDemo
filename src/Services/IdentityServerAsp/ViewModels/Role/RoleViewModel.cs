using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerAsp.Models.RoleViewModels
{
    public class RoleViewModel
    {
        [Required]
        [StringLength(30,ErrorMessage = "min 3 max 30 letters",MinimumLength = 3)]
        public string Name { get; set; }
    }
}
