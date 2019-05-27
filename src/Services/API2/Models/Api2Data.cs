using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API2.Models
{
    [Table("Api2_Data")]
    public partial class Api2Data
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Data { get; set; }
    }
}