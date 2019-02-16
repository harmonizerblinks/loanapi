using LoanApi.Models;
using System.Linq;

namespace LoanApi.Repository
{
    public interface IChequeRepository : IGenericRepository<Cheque>
    {
        IQueryable<Cheque> GetAll();
    }
}
