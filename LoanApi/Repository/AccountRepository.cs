using LoanApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LoanApi.Repository
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Account> GetAll()
        {
            return _dbContext.Account.Include(x => x.Customer).Include(x => x.AccountType).Include(x => x.Charges).Include(x => x.Cheque).Include(x => x.Employee).Include(x => x.Garantors).Include(x => x.Transactions).AsQueryable();
        }

        public IQueryable<Account> GetList()
        {
            return _dbContext.Account.Include(x => x.Customer).Include(x => x.AccountType).AsQueryable();
        }

        public IQueryable<Account> GetAll(int id)
        {
            return _dbContext.Account.Where(a=>a.AccountId == id).Include(x => x.Customer).Include(x => x.AccountType).Include(x => x.Charges).Include(x => x.Cheque).Include(x => x.Employee).Include(x => x.Garantors).Include(x => x.Transactions).AsQueryable();
        }
    }
}
