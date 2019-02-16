using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System;

namespace LoanApi.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public IConfiguration Configuration { get; }
        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            string FromEmail = "no-reply@acyst.tech"; 
            return Execute(FromEmail, subject, message, email);
        }

        public  Task Execute(string fromEmail, string subject, string message, string email)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("acystrest@gmail.com".Trim(), "Stevie@1".Trim());

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromEmail);
            mailMessage.To.Add(email);
            mailMessage.Body = message;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            return  client.SendMailAsync(mailMessage);
        }

        //SmtpClient client = new SmtpClient();
        //client.Host = Configuration.GetSection("AppSettings")["Host"]; // "smtp.gmail.com";
        //client.Port = Convert.ToInt32(Configuration.GetSection("AppSettings")["Port"]); //587;
        //client.UseDefaultCredentials = Convert.ToBoolean(Configuration.GetSection("AppSettings")["UseDefaultCredentials"]);
        //client.EnableSsl = Convert.ToBoolean(Configuration.GetSection("AppSettings")["EnableSsl"]);
        //string Email = Configuration.GetSection("AppSettings")["Email"];
        //string Password = Configuration.GetSection("AppSettings")["Password"];
        //client.Credentials = new NetworkCredential(Email.Trim(), Password.Trim());


    }
}
