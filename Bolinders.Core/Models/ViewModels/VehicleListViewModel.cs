using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bolinders.Core.Models;
using Bolinders.Core.Models.ViewModels;

namespace Bolinders.Core.ViewModels
{
    public class VehicleListViewModel
    {
        public IEnumerable<Vehicle> Vehicles { get; set; }
        public PagingInfo Pager { get; set; }
        public VehicleSearchModel SeachModel { get; set; }
    }
}
