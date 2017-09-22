using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Core.Models.ViewModels
{
    public class VehicleSearchModel
    {
        public bool? Used { get; set; }
        public string SearchText { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public int? BodyType { get; set; }
        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }
        public int? MileageFrom { get; set; }
        public int? MileageTo { get; set; }
        public string Gearbox { get; set; }
    }
}