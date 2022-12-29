using MimeKit;
using System;
using System.Threading.Tasks;
using targeteo.pl.Model.Entities;
using targeteo.pl.Model.ViewModel;

namespace targeteo.pl.Services
{
    public class EmailService
    {
        public static async Task SendAsync(string subject, string msg, string toAddress, BodyBuilder bodyBuilder, CompanyInformation companyinfo)
        {
            if(bodyBuilder == null)
            {
                bodyBuilder = new BodyBuilder();
            }

            try
            {
                var serviceInfo = companyinfo;
                MimeMessage message = new MimeMessage();
                MailboxAddress from = new MailboxAddress(serviceInfo.Name, serviceInfo.Email);
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress(serviceInfo.Name, toAddress);
                message.To.Add(to);

                message.Subject = subject;

                bodyBuilder.HtmlBody = msg;

                //foreach (var fileName in fileNames)
                //{
                //    var pathImage = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                //    var image = bodyBuilder.LinkedResources.Add(pathImage);
                //    image.ContentId = MimeUtils.GenerateMessageId();
                //    bodyBuilder.HtmlBody += string.Format(@"<p>Hey!</p><img src=""cid:{0}"">", image.ContentId);
                //    //builder.HtmlBody = string.Format(@"<p>Hey!</p><img src=""cid:{0}"">", image.ContentId);
                //}
                    


                message.Body = bodyBuilder.ToMessageBody();


                MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect(serviceInfo.SmtpServer, 465, true);
                client.Authenticate(serviceInfo.Email, serviceInfo.EmailPassword);

                await client.SendAsync(message);
                client.Disconnect(true);
                client.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
