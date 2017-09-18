using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bolinders.Core.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
