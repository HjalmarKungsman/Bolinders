using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Bolinders.Core.Models
{
    public class VehiclesXml
    {
        [XmlElement("car")]
        public VehicleXml VehicleXml { get; set; }
    }

    public class VehicleXml
    {
        [XmlElement("regno")]
        public string RegistrationNumber { get; set; }
        [XmlElement("brand")]
        public string Make { get; set; }
        [XmlElement("model")]
        public string Model { get; set; }
        [XmlElement("modeldescription")]
        public string ModelDescription { get; set; }
        [XmlElement("yearmodel")]
        public int Year { get; set; }
        [XmlElement("miles")]
        public int Milage { get; set; }
        [XmlElement("price")]
        public double Price { get; set; }
        [XmlElement("bodytype")]
        public string BodyType { get; set; }
        [XmlElement("color")]
        public string Colour { get; set; }
        [XmlElement("gearboxtype")]
        public string Gearbox { get; set; }
        [XmlElement("fueltype")]
        public string FuelType { get; set; }
        [XmlElement("horsepower")]
        public int Horsepowers { get; set; }
        //If mils == 0 then Used = false
        //public bool Used { get; set; }
        [XmlElement("station")]
        public string FacilityId { get; set; }
        [XmlElement("images")]
        public List<ImageXml> Images { get; set; }
        [XmlElement("exkl_moms")]
        public bool Leasable { get; set; }
        [XmlElement("updated")]
        public int Updated { get; set; }
        [XmlElement("info")]
        public string Equipment { get; set; }
    }

    public class ImageXml
    {
        [XmlElement("image")]
        public string Url { get; set; }
    }
}
