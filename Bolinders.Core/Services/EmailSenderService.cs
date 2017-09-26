using Bolinders.Core.Helpers;
using Bolinders.Core.Models;
using Microsoft.AspNetCore.Hosting;
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
        
        public static SmtpStatusCode SendEmailToFacility(string senderName, string senderEmail, string reciever, string subject, string message, string phoneNumber)
        {
            SmtpClient client = new SmtpClient("mailcluster.loopia.se")
            {
                Port = 587,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("bolindersbil@byteshift.se", "MSD8921%ewf13Xf")
            };

            try
            {

                message = message + "<br />Telefon: " + phoneNumber + "<br />Namn: " + senderName;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(senderEmail);
                mailMessage.To.Add(reciever);
                mailMessage.Body = message;
                mailMessage.Subject = "Skickat från kontaktformulär: " + subject;
                client.Send(mailMessage);

                return SmtpStatusCode.Ok;
            }
            catch (Exception)
            {
                return SmtpStatusCode.GeneralFailure;
            }          
        }

        public static SmtpStatusCode SendEmailWithSharedVehicle(string reciever, Vehicle vehicle, string baseUrl)
        {
            SmtpClient client = new SmtpClient("mailcluster.loopia.se")
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("bolindersbil@byteshift.se", "MSD8921%ewf13Xf")
            };
            try
            {

                var message = string.Format("<h1>{0} {1} {2}</h1>" +
                   "<p>{5} vill att du ska kolla på:</p>" +
                "<a href='{3}'>{0} {1} {2}</a>" +
                "<p>Bilen finns hos Bolinders Bil AB</p>", vehicle.Make, vehicle.Model, vehicle.ModelDescription, baseUrl);

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("bolindersbil@byteshift.se");
                mailMessage.To.Add(reciever);
                mailMessage.Body = message;
                mailMessage.Subject = String.Format("Bolidners Bil AB har en {1} {2} {3} i lager!", vehicle.Make, vehicle.Model, vehicle.ModelDescription);
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
