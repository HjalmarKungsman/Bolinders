using Bolinders.Core.Enums;
using Bolinders.Core.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Core.Models.ViewModels
{
    public class VehicleEditViewModel
    {
        public Guid Id { get; set; }
        public string RegistrationNumber { get; set; }
        public int MakeId { get; set; }
        public virtual Make Make { get; set; }
        public string Model { get; set; }
        public string ModelDescription { get; set; }
        public int Year { get; set; }
        public int Mileage { get; set; }
        public double Price { get; set; }
        public BodyType BodyType { get; set; }
        public string Colour { get; set; }
        public Gearbox Gearbox { get; set; }
        public FuelType FuelType { get; set; }
        public int Horsepowers { get; set; }
        public bool Used { get; set; }
        public string FacilityId { get; set; }
        public virtual Facility Facility { get; set; }
        public List<Image> ImageList { get; set; }
        public ICollection<IFormFile> Images { get; set; }
        public bool Leasable { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public List<string> EquipmentString { get; set; }
        public List<Equipment> Equipment { get; set; }
    }
}
