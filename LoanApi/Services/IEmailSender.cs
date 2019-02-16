using System.Threading.Tasks;

namespace LoanApi.Services
{
    public interface IEmailSender
    {
       Task SendEmailAsync(string email, string subject, string message);
    }
}