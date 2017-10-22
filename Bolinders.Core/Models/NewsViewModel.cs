using Bolinders.Core.Models.SettingModels;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Core.Models
{
    public class NewsViewModel
    {

        public NewsViewModel(IOptions<MyNewsDeskSettings> myNewsDeskSettings)
        {
            Url = myNewsDeskSettings.Value.Url;
            Source = myNewsDeskSettings.Value.Source;
            SortBy = myNewsDeskSettings.Value.SortBy;
            ApiKey = myNewsDeskSettings.Value.ApiKey;
            FullUri = myNewsDeskSettings.Value.FullUri;
        }
        public string Url { get; set; }
        public string Source { get; set; }
        public string SortBy { get; set; }
        public string ApiKey { get; set; }
        public string FullUri { get; set; }
    }
}
