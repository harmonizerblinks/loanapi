using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Account : BaseModel
    {
        [Key]
        public int AccountId { get; set; }
        public string Number { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public string BaseType { get; set; }
        [Required]
        public int AccountTypeId { get; set; }
        [Required]
        public int Days { get; set; }
        [Required]
        [Column(TypeName = "Money")]
        public decimal Balance { get; set; }
        [Required]
        public string Purpose { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public bool Alert { get; set; }
        [Required]
        public string Status { get; set; }


        public Employee Employee { get; set; }
        public Customer Customer { get; set; }
        public List<Cheque> Cheque { get; set; }
        public List<Charge> Charges { get; set; }
        public AccountType AccountType { get; set; }
        public List<Garantor> Garantors { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
