using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Core.Models.SettingModels
{
    public class MyNewsDeskSettings
    {
        public MyNewsDeskSettings()
        {
  
        }
        public string Url { get; set; }
        public string Source { get; set; }
        public string SortBy { get; set; }
        public string ApiKey { get; set; }
        public string FullUri { get; set; }
    }
}
