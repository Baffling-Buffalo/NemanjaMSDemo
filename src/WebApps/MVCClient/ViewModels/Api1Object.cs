using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.ViewModels
{
    public class Api1Object
    {
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Data")]
        [Required(ErrorMessage = "{0} is required")]
        public string Data { get; set; }
    }
}
