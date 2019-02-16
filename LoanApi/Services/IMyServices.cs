using LoanApi.Models;
using System.Threading.Tasks;

namespace LoanApi.Services
{
    public interface IMyServices
    {
        Task<string> GetCode(string type);
        
    }
}
