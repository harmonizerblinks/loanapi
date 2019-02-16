using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Session : BaseModel
    {
        [Key]
        public int SessionId { get; set; }
        public DateTime SessionDate { get; set; }
        public string Status { get; set; }
    }
}
