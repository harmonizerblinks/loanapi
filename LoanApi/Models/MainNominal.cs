using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class MainNominal
    {
        [Key]
        public int MainNominalId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Status { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
    }
}
