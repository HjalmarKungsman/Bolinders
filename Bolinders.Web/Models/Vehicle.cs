using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Web.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public int MakeId { get; set; }
        public virtual Make Make { get; set; }
        public string Model { get; set; }
        public string ModelDescription { get; set; }
        public int Year { get; set; }
        public int Milage { get; set; }
        public Decimal Price { get; set; }
        public string ChassiType { get; set; }
        public string Colour { get; set; }
        public string Gearbox { get; set; }
        public string FuelType { get; set; }
        public int Horsepowers { get; set; }
        public bool Used { get; set; }
        public string FacilityId { get; set; }
        public virtual Facility Facility { get; set; }
        // ImageUrl should be a List<string>
        public string ImageUrl { get; set; }
        public bool Leasable { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        //public Dictionary<string,string> Equipment { get; set; }
        public string Equipment { get; set; }
    }
}