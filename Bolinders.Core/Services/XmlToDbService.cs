﻿using Bolinders.Core.DataAccess;
using Bolinders.Core.Enums;
using Bolinders.Core.Models;
using Bolinders.Core.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Bolinders.Core.Services
{
    public class XmlToDbService : IXmlToDbService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _image;
        private readonly IEmailSenderService _email;
        private readonly IHostingEnvironment _environment;

        public XmlToDbService(ApplicationDbContext context, IImageService image, IEmailSenderService email, IHostingEnvironment environment)
        {
            _context = context;
            _image = image;
            _email = email;
            _environment = environment;
        }

        //Nu är metoden kopplad till en Controller och en View bara utvecklings skull.
        //Skall väl göras om till en void utan return som triggas från en tidsinställd Task.

        public string Run()
        //public static void Run()
        {
            //Metod 1, FTP nedladdning. Pausar den, tar så himla lång tid. Gör en fake-XML istället
            //string _xmlFile = FtpDownload();

            //Metod 2, Parsar XML-string till ett VehicleXml object
            //List<VehicleXml> vehiclesAll = ParseXmlToObject(_xmlFile);

            //Metod 2b fake. Istället för 1 och 2. Mockar en XML-string och parsar den till ett VehicleXml object
            List<VehicleXml> vehiclesAll = ParseXmlToObjectFake();

            //Metod 3. Väljer ut alla bilar som uppdaterats sista 24 h.
            List<VehicleXml> vehiclesUpdatedLastDay = SelectUpdatedVehicles(vehiclesAll);

            SortVehicles(vehiclesUpdatedLastDay);

            return "Ok";
        }

        //Connects to FTP and downloads the XML-file
        private string FtpDownload()
        {
            //TODO: Flytta adress, användarnamn och lösen till appsettings.json
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://ftp.brighten.se/xml/bb-crm.xml");
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential("159616_exp", "Qwerty123");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("windows-1252"));
            string xmlFile = reader.ReadToEnd();

            reader.Close();
            response.Close();

            return xmlFile;
        }

        //Converts the XML-string to an IEnumerable-list of VehicleXml's
        private List<VehicleXml> ParseXmlToObject(string xmlFile)
        {
            Serializer ser = new Serializer();
            VehiclesXml vehiclesXml = ser.Deserialize<VehiclesXml>(xmlFile);

            return vehiclesXml.VehicleXml;
        }

        //Generates a fake XML-string
        private List<VehicleXml> ParseXmlToObjectFake()
        {
            var testXmlFeed = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\n<cars xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">\n\t<car>\n\t\t<id>11159975</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>ABC123</regno>\n\t\t<brand>BMW</brand>\n\t\t<model>X5</model>\n\t\t<modeldescription>Xdrive 3.0</modeldescription>\n\t\t<yearmodel>2014</yearmodel>\n\t\t<miles>7577</miles>\n\t\t<price>425.000</price>\n\t\t<bodytype>SUV</bodytype>\n\t\t<color>Sparkling Brown Met</color>\n\t\t<station>BB1</station>\n\t\t<info>Sidogardinner Baktill,1 Ägare,Ljuspaket,Remote Services,Aut.Avbländning Speglar,Rails i Alu.Utförande,Utökad Smartphone Användning i IDrive,Individual Atracite Innertak,Paneler invändigt i smakfulla American Oak-utförande,Rattvärme,Fyrhjulsdrift,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,PDC Parksensorer inkl.Skärmvisning,Lastnät,AdBlue Reningssystem,4 - Zons Klimatanläggning,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Pass Airbag ON/OFF,Årsskatt: 3864kr,Dieselvärmare inkl. Fjärr &amp; Timer,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Elektrisk Dragkrok(Dragvikt max 3500kg),Stolsvärme Fram,Teleservices för Servicelägen verkstadsbesök Bil-data,Larm,Adaptiva BI-Xenon,Backkamera,Komfort Access System(Keyless),Aktiv Pedestrian Protection,Navigationssystem Professional,Remoteservices,Harman / Kardon Surround Ljudsystem,Multisportratt inkl. Paddlar,Elektrisk Baklucka,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Helljusassistent,20\\\" Y - Org. Fälg,Intelligent Emergency Call,Prova På Försäkring möjlig 1400kr / 4mån,Aktiv Styrning,Svensksåld,Golvmattor i Velourutförande</info>\n\t\t<gearboxtype>Automatisk</gearboxtype>\n\t\t<fueltype>Diesel</fueltype>\n\t\t<exkl_moms>1</exkl_moms>\n\t\t<added>1503583766</added>\n\t\t<horsepower>258</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159976</id>\n\t\t<updated>1507294506</updated>\n\t\t<regno>DEF345</regno>\n\t\t<brand>Ford</brand>\n\t\t<model>Fiesta</model>\n\t\t<modeldescription>Vignale</modeldescription>\n\t\t<yearmodel>2017</yearmodel>\n\t\t<miles>0</miles>\n\t\t<price>325.000</price>\n\t\t<bodytype>Småbil</bodytype>\n\t\t<color>Vit</color>\n\t\t<station>BB1</station>\n\t\t<info>Helljusassistent,Rails i Alu.Utförande,4 - Zons Klimatanläggning,1 Ägare,Backkamera,Intelligent Emergency Call,Rattvärme,Adaptiva BI-Xenon,Stolsvärme Fram,Harman / Kardon Surround Ljudsystem,Lastnät,Dieselvärmare inkl. Fjärr &amp; Timer,Pass Airbag ON/OFF,Teleservices för Servicelägen verkstadsbesök Bil-data,Elektrisk Baklucka,Årsskatt: 3864kr,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Aktiv Pedestrian Protection,Elektrisk Dragkrok(Dragvikt max 3500kg),Remote Services,Svensksåld,Multisportratt inkl. Paddlar,Navigationssystem Professional,Golvmattor i Velourutförande,Prova På Försäkring möjlig 1400kr / 4mån,Aktiv Styrning,Sidogardinner Baktill,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Komfort Access System(Keyless),Remoteservices,PDC Parksensorer inkl.Skärmvisning,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Paneler invändigt i smakfulla American Oak-utförande,AdBlue Reningssystem,Aut.Avbländning Speglar,20\\\" Y - Org. Fälg,Fyrhjulsdrift,Ljuspaket,Utökad Smartphone Användning i IDrive,Individual Atracite Innertak,Larm</info>\n\t\t<gearboxtype>Manuell</gearboxtype>\n\t\t<fueltype>Bensin</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>120</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159977</id>\n\t\t<updated>1507262106</updated>\n\t\t<regno>KRA987</regno>\n\t\t<brand>Ford</brand>\n\t\t<model>Mondeo</model>\n\t\t<modeldescription>Business</modeldescription>\n\t\t<yearmodel>2016</yearmodel>\n\t\t<miles>1563</miles>\n\t\t<price>375.000</price>\n\t\t<bodytype>Kombi</bodytype>\n\t\t<color>Magnetic Gray</color>\n\t\t<station>BB3</station>\n\t\t<info>Aktiv Pedestrian Protection,Harman / Kardon Surround Ljudsystem,Remoteservices,Fyrhjulsdrift,Svensksåld,Komfort Access System(Keyless),Utökad Smartphone Användning i IDrive,Elektrisk Baklucka,4 - Zons Klimatanläggning,Stolsvärme Fram,PDC Parksensorer inkl.Skärmvisning,Larm,Helljusassistent,Elektrisk Dragkrok(Dragvikt max 3500kg),Navigationssystem Professional,Pass Airbag ON/OFF,Individual Atracite Innertak,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Teleservices för Servicelägen verkstadsbesök Bil-data,Multisportratt inkl. Paddlar,Dieselvärmare inkl. Fjärr &amp; Timer,Aktiv Styrning,Sidogardinner Baktill,Golvmattor i Velourutförande,20\\\" Y - Org. Fälg,Rattvärme,Rails i Alu.Utförande,AdBlue Reningssystem,Lastnät,Aut.Avbländning Speglar,Remote Services,Paneler invändigt i smakfulla American Oak-utförande,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,1 Ägare,Adaptiva BI-Xenon,Årsskatt: 3864kr,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Ljuspaket,Intelligent Emergency Call,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Backkamera,Prova På Försäkring möjlig 1400kr / 4mån</info>\n\t\t<gearboxtype>Automatisk</gearboxtype>\n\t\t<fueltype>Diesel</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>200</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159978</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>LOL483</regno>\n\t\t<brand>Volvo</brand>\n\t\t<model>V70</model>\n\t\t<modeldescription>Momentum</modeldescription>\n\t\t<yearmodel>2015</yearmodel>\n\t\t<miles>8500</miles>\n\t\t<price>195.000</price>\n\t\t<bodytype>Kombi</bodytype>\n\t\t<color>Polar Blue</color>\n\t\t<station>BB3</station>\n\t\t<info>Adaptiva BI-Xenon,Aktiv Styrning,Individual Atracite Innertak,Pass Airbag ON/OFF,Dieselvärmare inkl. Fjärr &amp; Timer,20\\\" Y - Org. Fälg,Larm,Svensksåld,Remote Services,Harman / Kardon Surround Ljudsystem,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,1 Ägare,Sidogardinner Baktill,Lastnät,Teleservices för Servicelägen verkstadsbesök Bil-data,Backkamera,Elektrisk Dragkrok(Dragvikt max 3500kg),AdBlue Reningssystem,Navigationssystem Professional,Paneler invändigt i smakfulla American Oak-utförande,Multisportratt inkl. Paddlar,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Rattvärme,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,4 - Zons Klimatanläggning,Elektrisk Baklucka,Komfort Access System(Keyless),Rails i Alu.Utförande,Remoteservices,Prova På Försäkring möjlig 1400kr / 4mån,Aut.Avbländning Speglar,Utökad Smartphone Användning i IDrive,Helljusassistent,PDC Parksensorer inkl.Skärmvisning,Intelligent Emergency Call,Årsskatt: 3864kr,Golvmattor i Velourutförande,Ljuspaket,Fyrhjulsdrift,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Stolsvärme Fram,Aktiv Pedestrian Protection</info>\n\t\t<gearboxtype>Automatisk</gearboxtype>\n\t\t<fueltype>Diesel</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>185</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159979</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>YLO365</regno>\n\t\t<brand>Cadillac</brand>\n\t\t<model>Escalade</model>\n\t\t<modeldescription>ESV Platinum</modeldescription>\n\t\t<yearmodel>2018</yearmodel>\n\t\t<miles>0</miles>\n\t\t<price>950.000</price>\n\t\t<bodytype>SUV</bodytype>\n\t\t<color>Svart</color>\n\t\t<station>BB1</station>\n\t\t<info>Intelligent Emergency Call,Remote Services,Komfort Access System(Keyless),Aktiv Styrning,Ljuspaket,Multisportratt inkl. Paddlar,Navigationssystem Professional,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Rattvärme,Golvmattor i Velourutförande,Fyrhjulsdrift,Aktiv Pedestrian Protection,1 Ägare,Rails i Alu.Utförande,Dieselvärmare inkl. Fjärr &amp; Timer,Prova På Försäkring möjlig 1400kr / 4mån,Årsskatt: 3864kr,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Svensksåld,Paneler invändigt i smakfulla American Oak-utförande,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Individual Atracite Innertak,Aut.Avbländning Speglar,Utökad Smartphone Användning i IDrive,Backkamera,Sidogardinner Baktill,Teleservices för Servicelägen verkstadsbesök Bil-data,Stolsvärme Fram,Helljusassistent,Pass Airbag ON/OFF,20\\\" Y - Org. Fälg,PDC Parksensorer inkl.Skärmvisning,Elektrisk Dragkrok(Dragvikt max 3500kg),Remoteservices,Larm,Harman / Kardon Surround Ljudsystem,AdBlue Reningssystem,Adaptiva BI-Xenon,Elektrisk Baklucka,Lastnät,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,4 - Zons Klimatanläggning</info>\n\t\t<gearboxtype>Automatisk</gearboxtype>\n\t\t<fueltype>Bensin</fueltype>\n\t\t<exkl_moms>1</exkl_moms>\n\t\t<horsepower>450</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159980</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>ORA058</regno>\n\t\t<brand>Ford</brand>\n\t\t<model>Explorer</model>\n\t\t<modeldescription>Sport</modeldescription>\n\t\t<yearmodel>2017</yearmodel>\n\t\t<miles>0</miles>\n\t\t<price>750.000</price>\n\t\t<bodytype>SUV</bodytype>\n\t\t<color>Pearl White</color>\n\t\t<station>BB2</station>\n\t\t<info>Utökad Smartphone Användning i IDrive,Lastnät,Rails i Alu.Utförande,Intelligent Emergency Call,Dieselvärmare inkl. Fjärr &amp; Timer,Harman / Kardon Surround Ljudsystem,Backkamera,Helljusassistent,Aut.Avbländning Speglar,Stolsvärme Fram,Rattvärme,Komfort Access System(Keyless),20\\\" Y - Org. Fälg,Ljuspaket,4 - Zons Klimatanläggning,Årsskatt: 3864kr,Sidogardinner Baktill,Adaptiva BI-Xenon,Aktiv Pedestrian Protection,Elektrisk Baklucka,Larm,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Aktiv Styrning,Remoteservices,AdBlue Reningssystem,Fyrhjulsdrift,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Individual Atracite Innertak,Golvmattor i Velourutförande,Paneler invändigt i smakfulla American Oak-utförande,PDC Parksensorer inkl.Skärmvisning,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Svensksåld,Remote Services,1 Ägare,Teleservices för Servicelägen verkstadsbesök Bil-data,Elektrisk Dragkrok(Dragvikt max 3500kg),Navigationssystem Professional,Multisportratt inkl. Paddlar,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Prova På Försäkring möjlig 1400kr / 4mån,Pass Airbag ON/OFF</info>\n\t\t<gearboxtype>Automatisk</gearboxtype>\n\t\t<fueltype>Bensin</fueltype>\n\t\t<exkl_moms>1</exkl_moms>\n\t\t<horsepower>425</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159981</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>GRY902</regno>\n\t\t<brand>Land Rover</brand>\n\t\t<model>Discovery</model>\n\t\t<modeldescription>Versatile</modeldescription>\n\t\t<yearmodel>2016</yearmodel>\n\t\t<miles>1985</miles>\n\t\t<price>420.000</price>\n\t\t<bodytype>SUV</bodytype>\n\t\t<color>British racing green</color>\n\t\t<station>BB3</station>\n\t\t<info>Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Utökad Smartphone Användning i IDrive,Helljusassistent,Lastnät,Adaptiva BI-Xenon,Intelligent Emergency Call,Komfort Access System(Keyless),Elektrisk Baklucka,Rattvärme,Rails i Alu.Utförande,AdBlue Reningssystem,Sidogardinner Baktill,Paneler invändigt i smakfulla American Oak-utförande,Elektrisk Dragkrok(Dragvikt max 3500kg),Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Larm,Golvmattor i Velourutförande,Fyrhjulsdrift,20\\\" Y - Org. Fälg,Svensksåld,4 - Zons Klimatanläggning,Multisportratt inkl. Paddlar,Individual Atracite Innertak,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Aut.Avbländning Speglar,Pass Airbag ON/OFF,PDC Parksensorer inkl.Skärmvisning,Årsskatt: 3864kr,Ljuspaket,Remoteservices,Stolsvärme Fram,Harman / Kardon Surround Ljudsystem,Aktiv Styrning,Teleservices för Servicelägen verkstadsbesök Bil-data,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,1 Ägare,Dieselvärmare inkl. Fjärr &amp; Timer,Remote Services,Backkamera,Navigationssystem Professional,Aktiv Pedestrian Protection,Prova På Försäkring möjlig 1400kr / 4mån</info>\n\t\t<gearboxtype>Automatisk</gearboxtype>\n\t\t<fueltype>Diesel</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>385</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159982</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>WAS984</regno>\n\t\t<brand>Kia</brand>\n\t\t<model>Carens</model>\n\t\t<modeldescription>CRDI 16V</modeldescription>\n\t\t<yearmodel>2010</yearmodel>\n\t\t<miles>12560</miles>\n\t\t<price>65.000</price>\n\t\t<bodytype>Familjebuss</bodytype>\n\t\t<color>Silver</color>\n\t\t<station>BB2</station>\n\t\t<info>Remoteservices,Ljuspaket,Rails i Alu.Utförande,Adaptiva BI-Xenon,Paneler invändigt i smakfulla American Oak-utförande,Sidogardinner Baktill,Stolsvärme Fram,Lastnät,Årsskatt: 3864kr,Helljusassistent,Individual Atracite Innertak,AdBlue Reningssystem,Dieselvärmare inkl. Fjärr &amp; Timer,Svensksåld,Remote Services,Harman / Kardon Surround Ljudsystem,4 - Zons Klimatanläggning,Komfort Access System(Keyless),Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Intelligent Emergency Call,Multisportratt inkl. Paddlar,Pass Airbag ON/OFF,Teleservices för Servicelägen verkstadsbesök Bil-data,Elektrisk Dragkrok(Dragvikt max 3500kg),Utökad Smartphone Användning i IDrive,Aktiv Styrning,20\\\" Y - Org. Fälg,Navigationssystem Professional,Fyrhjulsdrift,PDC Parksensorer inkl.Skärmvisning,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Rattvärme,1 Ägare,Golvmattor i Velourutförande,Larm,Prova På Försäkring möjlig 1400kr / 4mån,Elektrisk Baklucka,Aktiv Pedestrian Protection,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Backkamera,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Aut.Avbländning Speglar</info>\n\t\t<gearboxtype>Manuell</gearboxtype>\n\t\t<fueltype>Diesel</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>85</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159983</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>LAT527</regno>\n\t\t<brand>Tesla</brand>\n\t\t<model>P85D</model>\n\t\t<modeldescription></modeldescription>\n\t\t<yearmodel>2016</yearmodel>\n\t\t<miles>2000</miles>\n\t\t<price>560.000</price>\n\t\t<bodytype>Sedan</bodytype>\n\t\t<color>Svart</color>\n\t\t<station>BB3</station>\n\t\t<info>Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Remote Services,Ljuspaket,Golvmattor i Velourutförande,Aktiv Pedestrian Protection,Navigationssystem Professional,Fyrhjulsdrift,Helljusassistent,20\\\" Y - Org. Fälg,Stolsvärme Fram,Teleservices för Servicelägen verkstadsbesök Bil-data,Lastnät,1 Ägare,Harman / Kardon Surround Ljudsystem,Komfort Access System(Keyless),Rattvärme,Svensksåld,Aut.Avbländning Speglar,Individual Atracite Innertak,Backkamera,Pass Airbag ON/OFF,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Intelligent Emergency Call,Elektrisk Dragkrok(Dragvikt max 3500kg),Dieselvärmare inkl. Fjärr &amp; Timer,Rails i Alu.Utförande,4 - Zons Klimatanläggning,Larm,Prova På Försäkring möjlig 1400kr / 4mån,Utökad Smartphone Användning i IDrive,AdBlue Reningssystem,Årsskatt: 3864kr,Adaptiva BI-Xenon,Sidogardinner Baktill,Aktiv Styrning,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Multisportratt inkl. Paddlar,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,PDC Parksensorer inkl.Skärmvisning,Elektrisk Baklucka,Remoteservices,Paneler invändigt i smakfulla American Oak-utförande</info>\n\t\t<gearboxtype>Automatisk</gearboxtype>\n\t\t<fueltype>El</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>700</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159984</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>MYS789</regno>\n\t\t<brand>Mazda</brand>\n\t\t<model>MX-5</model>\n\t\t<modeldescription>2.0 Hardtop</modeldescription>\n\t\t<yearmodel>2015</yearmodel>\n\t\t<miles>1500</miles>\n\t\t<price>225.000</price>\n\t\t<bodytype>Cab</bodytype>\n\t\t<color>Röd</color>\n\t\t<station>BB2</station>\n\t\t<info>Stolsvärme Fram,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Remoteservices,20\\\" Y - Org. Fälg,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Elektrisk Baklucka,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Golvmattor i Velourutförande,Svensksåld,Navigationssystem Professional,Fyrhjulsdrift,Komfort Access System(Keyless),Sidogardinner Baktill,Dieselvärmare inkl. Fjärr &amp; Timer,Remote Services,Teleservices för Servicelägen verkstadsbesök Bil-data,Adaptiva BI-Xenon,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Pass Airbag ON/OFF,Aktiv Pedestrian Protection,Backkamera,Ljuspaket,PDC Parksensorer inkl.Skärmvisning,Multisportratt inkl. Paddlar,Harman / Kardon Surround Ljudsystem,Intelligent Emergency Call,Aktiv Styrning,Individual Atracite Innertak,Aut.Avbländning Speglar,Årsskatt: 3864kr,4 - Zons Klimatanläggning,Utökad Smartphone Användning i IDrive,1 Ägare,Elektrisk Dragkrok(Dragvikt max 3500kg),Rattvärme,Larm,Prova På Försäkring möjlig 1400kr / 4mån,Paneler invändigt i smakfulla American Oak-utförande,Lastnät,AdBlue Reningssystem,Helljusassistent,Rails i Alu.Utförande</info>\n\t\t<gearboxtype>Manuell</gearboxtype>\n\t\t<fueltype>Bensin</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>195</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159985</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>HEJ642</regno>\n\t\t<brand>Ford</brand>\n\t\t<model>Focus</model>\n\t\t<modeldescription>RS</modeldescription>\n\t\t<yearmodel>2016</yearmodel>\n\t\t<miles>1500</miles>\n\t\t<price>390.000</price>\n\t\t<bodytype>Småbil</bodytype>\n\t\t<color>Magnetic Gray</color>\n\t\t<station>BB1</station>\n\t\t<info>Stolsvärme Fram,Aktiv Styrning,Backkamera,Aut.Avbländning Speglar,Ljuspaket,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Elektrisk Dragkrok(Dragvikt max 3500kg),1 Ägare,Remote Services,Fyrhjulsdrift,Remoteservices,Larm,Lastnät,Multisportratt inkl. Paddlar,PDC Parksensorer inkl.Skärmvisning,Aktiv Pedestrian Protection,Sidogardinner Baktill,Prova På Försäkring möjlig 1400kr / 4mån,Teleservices för Servicelägen verkstadsbesök Bil-data,Paneler invändigt i smakfulla American Oak-utförande,20\\\" Y - Org. Fälg,Utökad Smartphone Användning i IDrive,4 - Zons Klimatanläggning,Adaptiva BI-Xenon,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Intelligent Emergency Call,Individual Atracite Innertak,Svensksåld,Navigationssystem Professional,Komfort Access System(Keyless),Helljusassistent,Rattvärme,Pass Airbag ON/OFF,Elektrisk Baklucka,Dieselvärmare inkl. Fjärr &amp; Timer,Golvmattor i Velourutförande,Årsskatt: 3864kr,Harman / Kardon Surround Ljudsystem,Rails i Alu.Utförande,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,AdBlue Reningssystem</info>\n\t\t<gearboxtype>Manuell</gearboxtype>\n\t\t<fueltype>Bensin</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>350</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159986</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>FET201</regno>\n\t\t<brand>Honda</brand>\n\t\t<model>Civic</model>\n\t\t<modeldescription>Type-R</modeldescription>\n\t\t<yearmodel>2016</yearmodel>\n\t\t<miles>1800</miles>\n\t\t<price>350.000</price>\n\t\t<bodytype>Småbil</bodytype>\n\t\t<color>Vit</color>\n\t\t<station>BB3</station>\n\t\t<info>Rails i Alu.Utförande,Backkamera,Navigationssystem Professional,Rattvärme,AdBlue Reningssystem,Fyrhjulsdrift,Paneler invändigt i smakfulla American Oak-utförande,Harman / Kardon Surround Ljudsystem,Sidogardinner Baktill,Komfort Access System(Keyless),1 Ägare,Elektrisk Dragkrok(Dragvikt max 3500kg),Dieselvärmare inkl. Fjärr &amp; Timer,Lastnät,Utökad Smartphone Användning i IDrive,Prova På Försäkring möjlig 1400kr / 4mån,Larm,Aut.Avbländning Speglar,Årsskatt: 3864kr,Teleservices för Servicelägen verkstadsbesök Bil-data,Aktiv Styrning,PDC Parksensorer inkl.Skärmvisning,Stolsvärme Fram,Intelligent Emergency Call,Individual Atracite Innertak,Golvmattor i Velourutförande,Remoteservices,Adaptiva BI-Xenon,Multisportratt inkl. Paddlar,20\\\" Y - Org. Fälg,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,4 - Zons Klimatanläggning,Pass Airbag ON/OFF,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Svensksåld,Helljusassistent,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Remote Services,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Aktiv Pedestrian Protection,Ljuspaket,Elektrisk Baklucka</info>\n\t\t<gearboxtype>Manuell</gearboxtype>\n\t\t<fueltype>Bensin</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>300</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159987</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>SKO309</regno>\n\t\t<brand>Volvo</brand>\n\t\t<model>V90</model>\n\t\t<modeldescription>Executive</modeldescription>\n\t\t<yearmodel>2017</yearmodel>\n\t\t<miles>0</miles>\n\t\t<price>485.000</price>\n\t\t<bodytype>Kombi</bodytype>\n\t\t<color>Svart</color>\n\t\t<station>BB1</station>\n\t\t<info>Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Harman / Kardon Surround Ljudsystem,Årsskatt: 3864kr,Rattvärme,Aktiv Pedestrian Protection,Remote Services,Svensksåld,Paneler invändigt i smakfulla American Oak-utförande,Multisportratt inkl. Paddlar,Helljusassistent,4 - Zons Klimatanläggning,Utökad Smartphone Användning i IDrive,Individual Atracite Innertak,Intelligent Emergency Call,Golvmattor i Velourutförande,Elektrisk Baklucka,Aktiv Styrning,Navigationssystem Professional,Pass Airbag ON/OFF,Larm,Aut.Avbländning Speglar,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Backkamera,Stolsvärme Fram,Sidogardinner Baktill,Remoteservices,Ljuspaket,Teleservices för Servicelägen verkstadsbesök Bil-data,Elektrisk Dragkrok(Dragvikt max 3500kg),AdBlue Reningssystem,PDC Parksensorer inkl.Skärmvisning,20\\\" Y - Org. Fälg,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Rails i Alu.Utförande,Adaptiva BI-Xenon,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Prova På Försäkring möjlig 1400kr / 4mån,Lastnät,Komfort Access System(Keyless),Fyrhjulsdrift,1 Ägare,Dieselvärmare inkl. Fjärr &amp; Timer</info>\n\t\t<gearboxtype>Automatisk</gearboxtype>\n\t\t<fueltype>Bensin</fueltype>\n\t\t<exkl_moms>1</exkl_moms>\n\t\t<horsepower>175</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159988</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>STI879</regno>\n\t\t<brand>Ford</brand>\n\t\t<model>Model T</model>\n\t\t<modeldescription></modeldescription>\n\t\t<yearmodel>1925</yearmodel>\n\t\t<miles></miles>\n\t\t<price>120.000</price>\n\t\t<bodytype>Småbil</bodytype>\n\t\t<color>Svart</color>\n\t\t<station>BB2</station>\n\t\t<info>Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Navigationssystem Professional,Rails i Alu.Utförande,Adaptiva BI-Xenon,Utökad Smartphone Användning i IDrive,Elektrisk Baklucka,Dieselvärmare inkl. Fjärr &amp; Timer,Fyrhjulsdrift,Aut.Avbländning Speglar,PDC Parksensorer inkl.Skärmvisning,Årsskatt: 3864kr,Elektrisk Dragkrok(Dragvikt max 3500kg),Remoteservices,Remote Services,Backkamera,20\\\" Y - Org. Fälg,Intelligent Emergency Call,Paneler invändigt i smakfulla American Oak-utförande,Aktiv Pedestrian Protection,Lastnät,Stolsvärme Fram,Prova På Försäkring möjlig 1400kr / 4mån,1 Ägare,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Golvmattor i Velourutförande,Sidogardinner Baktill,Larm,Individual Atracite Innertak,Rattvärme,Multisportratt inkl. Paddlar,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Komfort Access System(Keyless),Aktiv Styrning,Ljuspaket,AdBlue Reningssystem,Helljusassistent,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Svensksåld,Teleservices för Servicelägen verkstadsbesök Bil-data,Pass Airbag ON/OFF,Harman / Kardon Surround Ljudsystem,4 - Zons Klimatanläggning</info>\n\t\t<gearboxtype>Manuell</gearboxtype>\n\t\t<fueltype>Bensin</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>25</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159989</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>KOL472</regno>\n\t\t<brand>Dacia</brand>\n\t\t<model>Sandero</model>\n\t\t<modeldescription>Stepway</modeldescription>\n\t\t<yearmodel>2013</yearmodel>\n\t\t<miles>8500</miles>\n\t\t<price>90.000</price>\n\t\t<bodytype>Småbil</bodytype>\n\t\t<color>BlÃ¥</color>\n\t\t<station>BB2</station>\n\t\t<info>Komfort Access System(Keyless),Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Remote Services,Aktiv Pedestrian Protection,Pass Airbag ON/OFF,4 - Zons Klimatanläggning,Utökad Smartphone Användning i IDrive,Multisportratt inkl. Paddlar,Rattvärme,Sidogardinner Baktill,Navigationssystem Professional,Adaptiva BI-Xenon,Svensksåld,PDC Parksensorer inkl.Skärmvisning,Rails i Alu.Utförande,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Teleservices för Servicelägen verkstadsbesök Bil-data,Ljuspaket,Larm,Lastnät,Intelligent Emergency Call,Årsskatt: 3864kr,Elektrisk Dragkrok(Dragvikt max 3500kg),Fyrhjulsdrift,1 Ägare,Aut.Avbländning Speglar,Aktiv Styrning,Stolsvärme Fram,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Prova På Försäkring möjlig 1400kr / 4mån,AdBlue Reningssystem,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Remoteservices,Harman / Kardon Surround Ljudsystem,Helljusassistent,Individual Atracite Innertak,Golvmattor i Velourutförande,20\\\" Y - Org. Fälg,Elektrisk Baklucka,Backkamera,Dieselvärmare inkl. Fjärr &amp; Timer,Paneler invändigt i smakfulla American Oak-utförande</info>\n\t\t<gearboxtype>Manuell</gearboxtype>\n\t\t<fueltype>Diesel</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>85</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159990</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>LOK053</regno>\n\t\t<brand>Fiat</brand>\n\t\t<model>500</model>\n\t\t<modeldescription>Abarth</modeldescription>\n\t\t<yearmodel>2010</yearmodel>\n\t\t<miles>8400</miles>\n\t\t<price>150.000</price>\n\t\t<bodytype>Småbil</bodytype>\n\t\t<color>Mörkgråmetallic</color>\n\t\t<station>BB1</station>\n\t\t<info>Intelligent Emergency Call,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Svensksåld,Sidogardinner Baktill,20\\\" Y - Org. Fälg,4 - Zons Klimatanläggning,Larm,Lastnät,Helljusassistent,Fyrhjulsdrift,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Elektrisk Dragkrok(Dragvikt max 3500kg),Utökad Smartphone Användning i IDrive,1 Ägare,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Stolsvärme Fram,Aut.Avbländning Speglar,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,PDC Parksensorer inkl.Skärmvisning,Remote Services,Remoteservices,Harman / Kardon Surround Ljudsystem,Prova På Försäkring möjlig 1400kr / 4mån,Backkamera,Årsskatt: 3864kr,Rattvärme,Golvmattor i Velourutförande,Elektrisk Baklucka,Teleservices för Servicelägen verkstadsbesök Bil-data,Rails i Alu.Utförande,Dieselvärmare inkl. Fjärr &amp; Timer,Adaptiva BI-Xenon,Aktiv Pedestrian Protection,Multisportratt inkl. Paddlar,Pass Airbag ON/OFF,Ljuspaket,Paneler invändigt i smakfulla American Oak-utförande,Komfort Access System(Keyless),Navigationssystem Professional,AdBlue Reningssystem,Aktiv Styrning,Individual Atracite Innertak</info>\n\t\t<gearboxtype>Manuell</gearboxtype>\n\t\t<fueltype>Bensin</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>212</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159991</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>TAG931</regno>\n\t\t<brand>Mercedes-Benz</brand>\n\t\t<model>E</model>\n\t\t<modeldescription>220D</modeldescription>\n\t\t<yearmodel>2017</yearmodel>\n\t\t<miles>0</miles>\n\t\t<price>485.000</price>\n\t\t<bodytype>Kombi</bodytype>\n\t\t<color>Svart</color>\n\t\t<station>BB3</station>\n\t\t<info>Komfort Access System(Keyless),Multisportratt inkl. Paddlar,Årsskatt: 3864kr,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Pass Airbag ON/OFF,Svensksåld,Prova På Försäkring möjlig 1400kr / 4mån,Backkamera,Ljuspaket,Helljusassistent,Lastnät,Rails i Alu.Utförande,Aktiv Styrning,Rattvärme,1 Ägare,Larm,Remoteservices,PDC Parksensorer inkl.Skärmvisning,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Fyrhjulsdrift,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Intelligent Emergency Call,20\\\" Y - Org. Fälg,Utökad Smartphone Användning i IDrive,Golvmattor i Velourutförande,Elektrisk Dragkrok(Dragvikt max 3500kg),Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,AdBlue Reningssystem,Remote Services,Sidogardinner Baktill,Aktiv Pedestrian Protection,Teleservices för Servicelägen verkstadsbesök Bil-data,Elektrisk Baklucka,4 - Zons Klimatanläggning,Stolsvärme Fram,Aut.Avbländning Speglar,Adaptiva BI-Xenon,Dieselvärmare inkl. Fjärr &amp; Timer,Paneler invändigt i smakfulla American Oak-utförande,Individual Atracite Innertak,Navigationssystem Professional,Harman / Kardon Surround Ljudsystem</info>\n\t\t<gearboxtype>Manuell</gearboxtype>\n\t\t<fueltype>Diesel</fueltype>\n\t\t<exkl_moms>1</exkl_moms>\n\t\t<horsepower>205</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159992</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>STO014</regno>\n\t\t<brand>Volvo</brand>\n\t\t<model>XC90</model>\n\t\t<modeldescription>T8 AWD</modeldescription>\n\t\t<yearmodel>2017</yearmodel>\n\t\t<miles>0</miles>\n\t\t<price>850.000</price>\n\t\t<bodytype>SUV</bodytype>\n\t\t<color>Blå</color>\n\t\t<station>BB1</station>\n\t\t<info>Dieselvärmare inkl. Fjärr &amp; Timer,Intelligent Emergency Call,Elektrisk Baklucka,Pass Airbag ON/OFF,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Aktiv Styrning,Ljuspaket,4 - Zons Klimatanläggning,Aktiv Pedestrian Protection,Stolsvärme Fram,Utökad Smartphone Användning i IDrive,Komfort Access System(Keyless),Lastnät,Elektrisk Dragkrok(Dragvikt max 3500kg),Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Harman / Kardon Surround Ljudsystem,AdBlue Reningssystem,Backkamera,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,1 Ägare,Aut.Avbländning Speglar,Paneler invändigt i smakfulla American Oak-utförande,Multisportratt inkl. Paddlar,Navigationssystem Professional,Remote Services,Golvmattor i Velourutförande,Larm,Fyrhjulsdrift,Individual Atracite Innertak,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,20\\\" Y - Org. Fälg,Svensksåld,Prova På Försäkring möjlig 1400kr / 4mån,Teleservices för Servicelägen verkstadsbesök Bil-data,Sidogardinner Baktill,Årsskatt: 3864kr,Rails i Alu.Utförande,Helljusassistent,Rattvärme,PDC Parksensorer inkl.Skärmvisning,Remoteservices,Adaptiva BI-Xenon</info>\n\t\t<gearboxtype>Automatisk</gearboxtype>\n\t\t<fueltype>Bensin</fueltype>\n\t\t<exkl_moms>1</exkl_moms>\n\t\t<horsepower>525</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159993</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>BUR498</regno>\n\t\t<brand>Renault</brand>\n\t\t<model>Megane</model>\n\t\t<modeldescription>Sport Tourer</modeldescription>\n\t\t<yearmodel>2017</yearmodel>\n\t\t<miles>0</miles>\n\t\t<price></price>\n\t\t<bodytype>Kombi</bodytype>\n\t\t<color>Silvermetallic</color>\n\t\t<station>BB3</station>\n\t\t<info>Aktiv Pedestrian Protection,Fyrhjulsdrift,Adaptiva BI-Xenon,Rails i Alu.Utförande,Stolsvärme Fram,Aktiv Styrning,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Aut.Avbländning Speglar,Ljuspaket,PDC Parksensorer inkl.Skärmvisning,Utökad Smartphone Användning i IDrive,Larm,Paneler invändigt i smakfulla American Oak-utförande,Årsskatt: 3864kr,Backkamera,Prova På Försäkring möjlig 1400kr / 4mån,Golvmattor i Velourutförande,Komfort Access System(Keyless),Individual Atracite Innertak,Harman / Kardon Surround Ljudsystem,Helljusassistent,AdBlue Reningssystem,20\\\" Y - Org. Fälg,Remoteservices,Navigationssystem Professional,1 Ägare,Elektrisk Baklucka,Dieselvärmare inkl. Fjärr &amp; Timer,Rattvärme,Multisportratt inkl. Paddlar,Elektrisk Dragkrok(Dragvikt max 3500kg),Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Lastnät,Intelligent Emergency Call,Pass Airbag ON/OFF,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Teleservices för Servicelägen verkstadsbesök Bil-data,Sidogardinner Baktill,Svensksåld,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,4 - Zons Klimatanläggning,Remote Services</info>\n\t\t<gearboxtype>Automatisk</gearboxtype>\n\t\t<fueltype>Bensin</fueltype>\n\t\t<exkl_moms>1</exkl_moms>\n\t\t<horsepower>225</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159994</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>HBH051</regno>\n\t\t<brand>Dacia</brand>\n\t\t<model>Duster</model>\n\t\t<modeldescription>Black Shadow</modeldescription>\n\t\t<yearmodel>2016</yearmodel>\n\t\t<miles>1500</miles>\n\t\t<price>185.000</price>\n\t\t<bodytype>SUV</bodytype>\n\t\t<color>Svart</color>\n\t\t<station>BB3</station>\n\t\t<info>Rattvärme,1 Ägare,Prova På Försäkring möjlig 1400kr / 4mån,Lastnät,Remote Services,Svensksåld,Utökad Smartphone Användning i IDrive,Komfort Access System(Keyless),20\\\" Y - Org. Fälg,Fyrhjulsdrift,Stolsvärme Fram,Aut.Avbländning Speglar,Aktiv Styrning,Individual Atracite Innertak,Teleservices för Servicelägen verkstadsbesök Bil-data,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Elektrisk Baklucka,Dieselvärmare inkl. Fjärr &amp; Timer,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Larm,Navigationssystem Professional,Golvmattor i Velourutförande,Elektrisk Dragkrok(Dragvikt max 3500kg),4 - Zons Klimatanläggning,Årsskatt: 3864kr,Multisportratt inkl. Paddlar,Remoteservices,Backkamera,Helljusassistent,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,PDC Parksensorer inkl.Skärmvisning,AdBlue Reningssystem,Paneler invändigt i smakfulla American Oak-utförande,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Pass Airbag ON/OFF,Aktiv Pedestrian Protection,Sidogardinner Baktill,Adaptiva BI-Xenon,Intelligent Emergency Call,Rails i Alu.Utförande,Harman / Kardon Surround Ljudsystem,Ljuspaket</info>\n\t\t<gearboxtype>Manuell</gearboxtype>\n\t\t<fueltype>Diesel</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>125</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159995</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>TNJ007</regno>\n\t\t<brand>Dodge</brand>\n\t\t<model>Ram</model>\n\t\t<modeldescription>5.7 Hemi</modeldescription>\n\t\t<yearmodel>2013</yearmodel>\n\t\t<miles>7500</miles>\n\t\t<price>359.900</price>\n\t\t<bodytype>Yrkesfordon</bodytype>\n\t\t<color>Svart</color>\n\t\t<station>BB1</station>\n\t\t<info>Rattvärme,Navigationssystem Professional,Prova På Försäkring möjlig 1400kr / 4mån,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Backkamera,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,AdBlue Reningssystem,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Fyrhjulsdrift,Pass Airbag ON/OFF,Multisportratt inkl. Paddlar,Stolsvärme Fram,Dieselvärmare inkl. Fjärr &amp; Timer,Årsskatt: 3864kr,Ljuspaket,Remote Services,Golvmattor i Velourutförande,Teleservices för Servicelägen verkstadsbesök Bil-data,Elektrisk Dragkrok(Dragvikt max 3500kg),ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Komfort Access System(Keyless),1 Ägare,Elektrisk Baklucka,20\\\" Y - Org. Fälg,Adaptiva BI-Xenon,Aut.Avbländning Speglar,Aktiv Styrning,PDC Parksensorer inkl.Skärmvisning,Aktiv Pedestrian Protection,Rails i Alu.Utförande,Svensksåld,Helljusassistent,Individual Atracite Innertak,Paneler invändigt i smakfulla American Oak-utförande,Utökad Smartphone Användning i IDrive,Lastnät,Harman / Kardon Surround Ljudsystem,Larm,4 - Zons Klimatanläggning,Intelligent Emergency Call,Remoteservices,Sidogardinner Baktill</info>\n\t\t<gearboxtype>Automatisk</gearboxtype>\n\t\t<fueltype>Bensin</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>425</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n\t<car>\n\t\t<id>11159996</id>\n\t\t<updated>1505373298</updated>\n\t\t<regno>SAK610</regno>\n\t\t<brand>Peugeot</brand>\n\t\t<model>206</model>\n\t\t<modeldescription>GTI</modeldescription>\n\t\t<yearmodel>2005</yearmodel>\n\t\t<miles>15500</miles>\n\t\t<price>54.000</price>\n\t\t<bodytype>Småbil</bodytype>\n\t\t<color>BlÃ¥metallic</color>\n\t\t<station>BB3</station>\n\t\t<info>Pass Airbag ON/OFF,Elektrisk Baklucka,Ljuspaket,Aktiv Styrning,Dieselvärmare inkl. Fjärr &amp; Timer,Interiör: Läder Inredning i Dakota/ Elfenbeinvit Kombination,Adaptiva BI-Xenon,Remote Services,Rattvärme,Backkamera,Svensksåld,Prova På Försäkring möjlig 1400kr / 4mån,Multisportratt inkl. Paddlar,Elektrisk Dragkrok(Dragvikt max 3500kg),Remoteservices,Individual Atracite Innertak,Aut.Avbländning Speglar,Teleservices för Servicelägen verkstadsbesök Bil-data,Utökad Smartphone Användning i IDrive,ConnectedDrive Services Fjärrstrning via ex App &amp; Dator,Sidogardinner Baktill,Lastnät,Aktiv Pedestrian Protection,Fyrhjulsdrift,AdBlue Reningssystem,Stolsvärme Fram,Årsskatt: 3864kr,Paneler invändigt i smakfulla American Oak-utförande,1 Ägare,Golvmattor i Velourutförande,Larm,4 - Zons Klimatanläggning,Komfort Access System(Keyless),Helljusassistent,Harman / Kardon Surround Ljudsystem,PDC Parksensorer inkl.Skärmvisning,Sportstolar inkl. Elektriskinställning &amp; Svankstödinställn.,Vinterhjul 19\\\" Org. M-Fälg Hakka Dubb,Rails i Alu.Utförande,20\\\" Y - Org. Fälg,Navigationssystem Professional,Intelligent Emergency Call</info>\n\t\t<gearboxtype>Manuell</gearboxtype>\n\t\t<fueltype>Bensin</fueltype>\n\t\t<exkl_moms></exkl_moms>\n\t\t<horsepower>125</horsepower>\n\t\t<images>\n\t\t\t<image>http://brighten.se/export/images/bild-1.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-2.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-3.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-4.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-5.jpg</image>\n\t\t\t<image>http://brighten.se/export/images/bild-6.jpg</image>\n\t\t</images>\n\t</car>\n</cars>";

            Serializer ser = new Serializer();
            VehiclesXml vehiclesXml = ser.Deserialize<VehiclesXml>(testXmlFeed);

            return vehiclesXml.VehicleXml;
        }

        private List<VehicleXml> SelectUpdatedVehicles(List<VehicleXml> allVehicles)
        {
            TimeSpan epochTicks = new TimeSpan(new DateTime(1970, 1, 1).Ticks);
            // Fake-XML:en s upate värden är satta till typ i torsdags eller fredags. Ändra bara AddDays(-1) till -5 så levererar den här metoden ett litet urval...
            TimeSpan unixTicks = new TimeSpan(DateTime.UtcNow.AddDays(-5).Ticks) - epochTicks;
            //TODO: Gör om så den inte jämför från Nu till minus 24h utan istället jämför idag klockan 01:00:0000 och minus 24h. Risken finns ju att den ena dagen triggas 01:00:0001 och andra dagaen triggas den 01:00:0077. Alltså skulle det finnas utrymme för att en bil faller emellan...


            var vehiclesUpdatedLastDay = allVehicles.Select(n => n).Where(n => Double.Parse(n.Updated) > unixTicks.TotalSeconds).ToList();

            return vehiclesUpdatedLastDay;
        }

        private void SortVehicles(List<VehicleXml> vehicles)
        {
            List<Guid> addedVehicles = new List<Guid>();
            List<VehicleXml> vehiclesToRemove = new List<VehicleXml>();

            //Checks for edits and apply them.
            foreach (var vehicle in vehicles)
            {

                var existingVehicle = _context.Vehicles.Where(x => x.RegistrationNumber == vehicle.RegistrationNumber).Include(x => x.Images).FirstOrDefault();
                if (existingVehicle == null)
                {
                    break;
                }

                addedVehicles.Add(existingVehicle.Id);

                List<string> listOfImages = new List<string>();

                foreach (var image in vehicle.Images)
                {
                    listOfImages.Add(image.Url);
                }

                //remove old images from disk
                var directory = Path.Combine(_environment.WebRootPath, "images/uploads");
                foreach (var image in existingVehicle.Images)
                {
                    _image.RemoveImageFromDisk(directory, image.ImageUrl);
                    _context.Images.Remove(image);
                }
                
                existingVehicle.Colour = vehicle.Colour;
                existingVehicle.FacilityId = vehicle.FacilityId;
                existingVehicle.Horsepowers = vehicle.Horsepowers;
                existingVehicle.Make = _context.Make.Where(x => x.Name == vehicle.Make).FirstOrDefault();
                existingVehicle.Mileage = Int32.Parse(vehicle.Milage);
                existingVehicle.Model = vehicle.Model;
                existingVehicle.ModelDescription = vehicle.ModelDescription;
                existingVehicle.RegistrationNumber = vehicle.RegistrationNumber;
                existingVehicle.Updated = FromUnixTime(Int32.Parse(vehicle.Updated));
                existingVehicle.Year = Int32.Parse(vehicle.Year);
                existingVehicle.Images = new List<Image>();

                if (Enum.TryParse(vehicle.BodyType, out BodyType bodyType))
                {
                    existingVehicle.BodyType = bodyType;
                }
                if (Enum.TryParse(vehicle.FuelType, out FuelType fuelType))
                {
                    existingVehicle.FuelType = fuelType;
                }
                if (Enum.TryParse(vehicle.Gearbox, out Gearbox gearbox))
                {
                    existingVehicle.Gearbox = gearbox;
                }

                if (vehicle.Leasable == "0" || vehicle.Leasable == "")
                {
                    existingVehicle.Leasable = false;
                }
                else
                {
                    existingVehicle.Leasable = true;
                }

                string justNumbers = new String(vehicle.Price.Where(Char.IsDigit).ToArray());
                existingVehicle.Price = Int32.Parse(justNumbers);


                var downloadedImages = _image.DownloadImagesFromURL(listOfImages);
                existingVehicle = _image.ImageBuilder(downloadedImages, existingVehicle);



                if (vehicle.Equipment != null)
                {
                    var listOfEquipment = vehicle.Equipment.Split(",").ToList();
                    existingVehicle.Equipment = listOfEquipment.Select(x => new Equipment(x, existingVehicle)).ToList();
                }
                else
                {
                    existingVehicle.Equipment = null;
                }
                vehiclesToRemove.Add(vehicle);

                _context.Entry(existingVehicle).State = EntityState.Modified;
                _context.Entry(existingVehicle).Property(x => x.UrlId).IsModified = false;   
            }
            _context.SaveChangesAsync();

            //removes already updated vehicles
            foreach (var item in vehiclesToRemove)
            {
                vehicles.Remove(item);
            }

            //Create new vehicles out of what's left in the list
            foreach (var vehicle in vehicles)
            {

                List<string> listOfImages = new List<string>();

                foreach (var image in vehicle.Images)
                {
                    listOfImages.Add(image.Url);
                }

                Vehicle newVehicle = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    RegistrationNumber = vehicle.RegistrationNumber.ToUpper(),
                    Colour = vehicle.Colour,
                    Created = DateTime.UtcNow,
                    FacilityId = vehicle.FacilityId,
                    Horsepowers = vehicle.Horsepowers,
                    Make = _context.Make.Where(x => x.Name == vehicle.Make).First(),
                    Mileage = Int32.Parse(vehicle.Milage),
                    Model = vehicle.Model,
                    ModelDescription = vehicle.ModelDescription,
                    Updated = DateTime.UtcNow,
                    Year = Int32.Parse(vehicle.Year),
                    Images = new List<Image>()

                };

                string justNumbers = new String(vehicle.Price.Where(Char.IsDigit).ToArray());
                newVehicle.Price = Int32.Parse(justNumbers);


                if (Enum.TryParse(vehicle.BodyType, out BodyType bodyType))
                {
                    newVehicle.BodyType = bodyType;
                }
                if (Enum.TryParse(vehicle.FuelType, out FuelType fuelType))
                {
                    newVehicle.FuelType = fuelType;
                }
                if (Enum.TryParse(vehicle.Gearbox, out Gearbox gearbox))
                {
                    newVehicle.Gearbox = gearbox;
                }



                if (vehicle.Leasable == "0" || vehicle.Leasable == "")
                {
                    newVehicle.Leasable = false;
                }
                else
                {
                    newVehicle.Leasable = true;
                }


                if (newVehicle.Mileage > 0)
                {
                    newVehicle.Used = true;
                }
                else
                {
                    newVehicle.Used = false;
                }

                var downloadedImages = _image.DownloadImagesFromURL(listOfImages);
                newVehicle = _image.ImageBuilder(downloadedImages, newVehicle);



                if (vehicle.Equipment != null)
                {
                    var listOfEquipment = vehicle.Equipment.Split(",").ToList();
                    newVehicle.Equipment = listOfEquipment.Select(x => new Equipment(x, newVehicle)).ToList();
                }
                else
                {
                    newVehicle.Equipment = null;
                }

                _context.Add(newVehicle);
                addedVehicles.Add(newVehicle.Id);
            }
            _context.SaveChangesAsync();

            _email.SendImportNotification(addedVehicles);

        }

        private DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

    }
}
