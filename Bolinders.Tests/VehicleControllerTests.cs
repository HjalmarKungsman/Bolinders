using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Bolinders.Web.Repositories;
using Bolinders.Web.Models;
using Bolinders.Web.Controllers;
using Bolinders.Web.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Bolinders.Web.ViewModels;

namespace Bolinders.Tests
{
    public class VehicleControllerTests
    {
        [Fact]
        public void Able_To_Paginate()
        {
            //Arrange
            //Install Moq package to use Mock
            Mock<IVehicleRepository> mock = new Mock<IVehicleRepository>();
            mock.Setup(m => m.Vehicles).Returns(new Vehicle[] {
                new Vehicle { Id = 1, Model = "Produkt 1" },
                new Vehicle { Id = 2, Model = "Produkt 2" },
                new Vehicle { Id = 3, Model = "Produkt 3" },
                new Vehicle { Id = 4, Model = "Produkt 4" },
                new Vehicle { Id = 5, Model = "Produkt 5" },
            });
            var pc = new VehicleController(mock.Object);
            pc.PageLimit = 3;

            //Act
            var response = (pc.List(2) as ViewResult)?.ViewData.Model as VehicleListViewModel;
            var vehicles = response.Vehicles;

            //Assert
            var vehicleArray = vehicles.ToArray();
            Assert.True(vehicleArray.Length == 2);
            Assert.Equal("Produkt 4", vehicleArray[0].Model);
            Assert.Equal("Produkt 5", vehicleArray[1].Model);
        }
    }
}
