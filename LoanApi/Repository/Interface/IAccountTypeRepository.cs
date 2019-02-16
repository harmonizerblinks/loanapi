using LoanApi.Models;
using System.Linq;

namespace LoanApi.Repository
{
    public interface IAccountTypeRepository : IGenericRepository<AccountType>
    {
        IQueryable<AccountType> GetAll();
    }
}
