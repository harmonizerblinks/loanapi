using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class AddUser : BaseModel
    {
        public string Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string Fullname { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Mobile { get; set; }

        [Required]
        public int? EmployeeId { get; set; }
        [Required]
        public string UserType { get; set; }
        public bool IsLoggedIn { get; set; }
        public DateTime Login { get; set; }
        public DateTime LogOut { get; set; }
    }
}
