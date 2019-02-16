using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Sms : BaseModel
    {
        [Key]
        public int SmsId { get; set; }
        public string Type { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string Message { get; set; }
        public int Code { get; set; }
        public string Response { get; set; }
        public int? AccountId { get; set; }
        public int? CustomerId { get; set; }

        public Account Account { get; set; }
        public Customer Customer { get; set; }
    }
}
