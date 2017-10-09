using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Core.Helpers
{
    public static class UrlHelpers
    {
        public static string GetBaseUrl(HttpContext _httpContext)
        {
            var request = _httpContext.Request;

            var host = request.Host.ToUriComponent();

            var pathBase = request.PathBase.ToUriComponent();

            return $"{request.Scheme}://{host}{pathBase}";
        }
    }
}
