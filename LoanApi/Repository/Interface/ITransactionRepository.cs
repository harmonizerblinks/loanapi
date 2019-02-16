using LoanApi.Models;
using System.Linq;

namespace LoanApi.Repository
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        IQueryable<Transaction> GetAll();
    }
}
