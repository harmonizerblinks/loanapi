using LoanApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LoanApi.Repository
{
    public class CotRepository : GenericRepository<Cot>, ICotRepository
    {
        public CotRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Cot> GetAll()
        {
            return _dbContext.Cot.Include(x => x.Nominal).Include(x => x.AccountTypes).Include(x => x.Charges).AsQueryable();
        }
    }
}
