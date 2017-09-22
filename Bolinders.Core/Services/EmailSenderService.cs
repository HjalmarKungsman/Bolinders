using Bolinders.Core.Helpers;
using Bolinders.Core.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Bolinders.Core.Services
{
    public class EmailSenderService
    {
        public static void SendEmailToFacility(string sender, Facility reciever, string subject, string message)
        {
            SmtpClient client = new SmtpClient("mysmtpserver")
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("username", "password")
            };

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(sender);
            mailMessage.To.Add(reciever.Email);
            mailMessage.Body = message;
            mailMessage.Subject = "Skickat från kontaktformulär - " + subject;
            client.Send(mailMessage);
        }

        public static void SendEmailWithSharedVehicle(string senderName, string senderEmail, string reciever, Vehicle vehicle)
        {
            SmtpClient client = new SmtpClient("mysmtpserver")
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("username", "password")
            };

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderEmail);
            mailMessage.To.Add(reciever);
            mailMessage.Body = EmailBuilders.ShareVehicleMessageBuilder(vehicle);
            mailMessage.Subject = String.Format("{0} vill tipsa dig om en {1} {2} {3}",senderName, vehicle.Make, vehicle.Model, vehicle.ModelDescription);
            client.Send(mailMessage);
        }
    }
}
