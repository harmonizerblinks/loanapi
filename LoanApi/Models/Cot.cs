using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Cot : BaseModel
    {
        [Key]
        public int CotId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public int NominalId { get; set; }
        [Required]
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }
        [Required]
        public string Frequency { get; set; }

        public Nominal Nominal { get; set; }
        public List<Charge> Charges { get; set; }
        public List<AccountType> AccountTypes { get; set; }
    }
}
