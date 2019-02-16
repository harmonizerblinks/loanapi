using LoanApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LoanApi.Repository
{
    public class AccountTypeRepository : GenericRepository<AccountType>, IAccountTypeRepository
    {
        public AccountTypeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<AccountType> GetAll()
        {
            return _dbContext.AccountType.Include(x => x.Cot).Include(x => x.Sequence).Include(x => x.Nominal).Include(x => x.Accounts).AsQueryable();
        }
    }
}
