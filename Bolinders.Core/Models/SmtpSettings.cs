using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Core.Models
{
    public class SmtpSettings
    {
        public SmtpSettings()
        {

        }
        public int Port { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SmtpServer { get; set; }
    }
}
