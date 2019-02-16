using LoanApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Repository
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        IQueryable<Customer> GetList();
        IQueryable<Customer> GetAll();
    }
}
