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
        [Key]
        public int Id { get; set; }
        public Guid VehicledId { get; set; }
        [ForeignKey("VehicledId")]
        public Vehicle Vehicle { get; set; }
        public string Value { get; set; }
    }
}
