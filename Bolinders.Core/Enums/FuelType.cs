using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bolinders.Core.Enums
{
    public enum FuelType
    {
        [Display(Name = "Bensin")]
        F1,
        [Display(Name = "Bensin/el")]
        F2,
        [Display(Name = "Bensin/etanol")]
        F3,
        [Display(Name = "Bensin/gas")]
        F4,
        [Display(Name = "Diesel")]
        F5,
        [Display(Name = "Diesel/el")]
        F6,
        [Display(Name = "El")]
        F7,
        [Display(Name = "Gas")]
        F8,
        [Display(Name = "Övrigt")]
        F9
    }
}
