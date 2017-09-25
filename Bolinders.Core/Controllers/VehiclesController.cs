using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bolinders.Core.Models;
using Bolinders.Core.DataAccess;
using Microsoft.AspNetCore.Http;
using Bolinders.Core.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Bolinders.Core.Helpers;
using Bolinders.Core.Models.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Bolinders.Core.Controllers
{
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _environment;

        public VehiclesController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        //GET: Vehicles/List
        [AllowAnonymous]
        public async Task<IActionResult> List(VehicleSearchModel formData = null, int page = 1, int pageLimit = 8)
        {
            var toSkip = (page - 1) * pageLimit;

            var test = formData.PriceFrom;

            var result = _context.Vehicles
                .OrderByDescending(x => x.Created > x.Updated ? x.Created : x.Updated)
                .Where(y => formData.SearchText == null || 
                    y.Make.Name.Contains(formData.SearchText) || 
                    y.Model.Contains(formData.SearchText) ||
                    y.ModelDescription.Contains(formData.SearchText))
                .Where(y => formData.Used == null || y.Used.Equals(formData.Used))
                .Where(z => formData.PriceFrom == null || z.Price >= formData.PriceFrom)
                .Where(z => formData.PriceTo == null || z.Price <= formData.PriceTo)
                .Where(z => formData.YearFrom == null || z.Year >= formData.YearFrom)
                .Where(z => formData.YearTo == null || z.Year <= formData.YearTo)
                .Where(z => formData.MileageFrom == null || z.Mileage >= formData.MileageFrom)
                .Where(z => formData.MileageTo == null || z.Mileage <= formData.MileageTo)
                .Where(z => formData.Make == null || z.MakeId.Equals(formData.Make))
                .Where(z => formData.BodyType == null || z.BodyType.Equals((BodyType)Enum.Parse(typeof(BodyType), formData.BodyType)))
                .Where(z => formData.Gearbox == null || z.Gearbox.Equals((Gearbox)Enum.Parse(typeof(Gearbox), formData.Gearbox)))
                .Where(z => formData.FuelType == null || z.FuelType.Equals((FuelType)Enum.Parse(typeof(FuelType), formData.FuelType)))
                .Skip(toSkip)
                .Take(pageLimit)
                .Include(v => v.Make)
                .AsQueryable();

            var countResult = _context.Vehicles
                .Where(y => formData.SearchText == null ||
                    y.Make.Name.Contains(formData.SearchText) ||
                    y.Model.Contains(formData.SearchText) ||
                    y.ModelDescription.Contains(formData.SearchText))
                .Where(y => formData.Used == null || y.Used.Equals(formData.Used))
                .Where(z => formData.PriceFrom == null || z.Price >= formData.PriceFrom)
                .Where(z => formData.PriceTo == null || z.Price <= formData.PriceTo)
                .Where(z => formData.YearFrom == null || z.Year >= formData.YearFrom)
                .Where(z => formData.YearTo == null || z.Year <= formData.YearTo)
                .Where(z => formData.MileageFrom == null || z.Mileage >= formData.MileageFrom)
                .Where(z => formData.MileageTo == null || z.Mileage >= formData.MileageTo)
                .Where(z => formData.Make == null || z.MakeId.Equals(formData.Make))
                .Where(z => formData.BodyType == null || z.BodyType.Equals((BodyType)Enum.Parse(typeof(BodyType), formData.BodyType)))
                .Where(z => formData.Gearbox == null || z.Gearbox.Equals((Gearbox)Enum.Parse(typeof(Gearbox), formData.Gearbox)))
                .Where(z => formData.FuelType == null || z.FuelType.Equals((FuelType)Enum.Parse(typeof(FuelType), formData.FuelType)))
                .Count();

            var itemsFinal = await result.ToListAsync();

            var paging = new PagingInfo { CurrentPage = page, ItemsPerPage = pageLimit, TotalItems = countResult };
            var vm = new VehicleListViewModel { Vehicles = itemsFinal, Pager = paging, SeachModel = formData };

            ViewBag.SearchText = formData.SearchText;
            ViewBag.PriceFrom = formData.PriceFrom;
            ViewBag.PriceTo = formData.PriceTo;
            ViewBag.MileageFrom = formData.MileageFrom;
            ViewBag.MileageTo = formData.MileageTo;
            ViewBag.YearFrom = formData.YearFrom;
            ViewBag.YearTo = formData.YearTo;
            ViewBag.BodyType = formData.BodyType;
            ViewBag.Gearbox = formData.Gearbox;
            return View("~/Views/Vehicles/List.cshtml", vm);
        }

        private object VehicleSearchHelper()
        {
            throw new NotImplementedException();
        }

        //GET: Vehicles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Vehicles.Include(v => v.Facility).Include(v => v.Make);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Vehicles/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id, string vehicleLinkId = null)
        {

            if (id == null && vehicleLinkId == null)
            {
                return NotFound();
            }
            else if (id != null)
            { 
                var vehicle = await _context.Vehicles
                    .Include(v => v.Facility)
                    .Include(v => v.Make)
                    .SingleOrDefaultAsync(m => m.Id == id);
                if (vehicle == null)
                {
                    return NotFound();
                }

                return View(vehicle);
            }
            else if (vehicleLinkId != null)
            {
                var lastSign = vehicleLinkId.LastIndexOf("-");
                int vehicleId = Int32.Parse(vehicleLinkId.Substring(lastSign + 1));


                var vehicle = await _context.Vehicles
                    .Include(v => v.Facility)
                    .Include(v => v.Make)
                    .SingleOrDefaultAsync(m => m.UrlId == vehicleId);
                if (vehicle == null)
                {
                    return NotFound();
                }

                return View(vehicle);
            }
            else
            {
                return NotFound();
            }


        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name");
            ViewData["MakeId"] = new SelectList(_context.Make, "Id", "Name");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RegistrationNumber,MakeId,Model,ModelDescription,Year,Mileage,Price,BodyType,Colour,Gearbox,FuelType,Horsepowers,Used,FacilityId,Leasable,Created,Updated,Images,Equipment")] VehicleViewModel vehicle)
        {
            if (ModelState.IsValid)
            {

                var listOfImages = await ImageHelpers.UploadImages(vehicle.Images, _environment);
                   
                Vehicle newVehicle = new Vehicle {
                    Id = Guid.NewGuid(),
                    RegistrationNumber = vehicle.RegistrationNumber,
                    BodyType = vehicle.BodyType,
                    Colour = vehicle.Colour,
                    Created = DateTime.UtcNow,
                    Facility = vehicle.Facility,
                    FacilityId = vehicle.FacilityId,
                    FuelType = vehicle.FuelType,
                    Gearbox = vehicle.Gearbox,
                    Horsepowers = vehicle.Horsepowers,
                    Leasable = vehicle.Leasable,
                    Make = vehicle.Make,
                    MakeId = vehicle.MakeId,
                    Mileage = vehicle.Mileage,
                    Model = vehicle.Model,
                    ModelDescription = vehicle.ModelDescription,
                    Price = vehicle.Price,
                    Updated = DateTime.UtcNow,
                    Used = vehicle.Used,
                    Year = vehicle.Year,
                    Images = new List<Image>(),
                    Equipment = new List<Equipment>()
                };

                newVehicle = ImageHelpers.ImageBuilder(listOfImages, newVehicle);

                if (vehicle.Equipment != null)
                {
                    newVehicle = EquipmentHelpers.EquipmentBuilder(vehicle.Equipment, newVehicle);                  
                }

                _context.Add(newVehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name", vehicle.FacilityId);
            ViewData["MakeId"] = new SelectList(_context.Make, "Id", "Name", vehicle.MakeId);
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.Include(x => x.Images).SingleOrDefaultAsync(m => m.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            VehicleEditModel VehicleEditing = new VehicleEditModel
            {
                RegistrationNumber = vehicle.RegistrationNumber,
                Make = vehicle.Make,
                MakeId = vehicle.MakeId,
                Model = vehicle.Model,
                ModelDescription = vehicle.ModelDescription,
                Year = vehicle.Year,
                Mileage = vehicle.Mileage,
                Price = vehicle.Price,
                BodyType = vehicle.BodyType,
                Colour = vehicle.Colour,
                Gearbox = vehicle.Gearbox,
                FuelType = vehicle.FuelType,
                Horsepowers = vehicle.Horsepowers,
                Used = vehicle.Used,
                FacilityId = vehicle.FacilityId,
                Facility = vehicle.Facility,
                ImageList = vehicle.Images.ToList(),
                Leasable = vehicle.Leasable,
                Created = vehicle.Created,
                Updated = vehicle.Updated,
                Equipment = vehicle.Equipment
            };
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name");
            ViewData["MakeId"] = new SelectList(_context.Make, "Id", "Name");

            return View(VehicleEditing);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,RegistrationNumber,MakeId,Model,ModelDescription,Year,Mileage,Price,BodyType,Colour,Gearbox,FuelType,Horsepowers,FacilityId,ImageList,Images,Used,Leasable,Created,Updated,EquipmentString")] VehicleEditModel vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {

                try
                {
                    vehicle.Updated = DateTime.UtcNow;
                    _context.Entry(vehicle).State = EntityState.Modified;               
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Id", vehicle.FacilityId);
            ViewData["MakeId"] = new SelectList(_context.Make, "Id", "Id", vehicle.MakeId);
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Facility)
                .Include(v => v.Make)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<Guid> selectedVehicles)
        {
            if (selectedVehicles == null)
            {
                return NotFound();
            }
            foreach(var i in selectedVehicles)
            {
                var vehicle = await _context.Vehicles.SingleOrDefaultAsync(m => m.Id == i);
                _context.Vehicles.Remove(vehicle);             
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var vehicle = await _context.Vehicles.SingleOrDefaultAsync(m => m.Id == id);
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(Guid id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
    }
}
