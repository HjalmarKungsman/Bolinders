using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bolinders.Core.Models
{
    public class Image
    {
        public int Id { get; set; }
        public int VehicledId { get; set; }
        [ForeignKey("VehicledId")]
        public Vehicle Vehicle { get; set; }
        public string ImageUrl { get; set; }
    }
}
