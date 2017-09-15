using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bolinders.Web.Repositories;
using Bolinders.Web.Models;
using Bolinders.Web.ViewModels;

namespace Bolinders.Web.Controllers
{
    public class VehicleController : Controller
    {
        private IVehicleRepository repo;
        public int PageLimit = 1;
        public VehicleController(IVehicleRepository productRepository)
        {
            repo = productRepository;
        }
        public IActionResult List(int page = 1)
        {
            var toSkip = (page - 1) * PageLimit;

            var products = repo.Vehicles.OrderBy(x => x.Id).Skip(toSkip).Take(PageLimit);
            var paging = new PagingInfo { CurrentPage = page, ItemsPerPage = PageLimit, TotalItems = repo.Vehicles.Count() };
            var vm = new VehicleListViewModel { Vehicles = products, Pager = paging };
            return View("~/Views/Vehicle/List.cshtml", vm);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}