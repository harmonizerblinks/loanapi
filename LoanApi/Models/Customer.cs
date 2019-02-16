using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Customer : BaseModel
    {
        [Key]
        public int CustomerId { get; set; }
        public string Code { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string MaritalStatus { get; set; }
        [Required]
        public int LocationId { get; set; }
        public string Image { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }

        public string Business { get; set; }
        public string BusType { get; set; }
        public string BusAddress { get; set; }

        public string Nok { get; set; }
        public string Rnok { get; set; }
        public string NokMobile { get; set; }
        public string NokAddress { get; set; }
        public string Status { get; set; }

        public Location Location { get; set; }
        public List<Account> Accounts { get; set; }
    }
}
