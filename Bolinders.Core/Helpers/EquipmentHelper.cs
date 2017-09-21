using Bolinders.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bolinders.Core.Helpers
{
    public class EquipmentHelper
    {
        public static Vehicle EquipmentBuilder(List<string> listOfEquipment, Vehicle vehicle)
        {
            for (int i = 0; i < listOfEquipment.Count; i++)
            {
                Equipment newEquipment = new Equipment
                {
                    VehicledId = vehicle.Id,
                    Value = listOfEquipment[i]
                };
                vehicle.Equipment.Add(newEquipment);
            }

            return vehicle;
        }
    }
}
