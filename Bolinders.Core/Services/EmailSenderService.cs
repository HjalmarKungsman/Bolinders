using Bolinders.Core.DataAccess;
using Bolinders.Core.Helpers;
using Bolinders.Core.Models;
using Bolinders.Core.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
                mailMessage.From = new MailAddress(_smtpSettings.Email);
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
                

                foreach (var vehicle in addedVehicles)
                {
                    listOfVehicles.Add(_context.Vehicles.Where(x => x.Id == vehicle).Include(x => x.Facility).Include(x => x.Make).FirstOrDefault());
                }


                Dictionary<string, string> mailQueue = new Dictionary<string, string>();

                foreach (var vehicle in listOfVehicles)
                {
                    var baseUrl = UrlHelpers.GetBaseUrl(_httpContext);

                    if (vehicle.FacilityId == "BB1")
                    {
                        mailQueue.Add(vehicle.FacilityId, MessageNotificationBuilder(vehicle, baseUrl));
                    }

                    if (vehicle.FacilityId == "BB2")
                    {
                        mailQueue.Add(vehicle.FacilityId, MessageNotificationBuilder(vehicle, baseUrl));
                    }

                    if (vehicle.FacilityId == "BB3")
                    {
                        mailQueue.Add(vehicle.FacilityId, MessageNotificationBuilder(vehicle, baseUrl));
                    }
                }

                var BB1 = mailQueue.Where(x => x.Key == "BB1").ToDictionary(x => x.Key, y => y.Value );
                if (BB1.Count > 0)
                {
                    SendNotification(BB1);
                }
                
                var BB2 = mailQueue.Where(x => x.Key == "BB2").ToDictionary(x => x.Key, y => y.Value);
                if (BB2.Count > 0)
                {
                    SendNotification(BB2);
                }

                var BB3 = mailQueue.Where(x => x.Key == "BB3").ToDictionary(x => x.Key, y => y.Value);
                if (BB3.Count > 0)
                {
                    SendNotification(BB3);
                }

                return SmtpStatusCode.Ok;

            }
            catch (Exception e)
            {

                return SmtpStatusCode.GeneralFailure;
            }

        }
        private string MessageNotificationBuilder(Vehicle vehicle, string baseUrl)
        {
            return string.Format("<h1>{1} {2} {3}</h1>" +
                        "<a href='{0}/bil/{1}-{2}-{3}-{4}'>Till annonsen</a><br/>",
                        baseUrl, vehicle.Make.Name, vehicle.Model, vehicle.ModelDescription, vehicle.UrlId);
        }

        private SmtpStatusCode SendNotification(Dictionary<string, string> facility)
        {
            var client = SmtpClientBuilder();
            var reciever = _context.Facilities.Find(facility.Keys.FirstOrDefault()).Email;
            //var reciever = "Din_testmail";

            var message = facility.Values.FirstOrDefault().ToString();

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_smtpSettings.Email);
            mailMessage.To.Add(reciever);
            mailMessage.Body = message;
            mailMessage.Subject = facility.Count() + " ändring(ar) har gjorts via importen.";
            mailMessage.IsBodyHtml = true;
            client.Send(mailMessage);

            return SmtpStatusCode.Ok;
        }
    }
}
