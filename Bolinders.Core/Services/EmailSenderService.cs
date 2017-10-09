using Bolinders.Core.Helpers;
using Bolinders.Core.Models;
using Bolinders.Core.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Bolinders.Core.Services
{
    public class EmailSenderService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailSenderService(IOptions<SmtpSettings> settings)
        {
            _smtpSettings = settings.Value;
        }

        private static SmtpClient SmtpClientBuilder()
        {
            SmtpClient client = new SmtpClient("mailcluster.loopia.se")
            {
                Port = 587,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("bolindersbil@byteshift.se", "MSD8921%ewf13Xf")
            };
            return client;
        }

        public static async Task<SmtpStatusCode> SendEmailToFacility(string senderName, string senderEmail, string reciever, string subject, string message, string phoneNumber)
        {         
            try
            {
                var client = SmtpClientBuilder();

                message = message + "<br />Telefon: " + phoneNumber + "<br />Namn: " + senderName;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(senderEmail);
                mailMessage.To.Add(reciever);
                mailMessage.Body = message;
                mailMessage.Subject = "Skickat från kontaktformulär: " + subject;
                mailMessage.IsBodyHtml = true;
                await client.SendMailAsync(mailMessage);

                return SmtpStatusCode.Ok;
            }
            catch (Exception)
            {
                return SmtpStatusCode.GeneralFailure;
            }
        }

        public static async Task<SmtpStatusCode> SendEmailWithSharedVehicle(string reciever, Vehicle vehicle, string baseUrl)
        {
            try
            {
                var client = SmtpClientBuilder();

                var message = string.Format("<h1>{1} {2} {3}</h1>" +
                "<a href='{0}/bil/{1}-{2}-{3}-{4}'>{1} {2} {3}</a>" +
                "<p>Bilen finns hos Bolinders Bil AB</p>", baseUrl, vehicle.Make.Name, vehicle.Model, vehicle.ModelDescription, vehicle.UrlId);

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("bolindersbil@byteshift.se");
                mailMessage.To.Add(reciever);
                mailMessage.Body = message;
                mailMessage.Subject = String.Format("Bolidners Bil AB har en {0} {1} {2} i lager!", vehicle.Make.Name, vehicle.Model, vehicle.ModelDescription);
                mailMessage.IsBodyHtml = true;
                client.Send(mailMessage);

                return SmtpStatusCode.Ok;
            }
            catch (Exception)
            {
                return SmtpStatusCode.GeneralFailure;
            }
        }
    }
}
