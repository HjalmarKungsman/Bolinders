﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bolinders.Core.Enums
{
    public enum BodyType
    {
        [Display(Name = "Sedan")]
        B1,
        [Display(Name = "Kombi")]
        B2,
        [Display(Name = "Halvkombi")]
        B3,
        [Display(Name = "Sportkupé")]
        B4,
        [Display(Name = "SUV")]
        B5,
        [Display(Name = "Cab")]
        B6,
        [Display(Name = "Minibuss")]
        B7,
        [Display(Name = "Övrigt")]
        B8,
        [Display(Name = "Transportbil")]
        B9,
        [Display(Name = "Småbil")]
        B10,
        [Display(Name = "Familjebuss")]
        B11,
        [Display(Name = "Yrkesfordon")]
        B12
    }
}
