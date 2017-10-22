using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bolinders.Core.DataAccess;
using Bolinders.Core.Models;
using Bolinders.Core.Models.Entities;

namespace Bolinders.Core.Repositories
{
      public class VehicleRepository : IVehicleRepository 
    {
        public ApplicationDbContext ctx;
        public VehicleRepository(ApplicationDbContext context)
        {
            ctx = context;
        }
        public IEnumerable<Vehicle> Vehicles => ctx.Vehicles;
    }
}
