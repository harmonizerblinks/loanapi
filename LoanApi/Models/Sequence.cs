using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Sequence : BaseModel
    {
        [Key]
        public int SequenceId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Prefix { get; set; }
        [Required]
        public int Counter { get; set; }
        [Required]
        public int Length { get; set; }
    }
}
