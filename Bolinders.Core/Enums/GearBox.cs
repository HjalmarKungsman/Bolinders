using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bolinders.Core.Enums
{
    public enum Gearbox
    {
        [Display(Name = "Manuell")]
        G1,
        [Display(Name = "Automatisk")]
        G2
    }
}
