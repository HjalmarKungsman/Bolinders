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

namespace Bolinders.Core.Controllers
{
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
        public async Task<IActionResult> List(bool? used = null, VehicleSearchModel formData = null, int page = 1, int pageLimit = 2)
        {
            var toSkip = (page - 1) * pageLimit;

            IQueryable<Vehicle> itemsAll;
            if (used != null)
            {
                if (used == true)
                {
                    itemsAll = _context.Vehicles.OrderBy(x => x.Id).Where(x => x.Used == true).AsQueryable();
                }
                else
                {
                    itemsAll = _context.Vehicles.OrderBy(x => x.Id).Where(x => x.Used == false).AsQueryable();
                }
            }
            else
            {
                itemsAll = _context.Vehicles.OrderBy(x => x.Id).AsQueryable();
            }
            
            var result = SearchHelpers.SearchVehicles(itemsAll, formData);

            var itemsOnThisPage = result.Skip(toSkip).Take(pageLimit).Include(v => v.Facility).Include(v => v.Make);
            var itemsFinal = await itemsOnThisPage.ToListAsync();

            var paging = new PagingInfo { CurrentPage = page, ItemsPerPage = pageLimit, TotalItems = result.Count() };
            var vm = new VehicleListViewModel { Vehicles = itemsFinal, Pager = paging };

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
        public async Task<IActionResult> Details(Guid? id)
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

                var listOfImages = await ImageUploadHelper.UploadImages(vehicle.Images, _environment);
                   
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

                newVehicle = ImageUploadHelper.ImageBuilder(listOfImages, newVehicle);

                if (vehicle.Equipment.Any())
                {
                    newVehicle = EquipmentHelper.EquipmentBuilder(vehicle.Equipment, newVehicle);                  
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

            var vehicle = await _context.Vehicles.SingleOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Id", vehicle.FacilityId);
            ViewData["MakeId"] = new SelectList(_context.Make, "Id", "Id", vehicle.MakeId);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,RegistrationNumber,MakeId,Model,ModelDescription,Year,Mileage,Price,BodyType,Colour,Gearbox,FuelType,Horsepowers,FacilityId,Used,Leasable,Created,Updated")] Vehicle vehicle)
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
                    _context.Entry(vehicle).Property(x => x.UrlId).IsModified = false;                  
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

        [HttpPost]
        public async Task<IActionResult> UploadImages(List<IFormFile> files)
        {
            if (files.Any())
            {
                var uploads = Path.Combine(_environment.WebRootPath, "images/uploads");
                List<string> filenames = new List<string>();
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            fileStream.Close();
                        }
                    }
                    filenames.Add(file.FileName);
                }

                return Ok(filenames);
            }

            return Ok("Add an item");
        }

        private bool VehicleExists(Guid id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
    }
}
