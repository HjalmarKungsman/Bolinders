﻿using Bolinders.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bolinders.Core.Models.Entities
{
    public class Vehicle
    {
        [Key]
        public Guid Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UrlId { get; set; }
        [Display(Name="Registreringsnummer")]
        public string RegistrationNumber { get; set; }
        [Display(Name = "Bilmärke")]
        public int MakeId { get; set; }
        [Display(Name = "Bilmärke")]
        public virtual Make Make { get; set; }
        [Display(Name = "Modell")]
        public string Model { get; set; }
        [Display(Name = "Modellbeskrivning")]
        public string ModelDescription { get; set; }
        [Display(Name = "Tillverkningsår")]
        public int Year { get; set; }
        [Display(Name = "Mätarställning")]
        public int Mileage { get; set; }
        [Display(Name = "Pris")]
        public double Price { get; set; }
        [Display(Name = "Kaross")]
        public BodyType BodyType { get; set; }
        [Display(Name = "Färg")]
        public string Colour { get; set; }
        [Display(Name = "Växellåda")]
        public Gearbox Gearbox { get; set; }
        [Display(Name = "Drivmedel")]
        public FuelType FuelType { get; set; }
        [Display(Name = "Hästkrafter")]
        public int Horsepowers { get; set; }
        [Display(Name = "Begagnad")]
        public bool Used { get; set; }
        [Display(Name = "Ort/Anläggning")]
        public string FacilityId { get; set; }
        [Display(Name = "Ort/Anläggning")]
        public virtual Facility Facility { get; set; }
        [Display(Name = "Bilder")]
        public List<Image> Images { get; set; }
        [Display(Name = "Leasningsbar")]
        public bool Leasable { get; set; }
        [Display(Name = "Skapad")]
        public DateTime Created { get; set; }
        [Display(Name = "Uppdaterad")]
        public DateTime Updated { get; set; }
        [Display(Name = "Utrustning")]
        public List<Equipment> Equipment { get; set; }


        public Vehicle()
        {

        }
    }
}
