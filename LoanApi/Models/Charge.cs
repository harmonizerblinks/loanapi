using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Charge
    {
        [Key]
        public int ChargeId { get; set; }
        [Required]
        public int CotId { get; set; }
        [Required]
        public string Method { get; set; }
        [Required]
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }
        [Required]
        public int AccountId { get; set; }
        public string Reference { get; set; }
        [Required]
        public string UserId { get; set; }
        public DateTime Date { get; set; }

        public Cot Cot { get; set; }
        public Account Account { get; set; }

    }
}
