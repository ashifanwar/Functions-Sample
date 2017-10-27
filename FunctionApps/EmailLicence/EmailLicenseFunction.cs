using System.Collections.Generic;
using FunctionApps.Models;
using Microsoft.Azure.WebJobs.Host;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace FunctionApps.EmailLicence
{
    public class EmailLicenseFunction
    {
        public static void Run(string myBlob, string filename, Order ordersRow, TraceWriter log, out SendGridMessage message)
        {
            var email = ordersRow.Email;
            log.Info($"Order from Email: {email}\n License file name: {filename} ");

            var byteText = System.Text.Encoding.UTF8.GetBytes(myBlob);
            var initiator = new EmailAddress("noreply@paycor.com", "Paycor");

            var attachments = new List<SendGrid.Helpers.Mail.Attachment>
            {
                new SendGrid.Helpers.Mail.Attachment
                {
                    Content = System.Convert.ToBase64String(byteText),
                    Type = "text/plain",
                    Filename = "license.lic",
                    Disposition = "attachment",
                    ContentId = "License File"
                }
            };
            
            var recipientEmailAddresses = new List<EmailAddress> {new EmailAddress(email, "Ashif Anwar")};
            message = MailHelper.CreateSingleEmailToMultipleRecipients(initiator, recipientEmailAddresses,
                "Thank you for your order", "Your license file is attached", "Your license file is attached");
            message.AddAttachments(attachments);

        }
    }
}
