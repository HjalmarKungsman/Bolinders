using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bolinders.Core.Models;
using Bolinders.Core.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Bolinders.Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var logos = _context.Make
                     .Include(c => c.Vehicles).ToList();

            foreach (Make s in logos.ToList())
            {
                if(s.Vehicles.Count < 1)
                {
                    logos.Remove(s);
                }
            }

            ViewBag.Makes = logos;

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            var contacts = _context.Facilities.ToList();
            ViewBag.Contacts = contacts;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
