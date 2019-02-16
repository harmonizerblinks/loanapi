using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Alert
    {
        [Key]
        public int AlertId { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public bool Name { get; set; }
        [Required]
        public bool Auto { get; set; }
    }
}
