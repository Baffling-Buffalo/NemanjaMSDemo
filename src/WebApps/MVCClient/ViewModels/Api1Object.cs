using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.ViewModels
{
    public class Api1Object
    {
        public int Id { get; set; }
        [Required]
        public string Data { get; set; }
    }
}
