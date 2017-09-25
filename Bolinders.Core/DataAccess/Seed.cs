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
                    new Make {Name = "Volkswagen", LogotypeFileName = "volkswagen.png"},
                    new Make {Name = "Porsche", LogotypeFileName = "porsche.png"},
                    new Make {Name = "Mercedes-Benz", LogotypeFileName = "mercedes-benz.png"},
                    new Make {Name = "Audi", LogotypeFileName = "audi.png"},
                    new Make {Name = "Toyota", LogotypeFileName = "toyota.png"},
                    new Make {Name = "Tesla", LogotypeFileName = "tesla.png"},
                    new Make {Name = "Big", LogotypeFileName = "big.png"},
                    new Make {Name = "Kia", LogotypeFileName = "kia.png"},
                    new Make {Name = "Seat", LogotypeFileName = "seat.png"},
                    new Make {Name = "Škoda", LogotypeFileName = "skoda.png"},
                    new Make {Name = "Suzuki", LogotypeFileName = "suzuki.png"},
                    new Make {Name = "Viper", LogotypeFileName = "viper.png"},
                    new Make {Name = "Maserati", LogotypeFileName = "maserati.png"},
                    new Make {Name = "Mazda", LogotypeFileName = "mazda.png"},
                    new Make {Name = "MG", LogotypeFileName = "mg.png"},
                    new Make {Name = "Mini", LogotypeFileName = "mini.png"},
                    new Make {Name = "Mitsubishi", LogotypeFileName = "mitsubishi.png"},
                    new Make {Name = "Mustang", LogotypeFileName = "mustang.png"},
                    new Make {Name = "Nissan", LogotypeFileName = "nissan.png"},
                    new Make {Name = "Peugeot", LogotypeFileName = "peugeot.png"},
                    new Make {Name = "Pontiac", LogotypeFileName = "pontiac.png"},
                    new Make {Name = "Renault", LogotypeFileName = "Renault.png"},
                    new Make {Name = "Rolls-royce", LogotypeFileName = "Rolls-royce.png"},
                    new Make {Name = "Land-Rover", LogotypeFileName = "land-rover.png"},
                    new Make {Name = "Lancia", LogotypeFileName = "lancia.png"},
                    new Make {Name = "Lamborghini", LogotypeFileName = "lamborghini.png"},
                    new Make {Name = "Koenigsegg", LogotypeFileName = "koenigsegg.png"}, 
                    new Make {Name = "Jeep", LogotypeFileName = "jeep.png"},
                    new Make {Name = "Isuzu", LogotypeFileName = "isuzu.png"},
                    new Make {Name = "Infiniti", LogotypeFileName = "infiniti.png"},
                    new Make {Name = "Hyundai", LogotypeFileName = "hyundai.png"},
                    new Make {Name = "Hummer", LogotypeFileName = "hummer.png"},
                    new Make {Name = "Honda", LogotypeFileName = "honda.png"},
                    new Make {Name = "General-Motors", LogotypeFileName = "general-motors.png"},
                    new Make {Name = "Geely", LogotypeFileName = "geely.png"},
                    new Make {Name = "Fiat", LogotypeFileName = "fiat.png"},
                    new Make {Name = "Acura", LogotypeFileName = "acura.png"},
                    new Make {Name = "Alfa-Romeo", LogotypeFileName = "alfa-romeo.png"},
                    new Make {Name = "Austin", LogotypeFileName = "austin.png"},
                    new Make {Name = "Bentley", LogotypeFileName = "bentley.png"},
                    new Make {Name = "Buick", LogotypeFileName = "buick.png"},
                    new Make {Name = "Chrysler", LogotypeFileName = "chrysler.png"},
                    new Make {Name = "Citroen", LogotypeFileName = "citroen.png"},
                    new Make {Name = "Corvette", LogotypeFileName = "corvette.png"},
                    new Make {Name = "Dacia", LogotypeFileName = "dacia.png"},
                    new Make {Name = "Datsun", LogotypeFileName = "datsun.png"},
                    new Make {Name = "Dodge", LogotypeFileName = "dodge.png"},
                    new Make {Name = "Ferrari", LogotypeFileName = "ferrari.png"}
                };

                ctx.Make.AddRange(make);
                ctx.SaveChanges();
            }

            Guid carId = Guid.NewGuid();
            var created = DateTime.UtcNow;
            var i = 1;

            if (!ctx.Vehicles.Any())
            {

                var vehicles = new List<Vehicle>
                {
                    new Vehicle { RegistrationNumber = "RTW610", MakeId = 1, Model = "V70", ModelDescription = "D1", BodyType = BodyType.B1, Colour="Röd", Horsepowers = 220, FacilityId = "BB1", FuelType = FuelType.F1, Gearbox = Gearbox.G1, Leasable = true, Mileage = 129, Used = false, Year = 2013, Price = 199999, Created = created.AddDays(-1), Updated = created.AddDays(-1)},
                    new Vehicle { Id = carId, RegistrationNumber = "UYM207", MakeId = 29, Model = "9-3", ModelDescription = "2.0 Turbo", BodyType = BodyType.B2, Colour="Svart", Horsepowers = 183, FacilityId = "BB2", FuelType = FuelType.F2, Gearbox = Gearbox.G1, Leasable = false, Mileage = 16240, Used = false, Year = 2007, Price = 62500, Created = created.AddDays(-2), Updated = created.AddDays(-2) },
                    new Vehicle { RegistrationNumber = "GRT912", MakeId = 30, Model = "M3", ModelDescription = "C2", BodyType = BodyType.B3, Colour="Vit", Horsepowers = 305, FacilityId = "BB2", FuelType = FuelType.F3, Gearbox = Gearbox.G1, Leasable = true, Mileage = 3250, Used = true, Year = 2014, Price = 32000, Created = created.AddDays(-3), Updated = created.AddDays(-3) },
                    new Vehicle { RegistrationNumber = "BVO184", MakeId = 30, Model = "330ci", ModelDescription = "SE", BodyType = BodyType.B4, Colour="Silver", Horsepowers = 285, FacilityId = "BB3", FuelType = FuelType.F4, Gearbox = Gearbox.G1, Leasable = false, Mileage = 30000, Used = true, Year = 1996, Price = 42000, Created = created.AddDays(-4), Updated = created.AddDays(-4) },
                    new Vehicle { RegistrationNumber = "RTF824", MakeId = 37, Model = "A4", ModelDescription = "Mark II", BodyType = BodyType.B5, Colour="Silver", Horsepowers = 170, FacilityId = "BB2", FuelType = FuelType.F5, Gearbox = Gearbox.G1, Leasable = false, Mileage = 25000, Used = true, Year = 2003, Price = 31000, Created = created.AddDays(-5), Updated = created.AddDays(-5) },
                    new Vehicle { RegistrationNumber = "PTT498", MakeId = 1, Model = "XC70", ModelDescription = "A3", BodyType = BodyType.B6, Colour="Vit", Horsepowers = 205, FacilityId = "BB1", FuelType = FuelType.F6, Gearbox = Gearbox.G2, Leasable = true, Mileage = 6223, Used = true, Year = 2016, Price = 280000, Created = created.AddDays(-6), Updated = created.AddDays(-6) },
                    new Vehicle { RegistrationNumber = "GRS267", MakeId = 35, Model = "Cayenne", ModelDescription = "SE", BodyType = BodyType.B7, Colour="Svart", Horsepowers = 300, FacilityId = "BB3", FuelType = FuelType.F7, Gearbox = Gearbox.G1, Leasable = true, Mileage = 13223, Used = true, Year = 2012, Price = 225000, Created = created.AddDays(-7), Updated = created.AddDays(7) },
                    new Vehicle { RegistrationNumber = "NKD972", MakeId = 37, Model = "RS6", ModelDescription = "Bison X", BodyType = BodyType.B8, Colour="Grå", Horsepowers = 375, FacilityId = "BB2", FuelType = FuelType.F8, Gearbox = Gearbox.G2, Leasable = false, Mileage = 11000, Used = true, Year = 2012, Price = 180000, Created = created.AddDays(-8), Updated = created.AddDays(-8) },
                    new Vehicle { RegistrationNumber = "FLS922", MakeId = 37, Model = "A2", ModelDescription = "D1", BodyType = BodyType.B9, Colour="Silver", Horsepowers = 130, FacilityId = "BB1", FuelType = FuelType.F1, Gearbox = Gearbox.G1, Leasable = false, Mileage = 13000, Used = true, Year = 2005, Price = 35000, Created = created.AddDays(-9), Updated = created.AddDays(-9) },
                    new Vehicle { RegistrationNumber = "LVC884", MakeId = 38, Model = "Hilux", ModelDescription = "1.2", BodyType = BodyType.B1, Colour="Vit", Horsepowers = 150, FacilityId = "BB1", FuelType = FuelType.F1, Gearbox = Gearbox.G1, Leasable = true, Mileage = 13000, Used = true, Year = 2015, Price = 220000, Created = created.AddDays(-10), Updated = created.AddDays(-10) },
                    new Vehicle { RegistrationNumber = "HSK563", MakeId = 31, Model = "Astra K", ModelDescription = "3.2", BodyType = BodyType.B1, Colour="Röd", Horsepowers = 148, FacilityId = "BB2", FuelType = FuelType.F1, Gearbox = Gearbox.G1, Leasable = true, Mileage = 34000, Used = true, Year = 2014, Price = 130000, Created = created.AddDays(-11), Updated = created.AddDays(-11) },
                    new Vehicle { RegistrationNumber = "KSD342", MakeId = 29, Model = "9-5", ModelDescription = "2.2 Turbo", BodyType = BodyType.B1, Colour="Svart", Horsepowers = 183, FacilityId = "BB2", FuelType = FuelType.F1, Gearbox = Gearbox.G1, Leasable = false, Mileage = 44000, Used = true, Year = 2002, Price = 18000, Created = created.AddDays(-12), Updated = created.AddDays(-12) },
                    new Vehicle { RegistrationNumber = "TWD702", MakeId = 33, Model = "Lexus RX", ModelDescription = "1.4", BodyType = BodyType.B1, Colour="Svart", Horsepowers = 313, FacilityId = "BB2", FuelType = FuelType.F1, Gearbox = Gearbox.G2, Leasable = false, Mileage = 14000, Used = true, Year = 2008, Price = 99000, Created = created.AddDays(-13), Updated = created.AddDays(-13) },
                    new Vehicle { RegistrationNumber = "APA007", MakeId = 5, Model = "Agera S", ModelDescription = "5.0L V8, dual Turbo", BodyType = BodyType.B8, Colour="Mörkgrön", Horsepowers = 1341, FacilityId = "BB1", FuelType = FuelType.F1, Gearbox = Gearbox.G2, Leasable = false, Mileage = 3600, Used = false, Year = 2013, Price = 4999000, Created = created.AddDays(6), Updated = created.AddDays(1) },
                    new Vehicle { RegistrationNumber = "APA007", MakeId = 41, Model = "Bobby Car", ModelDescription = "Dual-leg Kicker", BodyType = BodyType.B8, Colour="Röd", Horsepowers = 1, FacilityId = "BB3", FuelType = FuelType.F9, Gearbox = Gearbox.G1, Leasable = true, Mileage = 1, Used = true, Year = 1998, Price = 600, Created = created.AddDays(2), Updated = created.AddDays(5) },
                };

                ctx.Vehicles.AddRange(vehicles);
                ctx.SaveChanges();
            }

            if (!ctx.Images.Any())
            {

                var images = new List<Image>
                {
                   new Image("car-image-1.png", carId),
                   new Image("car-image-2.png", carId),
                   new Image("car-image-3.png", carId),
                   new Image("car-image-4.png", carId)
                };

                ctx.Images.AddRange(images);
                ctx.SaveChanges();
            }
        }
    }
}
