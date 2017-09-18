using System;
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
                };

                ctx.Make.AddRange(make);
                ctx.SaveChanges();
            }

            if (!ctx.Vehicles.Any())
            {


                string description = "Lorem Ipsum, yada yada. Lods of letters. Nada importento";
                var vehicles = new List<Vehicle>
                {
                    new Vehicle { RegistrationNumber = "ABC123", MakeId = 1, Model = "V70", ModelDescription = description, BodyType="Kombi", Colour="Röd", Horsepowers = 220, FacilityId = "BB1", FuelType = "Bensin", Gearbox = "Automatisk", Leasable = true, Mileage = 129, Used = false, Year = 2013, Price = 199999 },
                    new Vehicle { RegistrationNumber = "CMD084", MakeId = 2, Model = "9-3", ModelDescription = description, BodyType="Sedan", Colour="Svart", Horsepowers = 183, FacilityId = "BB2", FuelType = "Etanol/Bensin", Gearbox = "6-växlad manuel", Leasable = false, Mileage = 16240, Used = false, Year = 2007, Price = 62500 },
                };

                ctx.Vehicles.AddRange(vehicles);
                ctx.SaveChanges();
            }
        }
    }
}
