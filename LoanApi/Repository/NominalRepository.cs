using LoanApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LoanApi.Repository
{
    public class NominalRepository : GenericRepository<Nominal>, INominalRepository
    {
        public NominalRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public IQueryable<Nominal> GetAll()
        {
            return _dbContext.Nominal.Include(x => x.MainNominal).Include(x => x.Transactions).AsQueryable();
        }
        
        public IQueryable<MainNominal> GetMain()
        {
            return _dbContext.MainNominal.AsQueryable();
        }
    }
}
