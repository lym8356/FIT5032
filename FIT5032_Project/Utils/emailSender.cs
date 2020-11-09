using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FIT5032_Project.Utils
{
    public class emailSender
    {
        private const String API_KEY = "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";
        public void Send(String[] toEmail, String subject, String contents, byte[] fileBytes,  String filename)
        {
            var client = new SendGridClient(API_KEY);
            //var from = new EmailAddress("noreply@localhost.com", "FIT5032 Example Email User");
            var from = new EmailAddress("lym8356@gmail.com");

            var tos = new List<EmailAddress>();

            foreach(var email in toEmail) { tos.Add(new EmailAddress(email)); }

            var plainTextContent = contents;
            var htmlContent = "<p>" + contents + "</p>";
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, plainTextContent, htmlContent);

            if(fileBytes.Length > 0 && filename != null)
            {
                var file = Convert.ToBase64String(fileBytes);
                msg.AddAttachment(filename,file);
            }

            var response = client.SendEmailAsync(msg);
        }
    }
}