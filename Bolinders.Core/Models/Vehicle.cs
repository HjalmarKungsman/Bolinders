﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Core.Models
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
        public int Mileage { get; set; }
        public Decimal Price { get; set; }
        //Sedan, Kombi, Halvkombi, Sportkubé, SUV, Cab, Minibuss, Övrigt, Transportbil
        public string BodyType { get; set; }
        public string Colour { get; set; }
        public string Gearbox { get; set; }
        //Bensin, Bensin/el, Bensin/etanol, Bensin/gas, Diesel, Diesel/el, El, Gas
        public string FuelType { get; set; }
        public int Horsepowers { get; set; }
        public bool Used { get; set; }
        public string FacilityId { get; set; }
        public virtual Facility Facility { get; set; }
        public ICollection<Image> Images { get; set; }
        public bool Leasable { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public ICollection<Equipment> Equipment { get; set; }
    }
}