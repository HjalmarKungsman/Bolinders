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
        public Guid VehicleId { get; set; }
        [ForeignKey("VehicleId")]
        public Vehicle Vehicle { get; set; }
        public string ImageUrl { get; set; }

        public Image(string imageUrl, Vehicle vehicle)
        {
            VehicleId = vehicle.Id;
            Vehicle = vehicle;
            ImageUrl = imageUrl;
            
        }
    }
    
}
