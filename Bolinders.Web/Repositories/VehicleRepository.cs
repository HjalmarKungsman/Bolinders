using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bolinders.Web.DataAccess;
using Bolinders.Web.Models;

namespace Bolinders.Web.Repositories
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
