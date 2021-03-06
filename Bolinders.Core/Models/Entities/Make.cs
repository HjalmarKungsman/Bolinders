﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Core.Models.Entities
{
    public class Make
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogotypeFileName { get; set; }

        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
