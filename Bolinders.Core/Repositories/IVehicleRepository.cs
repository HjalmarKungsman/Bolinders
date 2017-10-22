using Bolinders.Core.Models;
using Bolinders.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Bolinders.Core.Repositories
{
    public interface IVehicleRepository
    {
        IEnumerable<Vehicle> Vehicles { get; }
    }
}
