﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Bolinders.Core.Models;
using Bolinders.Core.DataAccess;

namespace Bolinders.Core.DataAccess
{
    public static class Seed
    {
        public static void FillIfEmpty(ApplicationDbContext ctx, UserManager<ApplicationUser> userManager)
        {
            if (!ctx.Facilities.Any())
            {
                ctx.Facilities.Add(new Facility { Id = "BB1", Name = "Jönköping", Address = "Gata 1", ZipCode="12345", City = "Jönköping", Email = "jonkoping@bolinder.se", PhoneNumber = "036-102030", Longitude = "54,12345", Latitude = "10,12345" });
                ctx.Facilities.Add(new Facility { Id = "BB2", Name = "Göteborg", Address = "Gata 1", ZipCode = "12345", City = "Göteborg", Email = "goteborg@bolinder.se", PhoneNumber = "036-102030", Longitude = "54,12345", Latitude = "10,12345" });
                ctx.Facilities.Add(new Facility { Id = "BB3", Name = "Värnamo", Address = "Gata 1", ZipCode = "12345", City = "Värnamo", Email = "varnamo@bolinder.se", PhoneNumber = "036-102030", Longitude = "54,12345", Latitude = "10,12345" });
            }

            if(!userManager.Users.Any())
            {
                var user1 = new ApplicationUser { UserName = "jonkoping@bolindersbil.se", Email = "jonkoping@bolindersbil.se", FacilityId="BB1", EmailConfirmed = true };
                var user2 = new ApplicationUser { UserName = "goteborg@bolindersbil.se", Email = "goteborg@bolindersbil.se", FacilityId = "BB2", EmailConfirmed = true };
                var user3 = new ApplicationUser { UserName = "varnamo@bolindersbil.se", Email = "varnamo@bolindersbil.se", FacilityId = "BB3", EmailConfirmed = true };

                Task.WaitAll(userManager.CreateAsync(user1, "Abcd1234"));
                Task.WaitAll(userManager.CreateAsync(user2, "Abcd1234"));
                Task.WaitAll(userManager.CreateAsync(user3, "Abcd1234"));
            }

            if (!ctx.Make.Any())
            {
                var make = new List<Make>
                {
                    new Make {Name = "Volvo", LogotypeFileName = "volvo.png"},
                    new Make {Name = "SAAB", LogotypeFileName = "saab.png"},
                    new Make {Name = "BMW", LogotypeFileName = "bmw.png"},
                    new Make {Name = "Opel", LogotypeFileName = "opel.png"},
                    new Make {Name = "Ford", LogotypeFileName = "ford.png"},
                    new Make {Name = "Lexus", LogotypeFileName = "lexus.png"},
                    new Make {Name = "Volkswagen", LogotypeFileName = "Volkswagen.png"},
                    new Make {Name = "Porsche", LogotypeFileName = "Porsche.png"},
                    new Make {Name = "Mercedes-Benz", LogotypeFileName = "Mercedes-Benz.png"},
                    new Make {Name = "Audi", LogotypeFileName = "Audi.png"},
                    new Make {Name = "Toyota", LogotypeFileName = "Toyota.png"},
                };

                ctx.Make.AddRange(make);
                ctx.SaveChanges();
            }

            if (!ctx.Vehicles.Any())
            {


                string description = "Lorem Ipsum, yada yada. Lods of letters. Nada importento";
                var vehicles = new List<Vehicle>
                {
                   new Vehicle { RegistrationNumber = "RTW610", MakeId = 1, Model = "V70", ModelDescription = description, BodyType="Kombi", Colour="Röd", Horsepowers = 220, FacilityId = "BB1", FuelType = "Bensin", Gearbox = "Automatisk", Leasable = true, Mileage = 129, Used = false, Year = 2013, Price = 199999 },
                    new Vehicle { RegistrationNumber = "UYM207", MakeId = 2, Model = "9-3", ModelDescription = description, BodyType="Sedan", Colour="Svart", Horsepowers = 183, FacilityId = "BB2", FuelType = "Etanol/Bensin", Gearbox = "6-växlad manuel", Leasable = false, Mileage = 16240, Used = false, Year = 2007, Price = 62500 },
                    new Vehicle { RegistrationNumber = "GRT912", MakeId = 3, Model = "M3", ModelDescription = description, BodyType="Sedan", Colour="Vit", Horsepowers = 305, FacilityId = "BB2", FuelType = "Bensin", Gearbox = "5-växlad manuel", Leasable = true, Mileage = 3250, Used = true, Year = 2014, Price = 32000 },
                    new Vehicle { RegistrationNumber = "BVO184", MakeId = 3, Model = "330ci", ModelDescription = description, BodyType="Coupé", Colour="Silver", Horsepowers = 285, FacilityId = "BB3", FuelType = "Bensin", Gearbox = "5-växlad manuel", Leasable = false, Mileage = 30000, Used = true, Year = 1996, Price = 42000 },
                    new Vehicle { RegistrationNumber = "RTF824", MakeId = 10, Model = "A4", ModelDescription = description, BodyType="Sedan", Colour="Silver", Horsepowers = 170, FacilityId = "BB2", FuelType = "Bensin", Gearbox = "5-växlad manuel", Leasable = false, Mileage = 25000, Used = true, Year = 2003, Price = 31000 },
                    new Vehicle { RegistrationNumber = "PTT498", MakeId = 1, Model = "XC70", ModelDescription = description, BodyType="Suv", Colour="Vit", Horsepowers = 205, FacilityId = "BB1", FuelType = "Diesel", Gearbox = "Automatisk", Leasable = true, Mileage = 6223, Used = true, Year = 2016, Price = 280000 },
                    new Vehicle { RegistrationNumber = "GRS267", MakeId = 8, Model = "Cayenne", ModelDescription = description, BodyType="Suv", Colour="Svart", Horsepowers = 300, FacilityId = "BB3", FuelType = "Diesel", Gearbox = "6-växlad manuel", Leasable = true, Mileage = 13223, Used = true, Year = 2012, Price = 225000 },
                    new Vehicle { RegistrationNumber = "NKD972", MakeId = 10, Model = "RS6", ModelDescription = description, BodyType="Kombi", Colour="Grå", Horsepowers = 375, FacilityId = "BB2", FuelType = "Bensin", Gearbox = "Automatisk", Leasable = false, Mileage = 11000, Used = true, Year = 2012, Price = 180000 },
                    new Vehicle { RegistrationNumber = "FLS922", MakeId = 10, Model = "A2", ModelDescription = description, BodyType="Halvkombi", Colour="Silver", Horsepowers = 130, FacilityId = "BB1", FuelType = "Bensin", Gearbox = "5-växlad manuel", Leasable = false, Mileage = 13000, Used = true, Year = 2005, Price = 35000 },
                    new Vehicle { RegistrationNumber = "LVC884", MakeId = 11, Model = "Hilux", ModelDescription = description, BodyType="Pickup", Colour="Vit", Horsepowers = 150, FacilityId = "BB1", FuelType = "Diesel", Gearbox = "6-växlad manuel", Leasable = true, Mileage = 13000, Used = true, Year = 2015, Price = 220000 },
                    new Vehicle { RegistrationNumber = "HSK563", MakeId = 4, Model = "Astra K", ModelDescription = description, BodyType="Kombi", Colour="Röd", Horsepowers = 148, FacilityId = "BB2", FuelType = "Bensin", Gearbox = "5-växlad manuel", Leasable = true, Mileage = 34000, Used = true, Year = 2014, Price = 130000 },
                    new Vehicle { RegistrationNumber = "KSD342", MakeId = 2, Model = "9-5", ModelDescription = description, BodyType="Sedan", Colour="Svart", Horsepowers = 183, FacilityId = "BB2", FuelType = "Bensin", Gearbox = "5-växlad manuel", Leasable = false, Mileage = 44000, Used = true, Year = 2002, Price = 18000 },
                    new Vehicle { RegistrationNumber = "TWD702", MakeId = 6, Model = "Lexus RX", ModelDescription = description, BodyType="Suv", Colour="Svart", Horsepowers = 313, FacilityId = "BB2", FuelType = "El/Bensin", Gearbox = "5-växlad manuel", Leasable = false, Mileage = 14000, Used = true, Year = 2008, Price = 99000 },
                };

                ctx.Vehicles.AddRange(vehicles);
                ctx.SaveChanges();
            }
        }
    }
}