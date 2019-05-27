using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API1.Models
{
    [Table("Api1_Data")]
    public partial class Api1Data
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Data { get; set; }
    }
}