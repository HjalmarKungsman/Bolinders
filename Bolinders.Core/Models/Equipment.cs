using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bolinders.Core.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        public Guid VehicledId { get; set; }
        [ForeignKey("VehicledId")]
        public Vehicle Vehicle { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
