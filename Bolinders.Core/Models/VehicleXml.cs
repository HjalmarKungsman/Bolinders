using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Bolinders.Core.Models
{
    public class VehiclesXml
    {
        [XmlElement("car")]
        public List<VehicleXml> VehicleXml { get; set; }
    }

    [Serializable()]
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
        public string Year { get; set; }
        [XmlElement("miles")]
        public string Milage { get; set; }
        [XmlElement("price")]
        public string Price { get; set; }                       // double
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
        [XmlArray("images")]
        [XmlArrayItem("image", typeof(ImageXml))]
        public List<ImageXml> Images { get; set; }
        //public List<ImageXml> Images { get; set; }
        [XmlElement("exkl_moms")]
        public string Leasable { get; set; }                    // Bool?
        [XmlElement("updated")]
        public string Updated { get; set; }
        [XmlElement("info")]
        public string Equipment { get; set; }
    }

    [Serializable()]
    public class ImageXml
    {
        [XmlText()]
        public string Url { get; set; }
    }
}
