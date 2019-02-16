using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using LoanApi.Services;
using LoanApi.Models;

namespace LoanApi.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string link, string pin, AppUser user)
        {
            if (user.Email == null || link == null)
            {
                throw new Exception("Email or link cannot be null");
            }
            //$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(link)}'>clicking here</a>."
            string message = $"<table style='background-color: #f6f6f6; width: 100%;'><tr><td></td><td width='600' style='display: block !important; max-width: 600px !important; margin: 0 auto !important; clear: both !important;''><div style='max-width: 600px; margin: 0 auto; display: block; padding: 20px;'><table width='100%' cellpadding='0' cellspacing='0' style='background: #fff; border: 1px solid #e9e9e9; border-radius: 3px;'><tr><td style='padding: 20px;'><table  cellpadding='0' cellspacing='0'><tr><td><img class='img-responsive' style='width: 100%; height: 200px;' src='{HtmlEncoder.Default.Encode("https://admin.acyst.tech/assets/img/Acyst-banner1b.jpg")}'/></td></tr><tr><td style='padding: 30px 0 0;'><h3>Welcome {user.Employee.FullName}</h3></td></tr><tr><td style='padding: 0 0 20px;'><b>Login Details</b><br/> Username: <b> {user.UserName} </b><br/> Password: <b> {pin} </b></td></tr><tr><td style='padding: 0 0 20px;'>We may need to send you critical information about our service and it is important that we have an accurate email address.</td></tr><tr><td style='padding: 0 0 20px; text-align: center;'><a href='{HtmlEncoder.Default.Encode(link)}' style='text-decoration: none; color: #FFF; background-color: #1ab394; border: solid #1ab394; border-width: 5px 10px; line-height: 2; font-weight: bold; text-align: center; cursor: pointer; display: inline-block; border-radius: 5px; text-transform: capitalize;'>Confirm email address</a></td></tr></table></td></tr></table><div style='width: 100%; font-size: 12px; clear: both; color: #999; padding: 20px;'><table width='100%'><tr><td style='padding: 0 0 20px; text-align: center;'>Powered by <a href='{HtmlEncoder.Default.Encode("http://acyst.tech")}' style='color: #999;'>@ACYST</a> TECHNOLOGY.</td></tr></table></div></div></td><td></td></tr></table>";
            return emailSender.SendEmailAsync(user.Email, "ACYST TECH'S: Confirm your Email", message);
        }
        
        public static Task SendEmailResetConfirmationAsync(this IEmailSender emailSender, string link, string pin, AppUser user)
        {
            if (user.Email == null || link == null)
            {
                throw new Exception("Email or link cannot be null");
            }
            //$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(link)}'>clicking here</a>."
            string message = $"<table style='background-color: #f6f6f6; width: 100%;'><tr><td></td><td width='600' style='display: block !important; max-width: 600px !important; margin: 0 auto !important; clear: both !important;''><div style='max-width: 600px; margin: 0 auto; display: block; padding: 20px;'><table width='100%' cellpadding='0' cellspacing='0' style='background: #fff; border: 1px solid #e9e9e9; border-radius: 3px;'><tr><td style='padding: 20px;'><table  cellpadding='0' cellspacing='0'><tr><td><img class='img-responsive' style='width: 100%; height: 200px;' src='{HtmlEncoder.Default.Encode("https://admin.acyst.tech/assets/img/Acyst-banner1b.jpg")}'/></td></tr><tr><td style='padding: 30px 0 0;'><h3>Welcome Back {user.Employee.FullName}</h3></td></tr><tr><td style='padding: 0 0 20px;'><b>New Login Details</b><br/> Username: <b> {user.UserName} </b><br/> Password: <b> {pin} </b></td></tr><tr><td style='padding: 0 0 20px;'>We may need to send you critical information about our service and it is important that we have an accurate email address.</td></tr><tr><td style='padding: 0 0 20px; text-align: center;'><a href='{HtmlEncoder.Default.Encode(link)}' style='text-decoration: none; color: #FFF; background-color: #1ab394; border: solid #1ab394; border-width: 5px 10px; line-height: 2; font-weight: bold; text-align: center; cursor: pointer; display: inline-block; border-radius: 5px; text-transform: capitalize;'>Confirm Reset</a></td></tr></table></td></tr></table><div style='width: 100%; font-size: 12px; clear: both; color: #999; padding: 20px;'><table width='100%'><tr><td style='padding: 0 0 20px; text-align: center;'>Powered by <a href='{HtmlEncoder.Default.Encode("http://acyst.tech")}' style='color: #999;'>@ACYST</a> TECHNOLOGY.</td></tr></table></div></div></td><td></td></tr></table>";
            return emailSender.SendEmailAsync(user.Email, "ACYST TECH'S: Confirm Reset Account", message);
        }
        
        public static Task SendEmailConfirmAsync(this IEmailSender emailSender, string link, string email, string Name)
        {
            if (email == null)
            {
                throw new Exception("Email or link cannot be null");
            }
            //$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(link)}'>clicking here</a>."
            string message = $"<table style='background-color: #f6f6f6; width: 100%;'><tr><td></td><td width='600' style='display: block !important; max-width: 600px !important; margin: 0 auto !important; clear: both !important;''><div style='max-width: 600px; margin: 0 auto; display: block; padding: 20px;'><table width='100%' cellpadding='0' cellspacing='0' style='background: #fff; border: 1px solid #e9e9e9; border-radius: 3px;'><tr><td style='padding: 20px;'><table  cellpadding='0' cellspacing='0'><tr><td style='padding: 30px 0 0;'><h3>Hello {Name}</h3></td></tr><tr><td style='padding: 0 0 20px;'>Your Email Address Has been Confirm Successfully. Proceed to Login</td></tr><tr><td style='padding: 0 0 20px; text-align: center;'><a href='{HtmlEncoder.Default.Encode(link)}' style='text-decoration: none; color: #FFF; background-color: #1ab394; border: solid #1ab394; border-width: 5px 10px; line-height: 2; font-weight: bold; text-align: center; cursor: pointer; display: inline-block; border-radius: 5px; text-transform: capitalize;'>Click To Login</a></td></tr></table></td></tr></table><div style='width: 100%; font-size: 12px; clear: both; color: #999; padding: 20px;'><table width='100%'><tr><td style='padding: 0 0 20px; text-align: center;'>Powered by <a href='{HtmlEncoder.Default.Encode("http://acyst.tech")}' style='color: #999;'>@ACYST</a> TECHNOLOGY.</td></tr></table></div></div></td><td></td></tr></table>";
            return emailSender.SendEmailAsync(email, "ACYST TECH'S: Account Confirmed", message);
        }

        public static Task SendSubcribeEmailAsync(this IEmailSender emailSender, string email, string Name)
        {
            if (email == null)
            {
                throw new Exception("Email or link cannot be null");
            }
            //$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(link)}'>clicking here</a>."
            string message = $"<table style='background-color: #f6f6f6; width: 100%;'><tr><td></td><td width='600' style='display: block !important; max-width: 600px !important; margin: 0 auto !important; clear: both !important;''><div style='max-width: 600px; margin: 0 auto; display: block; padding: 20px;'><table width='100%' cellpadding='0' cellspacing='0' style='background: #fff; border: 1px solid #e9e9e9; border-radius: 3px;'><tr><td style='padding: 20px;'><table  cellpadding='0' cellspacing='0'><tr><td style='padding: 30px 0 0;'><h3>Hello {email}</h3></td></tr><tr><td style='padding: 0 0 20px;'>Thanks For Subcribing to our News Letter.</td></tr></table></td></tr></table><div style='width: 100%; font-size: 12px; clear: both; color: #999; padding: 20px;'><table width='100%'><tr><td style='padding: 0 0 20px; text-align: center;'>Powered by <a href='{HtmlEncoder.Default.Encode("http://acyst.tech")}' style='color: #999;'>@ACYST</a> TECHNOLOGY.</td></tr></table></div></div></td><td></td></tr></table>";
            return emailSender.SendEmailAsync(email, Name+": Thank For Subcribing", message);
        }

        public static Task SendResetPasswordAsync(this IEmailSender emailSender, string email, string callbackUrl)
        {
            return emailSender.SendEmailAsync(email, "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }
    }
}
