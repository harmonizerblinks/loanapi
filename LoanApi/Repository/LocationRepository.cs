using LoanApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LoanApi.Repository
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {
        public LocationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Location> GetAll()
        {
            return _dbContext.Location.Include(x => x.Customers).AsQueryable();
        }
    }
}
