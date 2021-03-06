﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using Bolinders.Core.Models.Entities;

namespace Bolinders.Core.Models.Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [ForeignKey("Facilities")]
        public string FacilityId { get; set; }
        public virtual Facility Facility { get; set; }
    }
}
