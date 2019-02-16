using LoanApi.Models;
using System.Linq;

namespace LoanApi.Repository
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        IQueryable<Account> GetAll();
        IQueryable<Account> GetList();
        IQueryable<Account> GetAll(int id);
    }
}
