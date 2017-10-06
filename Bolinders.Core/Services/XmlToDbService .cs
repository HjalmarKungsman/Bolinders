using Bolinders.Core.Helpers;
using Bolinders.Core.Models;
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
            VehiclesXml _vehicles = ParseXmlToObject(_xmlFile);

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
            //var cleanXmlFile1 = xmlFile.Replace("<? xml version = \"1.0\" encoding = \"iso - 8859 - 1\" ?>", "");
            //var cleanXmlFile2 = cleanXmlFile1.Replace(" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"", "");

            //XmlNodeList xnList = xmlFile.SelectNodes("/Names/Name");

            //var vehiclesXml = new VehicleXml();

            //XmlSerializer serializer = new XmlSerializer(typeof(VehicleXml), new XmlRootAttribute("cars"));
            //using (TextReader reader = new StringReader(xmlFile))
            //{
            //    vehiclesXml = (VehicleXml)serializer.Deserialize(reader);
            //};


            Serializer ser = new Serializer();
            string path = string.Empty;
            string xmlInputData = string.Empty;
            string xmlOutputData = string.Empty;

            // EXAMPLE 1
            //path = Directory.GetCurrentDirectory() + @"\Customer.xml";
            //xmlInputData = File.ReadAllText(path);
            xmlInputData = xmlFile;

            VehiclesXml vehiclesXml = ser.Deserialize<VehiclesXml>(xmlInputData);
            xmlOutputData = ser.Serialize<VehiclesXml>(vehiclesXml);




            return vehiclesXml;
        }

        public class Serializer
        {
            public T Deserialize<T>(string input) where T : class
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T), new XmlRootAttribute("cars"));

                using (StringReader sr = new StringReader(input))
                {
                    return (T)ser.Deserialize(sr);
                }
            }

            public string Serialize<T>(T ObjectToSerialize)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType(), new XmlRootAttribute("cars"));

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                    return textWriter.ToString();
                }
            }
        }
    }
}
