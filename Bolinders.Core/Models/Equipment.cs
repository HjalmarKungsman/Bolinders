using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bolinders.Core.Models
{
    public class Equipment
    {
        public Guid Id { get; set; }
        public Guid VehicledId { get; set; }
        [ForeignKey("VehicledId")]
        public Vehicle Vehicle { get; set; }
        public string Value { get; set; }

        public Equipment()
        {

        }

        public Equipment(string value, Vehicle vehicle)
        {
            if (vehicle.Id != Guid.Empty)
            {
                Id = Guid.NewGuid();
            }

            VehicledId = vehicle.Id;
            Vehicle = vehicle;
            Value = value;
        }
    }
}
