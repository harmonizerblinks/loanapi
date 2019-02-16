using LoanApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LoanApi.Repository
{
    public class ChargeRepository : GenericRepository<Charge>, IChargeRepository
    {
        public ChargeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Charge> GetAll()
        {
            return _dbContext.Charge.Include(x => x.Cot).Include(x => x.Account).AsQueryable();
        }
    }
}
