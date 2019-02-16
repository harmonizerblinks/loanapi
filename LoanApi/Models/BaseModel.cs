using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanApi.Models
{
    public class BaseModel
    {
        [Required]
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public string MUserId { get; set; }
        public DateTime? MDate { get; set; }

        //[ForeignKey("UserId")]
        //public AppUser User { get; set; }
    }
}
