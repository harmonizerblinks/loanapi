using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Teller : BaseModel
    {
        [Key]
        public int TellerId { get; set; }
        [Required]
        public int NominalId { get; set; }
        [Required]
        public string Id { get; set; }
        [NotMapped]
        public string Pin { get; set; }

        public AppUser AppUser { get; set; }
        public Nominal Nominal { get; set; }
        public IList<Transaction> Transactions { get; set;}
    }
}
