using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bolinders.Core.Models
{
    public class Vehicle
    {
        [Key]
        public Guid Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UrlId { get; set; }
        public string RegistrationNumber { get; set; }
        public int MakeId { get; set; }
        public virtual Make Make { get; set; }
        public string Model { get; set; }
        public string ModelDescription { get; set; }
        public int Year { get; set; }
        public int Mileage { get; set; }
        public Decimal Price { get; set; }
        public BodyType BodyType { get; set; }
        public string Colour { get; set; }
        public GearBox Gearbox { get; set; }
        public FuelType FuelType { get; set; }
        public int Horsepowers { get; set; }
        public bool Used { get; set; }
        public string FacilityId { get; set; }
        public virtual Facility Facility { get; set; }
        public ICollection<Image> Images { get; set; }
        public bool Leasable { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public List<Equipment> Equipment { get; set; }


        public Vehicle()
        {

        }
    }
}
