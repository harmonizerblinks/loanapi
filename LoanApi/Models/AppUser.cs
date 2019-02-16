using Microsoft.AspNetCore.Identity;
using System;

namespace LoanApi.Models
{
    public class AppUser : IdentityUser
    {
        public int? EmployeeId { get; set; }
        public string UserType { get; set; }
        public bool IsLoggedIn { get; set; }
        public DateTime Login { get; set; }
        public DateTime LogOut { get; set; }
        public string MUserId { get; set; }
        public DateTime? MDate { get; set; }

        public Employee Employee { get; set; }
    }
}
