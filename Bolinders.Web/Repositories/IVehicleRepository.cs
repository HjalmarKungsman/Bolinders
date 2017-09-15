using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bolinders.Web.Models;

namespace Bolinders.Web.Repositories
{
    public interface IVehicleRepository
    {
        IEnumerable<Vehicle> Vehicles { get; }
    }
}
