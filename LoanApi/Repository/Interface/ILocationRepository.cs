using LoanApi.Models;
using System.Linq;

namespace LoanApi.Repository
{
    public interface ILocationRepository : IGenericRepository<Location>
    {
        IQueryable<Location> GetAll();
    }
}
