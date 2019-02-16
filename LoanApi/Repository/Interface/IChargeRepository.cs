using LoanApi.Models;
using System.Linq;

namespace LoanApi.Repository
{
    public interface IChargeRepository : IGenericRepository<Charge>
    {
        IQueryable<Charge> GetAll();
    }
}
