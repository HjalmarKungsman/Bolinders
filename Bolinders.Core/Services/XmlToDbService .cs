using Bolinders.Core.Helpers;
using Bolinders.Core.Models;
using Bolinders.Core.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Bolinders.Core.Services
{
    public class XmlToDbService
    {
        public static void Run()
        {
            string _xmlFile = FtpDownload();
            VehiclesXml _vehiclesXml = ParseXmlToObject(_xmlFile);
            Vehicle _vehicle = ParseVehiclesXmlToVehicle(_vehiclesXml);


            

        }
        public static string FtpDownload()
        { 
            //TODO: Flytta adress, användarnamn och lösen till appsettings.json
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://ftp.brighten.se/xml/bb-crm.xml");
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential("159616_exp", "Qwerty123");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("windows-1252"));

            #region Serialize to Json
            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(reader.ReadToEnd());
            //string jsonText = JsonConvert.SerializeXmlNode(xmlDoc);
            #endregion

            string xmlFile = reader.ReadToEnd();

            reader.Close();
            response.Close();

            return xmlFile;
        }

        public static VehiclesXml ParseXmlToObject(string xmlFile)
        {
            Serializer ser = new Serializer();
            VehiclesXml vehiclesXml = ser.Deserialize<VehiclesXml>(xmlFile);

            return vehiclesXml;
        }

        public static Vehicle ParseVehiclesXmlToVehicle(VehiclesXml vehiclesXml)
        {
            var _vehiclesList = vehiclesXml.VehicleXml;




            Vehicle _vehicle = new Vehicle();
            return _vehicle;
        }


        public class Serializer
        {

            #region XML to object
            public T Deserialize<T>(string input) where T : class
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T), new XmlRootAttribute("cars"));

                using (StringReader sr = new StringReader(input))
                {
                    return (T)ser.Deserialize(sr);
                }
            }
            #endregion

            #region Oject to XML
            public string Serialize<T>(T ObjectToSerialize)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType(), new XmlRootAttribute("cars"));

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                    return textWriter.ToString();
                }
            }
#endregion
        }
    }
}
