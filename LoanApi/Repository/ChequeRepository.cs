using LoanApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LoanApi.Repository
{
    public class ChequeRepository : GenericRepository<Cheque>, IChequeRepository
    {
        public ChequeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Cheque> GetAll()
        {
            return _dbContext.Cheque.Include(x => x.Transaction).Include(x => x.Account).AsQueryable();
        }
    }
}
