using LoanApi.Models;
using System.Linq;

namespace LoanApi.Repository
{
    public interface INominalRepository : IGenericRepository<Nominal>
    {
        IQueryable<Nominal> GetAll();
        IQueryable<MainNominal> GetMain();
    }
}
