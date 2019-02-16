using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class SmsApi : BaseModel
    {
        [Key]
        public int SmsApiId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string SenderId { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public bool Default { get; set; }
    }
}
