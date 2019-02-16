using LoanApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Repository
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Customer> GetList()
        {
            return _dbContext.Customer.Include(x => x.Location).AsQueryable();
        }

        public IQueryable<Customer> GetAll()
        {
            return _dbContext.Customer.Include(x => x.Accounts).Include(x => x.Location).AsQueryable();
        }
    }
}
