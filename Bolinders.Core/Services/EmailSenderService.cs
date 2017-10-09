using Bolinders.Core.DataAccess;
using Bolinders.Core.Helpers;
using Bolinders.Core.Models;
using Bolinders.Core.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Bolinders.Core.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ApplicationDbContext _context;
        private HttpContext _httpContext;


        public EmailSenderService(IOptions<SmtpSettings> settings, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _smtpSettings = settings.Value;
            _context = context;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public SmtpClient SmtpClientBuilder()
        {
            SmtpClient client = new SmtpClient(_smtpSettings.SmtpServer)
            {
                Port = _smtpSettings.Port,
                UseDefaultCredentials = _smtpSettings.UseDefaultCredentials,
                Credentials = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password)
            };
            return client;
        }

        public async Task<SmtpStatusCode> SendEmailToFacility(string senderName, string senderEmail, string reciever, string subject, string message, string phoneNumber)
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

        public async Task<SmtpStatusCode> SendEmailWithSharedVehicle(string reciever, Vehicle vehicle, string baseUrl)
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

        public SmtpStatusCode SendImportNotification(List<Guid> addedVehicles)
        {
            try
            {
                List<Vehicle> listOfVehicles = new List<Vehicle>();
                var client = SmtpClientBuilder();

                foreach (var vehicle in addedVehicles)
                {
                    listOfVehicles.Add(_context.Vehicles.Where(x => x.Id == vehicle).FirstOrDefault());
                }

                foreach (var vehicle in listOfVehicles)
                {
                    var baseUrl = UrlHelpers.GetBaseUrl(_httpContext);

                    var message = string.Format("<h1>{1} {2} {3}</h1>" +
                    "<a href='{0}/bil/{1}-{2}-{3}-{4}'>Till annonsen</a>",
                    baseUrl, vehicle.Make.Name, vehicle.Model, vehicle.ModelDescription, vehicle.UrlId);
                }

                return SmtpStatusCode.Ok;
            }
            catch (Exception)
            {

                return SmtpStatusCode.GeneralFailure;
            }

        }
    }
}
