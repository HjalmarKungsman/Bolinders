using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bolinders.Web.Models;

namespace Bolinders.Web.DataAccess
{
    public static class Seed
    {
        internal static void FillIfEmpty(ApplicationDbContext ctx)
        {
            if (!ctx.Facilites.Any())
            {
                ctx.Facilites.Add(new Facility { Id = "BB1", Name = "Jönköping", Address = "Gata 1", ZipCode="12345", City = "Jönköping", Email = "jonkoping@bolinder.se", PhoneNumber = "036-102030", Longitude = "54,12345", Latitude = "10,12345" });
                ctx.Facilites.Add(new Facility { Id = "BB2", Name = "Värnamo", Address = "Gata 1", ZipCode = "12345", City = "Jönköping", Email = "jonkoping@bolinder.se", PhoneNumber = "036-102030", Longitude = "54,12345", Latitude = "10,12345" });
                ctx.Facilites.Add(new Facility { Id = "BB3", Name = "Göteborg", Address = "Gata 1", ZipCode = "12345", City = "Jönköping", Email = "jonkoping@bolinder.se", PhoneNumber = "036-102030", Longitude = "54,12345", Latitude = "10,12345" });
            }

            if (!ctx.Make.Any())
            {
                var make = new List<Make>
                {
                    new Make {Name = "Volvo", FileName = "volvo.png"},
                    new Make {Name = "SAAB", FileName = "saab.png"},
                    new Make {Name = "BMW", FileName = "bmw.png"},
                    new Make {Name = "Opel", FileName = "opel.png"},
                    new Make {Name = "Ford", FileName = "ford.png"},
                    new Make {Name = "Lexus", FileName = "lexus.png"},
                };

                ctx.Make.AddRange(make);
                ctx.SaveChanges();
            }

            if (!ctx.Vehicles.Any())
            {
                var vehicles = new List<Vehicle>
                {
                    new Vehicle { RegistrationNumber = "ABC123", MakeId = 1, Model = "V70", ModelDescription = "Lorem Ipsum", ChassiType="Kombi", Colour="Röd", Horsepowers = 220, FacilityId = "BB1", FuelType = "Bensin", Gearbox = "Automatisk", Leasable = true, Milage = 129, Used = false, Year = 2013, Price = 199999 },
                    new Vehicle { RegistrationNumber = "CMD084", MakeId = 2, Model = "9-3", ModelDescription = "Lorem Ipsum", ChassiType="Sedan", Colour="Svart", Horsepowers = 183, FacilityId = "BB2", FuelType = "Etanol/Bensin", Gearbox = "6-växlad manuel", Leasable = false, Milage = 16240, Used = false, Year = 2007, Price = 62500 },
                };

                ctx.Vehicles.AddRange(vehicles);
                ctx.SaveChanges();
            }
        }
    }
}
