using LoanApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LoanApi.Repository
{
    public class TellerRepository : GenericRepository<Teller>, ITellerRepository
    {
        public TellerRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Teller> GetAll()
        {
            return _dbContext.Teller.Include(x => x.AppUser).Include(x => x.Nominal).Include(x => x.Transactions).AsQueryable();
        }

        public IQueryable<Teller> GetUser(string id)
        {
            return _dbContext.Teller.Where(u=>u.Id == id).Include(x => x.AppUser).Include(x => x.Transactions).Include(x => x.Nominal).AsQueryable();
        }
    }
}
