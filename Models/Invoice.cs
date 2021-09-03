using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Invoice
    {
        [Required]
        public long id { get; set; }
        [Required]
        public string nit { get; set; }
        public string description{ get; set; }
        [Required]
        public decimal totalValue { get; set; }
        [Required]
        public int percentageIVA { get; set; }
    }
}
