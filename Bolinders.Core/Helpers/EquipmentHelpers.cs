﻿using Bolinders.Core.Models;
using Bolinders.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Core.Helpers
{
    public class EquipmentHelpers
    {
        public static Vehicle EquipmentBuilder(List<string> listOfEquipment, Vehicle vehicle)
        {
            for (int i = 0; i < listOfEquipment.Count; i++)
            {
                Equipment newEquipment = new Equipment
                {
                    Id = Guid.NewGuid(),
                    VehicledId = vehicle.Id,
                    Value = listOfEquipment[i]
                };
                vehicle.Equipment.Add(newEquipment);
            }

            return vehicle;
        }
    }
}