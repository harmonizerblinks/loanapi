using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class AccountType : BaseModel
    {
        [Key]
        public int AccountTypeId { get; set; }
        public string Code { get; set; }
        [Required]
        public string BaseType { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int SequenceId { get; set; }
        public int NominalId { get; set; }
        [Required]
        public int CotId { get; set; }
        [Required]
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }
        [Required]
        public string Frequency { get; set; }
        [Required]
        [Column(TypeName = "Money")]
        public decimal LoanAmount { get; set; }
        [Required]
        public int Days { get; set; }
        [Required]
        public int Dormant { get; set; }
        public string Status { get; set; }

        public Cot Cot { get; set; }
        public Sequence Sequence { get; set; }
        public Nominal Nominal { get; set; }
        public List<Account> Accounts { get; set; }
    }
}
