using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Nominal : BaseModel
    {
        [Key]
        public int NominalId { get; set; }
        public string Code { get; set; }
        [Required]
        public int MainNominalId { get; set; }
        [Required]
        public string GLType { get; set; }
        [Required]
        public string BalanceType { get; set; }
        [NotMapped]
        public decimal Balance { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public bool AllowJournal { get; set; }
        [Required]
        public string Status { get; set; }

        public MainNominal MainNominal { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
