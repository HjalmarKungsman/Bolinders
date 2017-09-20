using Bolinders.Core.Models;
using Bolinders.Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolinders.Core.Helpers
{
    public static class SearchHelpers
    {
        public static IQueryable<Vehicle> SearchVehicles(IQueryable<Vehicle> vehicles, VehicleSearchModel formData)
        {

            var result = vehicles;
            if (formData != null)
            {
                if (!string.IsNullOrEmpty(formData.SearchText))
                    // 3 different ways to search
                    // See: https://stackoverflow.com/questions/16993962/searching-multiple-fields-with-linq-contains-or-other
                    //result = result.Where(x => x.Make.Name.Contains(searchModel.SearchText));
                    //result = result.Where(x => new[] { x.Make.Name, x.Model, x.ModelDescription }.Any(s => s.Contains(searchModel.SearchText)));
                    result = result.Where(x =>
                        x.Make.Name.Contains(formData.SearchText) ||
                        x.Model.Contains(formData.SearchText) ||
                        x.ModelDescription.Contains(formData.SearchText));
                if (formData.PriceFrom.HasValue)
                    result = result.Where(x => x.Price >= formData.PriceFrom);
                if (formData.PriceTo.HasValue)
                    result = result.Where(x => x.Price <= formData.PriceTo);
                if (formData.MileageFrom.HasValue)
                    result = result.Where(x => x.Mileage >= formData.MileageFrom);
                if (formData.MileageTo.HasValue)
                    result = result.Where(x => x.Mileage <= formData.MileageTo);
                if (formData.YearFrom.HasValue)
                    result = result.Where(x => x.Year >= formData.YearFrom);
                if (formData.YearTo.HasValue)
                    result = result.Where(x => x.Year <= formData.YearTo);
                if (!string.IsNullOrEmpty(formData.BodyType))
                    result = result.Where(x => x.BodyType.Contains(formData.BodyType));
                if (!string.IsNullOrEmpty(formData.Gearbox))
                    result = result.Where(x => x.Gearbox.Contains(formData.Gearbox));
            }

            return result;
        }
    }
}
