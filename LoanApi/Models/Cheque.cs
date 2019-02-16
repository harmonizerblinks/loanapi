using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Cheque
    {
        [Key]
        public int ChequeId { get; set; }
        [Required]
        public int AccountId { get; set; }
        [Required]
        public int TransactionId { get; set; }
        [Required]
        public string Number { get; set; }
        [Required]
        public string UserId { get; set; }
        public DateTime Date { get; set; }

        public Account Account { get; set; }
        public Transaction Transaction { get; set; }
    }
}
