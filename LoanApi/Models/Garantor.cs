using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Garantor : BaseModel
    {
        [Key]
        public int GarantorId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string MaritalStatus { get; set; }
        public int AccountId { get; set; }
        [Required]
        public string Address { get; set; }

        public Account Account { get; set; }
    }
}
