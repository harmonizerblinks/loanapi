using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class SmsBoardcast : BaseModel
    {
        [Key]
        public int SmsBoardcastId { get; set; }
        public string Option { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string Message { get; set; }
        public int Code { get; set; }
        public string Response { get; set; }
        
    }
}
