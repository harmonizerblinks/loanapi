using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Transaction : BaseModel
    {
        [Key]
        public int TransactionId { get; set; }
        public string Code { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Source { get; set; }
        [Required]
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }
        [NotMapped]
        [Column(TypeName = "Money")]
        public decimal Balance { get; set; }
        [Required]
        public string Method { get; set; }
        public int? AccountId { get; set; }
        [Required]
        public int NominalId { get; set; }
        public int? TellerId { get; set; }
        [Required]
        public string Reference { get; set; }

        public Nominal Nominal { get; set; }
        public Teller Teller { get; set; }
        public Account Account { get; set; }
    }
}
