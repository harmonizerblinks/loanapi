using LoanApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LoanApi.Repository
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Transaction> GetAll()
        {
            return _dbContext.Transaction.Include(x => x.Account).Include(x => x.Teller).Include(x => x.Nominal).AsQueryable();
        }
    }
}
