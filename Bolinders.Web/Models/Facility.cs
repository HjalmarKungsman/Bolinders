﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Web.Models
{
    public class Facility
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}