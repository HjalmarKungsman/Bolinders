using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bolinders.Core.Models.Entities
{
    public class Image
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        [ForeignKey("VehicleId")]
        public Vehicle Vehicle { get; set; }
        public string ImageUrl { get; set; }

        public Image(string imageUrl, Guid vehicleId)
        {
            Id = Guid.NewGuid();
            VehicleId = vehicleId;
            ImageUrl = imageUrl;
        }

        public Image()
        {

        }
    }
    
}
