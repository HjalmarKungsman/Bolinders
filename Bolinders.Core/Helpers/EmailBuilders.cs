using Bolinders.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Core.Helpers
{
    public class EmailBuilders
    {
        public static string ShareVehicleMessageBuilder(Vehicle vehicle)
        {
            var message = string.Format("<h1>{0} {1} {2}</h1>" +
                "<a href='{3}'>{0} {1} {2}</a>", vehicle.Make, vehicle.Model, vehicle.ModelDescription, vehicle.Id);

            return message;
        }
    }
}
