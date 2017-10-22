using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Core.Models.SettingModels
{
    public class FtpSettings
    {
        public FtpSettings()
        {
        }
        public string FtpServer { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
