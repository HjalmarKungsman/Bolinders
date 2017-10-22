using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bolinders.Core.Models.ViewModels
{
    public class ContactformViewModel
    {
        [Required]
        [Display(Name = "Ditt namn (Obligatorisk)")]
        public string SenderName { get; set; }
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Din E-post (Obligatorisk)")]
        public string SenderEmail { get; set; }
        [Display(Name = "Mottagare")]
        [Required]
        public string Reciever { get; set; }
        [Display(Name = "Ämne (Obligatorisk)")]
        [Required]
        public string Subject { get; set; }
        [Required]
        [Display(Name = "Meddelande (Obligatorisk)")]
        public string Message { get; set; }
        

    }
}