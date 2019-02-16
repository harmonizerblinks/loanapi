using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LoanApi.Models
{
    public class Employee : BaseModel
    {
        [Key]
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Mobile { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Nationality { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        public List<Account> Accounts { get; set; }
    }
}
