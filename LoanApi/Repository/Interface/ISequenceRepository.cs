using LoanApi.Models;
using System.Threading.Tasks;

namespace LoanApi.Repository
{
    public interface ISequenceRepository : IGenericRepository<Sequence>
    {
        Task<string> GetCode(string type);

        Task<string> GetCode(int id);
    }
}
