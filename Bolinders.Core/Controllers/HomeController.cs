using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bolinders.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using Bolinders.Core.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bolinders.Core.Services;
using System.Net.Mail;
using Bolinders.Core.Models.Entities;
using Bolinders.Core.Models;
using Microsoft.Extensions.Options;
using Bolinders.Core.Models.SettingModels;

namespace Bolinders.Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyNewsDeskSettings _myNewsDeskSettings;
        private readonly ApplicationDbContext _context;
        private IEmailSenderService _emailSender;

        public HomeController(ApplicationDbContext context, IEmailSenderService emailSender, IOptions<MyNewsDeskSettings> myNewsDeskSettings)
        {
            _context = context;
            _emailSender = emailSender;
            _myNewsDeskSettings = myNewsDeskSettings.Value;
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
            ViewBag.MyNewsDeskSettings = _myNewsDeskSettings;

            return View();
        }

        public IActionResult Contact()
        {
            var contacts = _context.Facilities.ToList();
            ViewBag.Contacts = contacts;

            ViewData["Facility"] = new SelectList(_context.Facilities, "Email", "Email");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactformViewModel form)
        {
            var contacts = _context.Facilities.ToList();
            ViewBag.Contacts = contacts;
            ViewData["Facility"] = new SelectList(_context.Facilities, "Email", "Email");


            var emailSender = await _emailSender.SendEmailToFacility(form.SenderName, form.SenderEmail, form.Reciever, form.Subject, form.Message, form.PhoneNumber);

            if (emailSender == SmtpStatusCode.Ok)
            {
                ViewBag.Success = "Ditt meddelande har skickats.";
            }
            else
            {
                ViewBag.Fail = "Ett fel har uppstått se över dina uppgifter och testa igen.";
            }
            

            return View();
        }

        public IActionResult Errors(string Id)
        {
            switch (Id)
            {
                case "404":
                case "500":
                    return View($"~/Views/Errors/Page{Id}.cshtml");
                default: return DefaultError();
            }

        }

        public IActionResult DefaultError()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
