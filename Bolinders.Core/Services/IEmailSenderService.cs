using Bolinders.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Bolinders.Core.Services
{
    public interface IEmailSenderService
    {
        SmtpClient SmtpClientBuilder();
        Task<SmtpStatusCode> SendEmailToFacility(string senderName, string senderEmail, string reciever, string subject, string message, string phoneNumber);
        Task<SmtpStatusCode> SendEmailWithSharedVehicle(string reciever, Vehicle vehicle, string baseUrl);
        SmtpStatusCode SendImportNotification(List<Guid> addedVehicles);
    }
}
