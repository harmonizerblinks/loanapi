using LoanApi.Models;
using System.Linq;

namespace LoanApi.Repository
{
    public interface ITellerRepository : IGenericRepository<Teller>
    {
        IQueryable<Teller> GetAll();
        IQueryable<Teller> GetUser(string id);
    }
}
