using LoanApi.Models;
using System.Linq;

namespace LoanApi.Repository
{
    public interface ICotRepository : IGenericRepository<Cot>
    {
        IQueryable<Cot> GetAll();
    }
}
