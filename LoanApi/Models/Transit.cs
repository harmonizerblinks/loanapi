using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class Transit : BaseModel
    {
        [Key]
        public int TransitId { get; set; }
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Method { get; set; }
        [Required]
        public int NominalId { get; set; }
        public Nominal Nominal { get; set; }
    }
}
