using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipesLibrary.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("mishkavhristova@abv.bg", "Admin");
            var to = new EmailAddress(email, email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
