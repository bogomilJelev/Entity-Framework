namespace Artillery.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Artillery.Data;
    using Artillery.Data.Models;
    using Artillery.Data.Models.Enums;
    using Artillery.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage =
                "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();
            XmlRootAttribute root = new XmlRootAttribute("Countries");
            XmlSerializer xmlserial = new XmlSerializer(typeof(importCountriesxml[]), root);
            using StringReader sr = new StringReader(xmlString);
            importCountriesxml[] countryesdto = (importCountriesxml[])xmlserial.Deserialize(sr);

            HashSet<Country> country = new HashSet<Country>();
            foreach (var countrydto in countryesdto)
            {
                if (!IsValid(countrydto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Country c = new Country
                {
                    CountryName = countrydto.CountryName,
                    ArmySize = countrydto.ArmySize
                };
                country.Add(c);
                sb.AppendLine(string.Format(SuccessfulImportCountry, countrydto.CountryName, countrydto.ArmySize));
            }
            context.Countries.AddRange(country);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Manufacturers");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(importmanuxml[]), xmlRoot);

            using StringReader sr = new StringReader(xmlString);

            importmanuxml[] manuesdto = (importmanuxml[])xmlSerializer.Deserialize(sr);

            HashSet<Manufacturer> manufact = new HashSet<Manufacturer>();



            foreach (importmanuxml manue in manuesdto)
            {
                if (!IsValid(manue))
                {
                    sb.AppendLine("Invalid data.");
                    continue;
                }

                var ask = manufact.FirstOrDefault(x => x.ManufacturerName == manue.ManufacturerName);

                if (ask == null)
                {
                    Manufacturer m = new Manufacturer
                    {
                        ManufacturerName = manue.ManufacturerName,
                        Founded = manue.Founded
                    };

                    manufact.Add(m);
                    sb.AppendLine($"Successfully import manufacturer {m.ManufacturerName} founded in {m.Founded}.");
                }
                else
                {
                    sb.AppendLine("Invalid data.");
                    continue;
                }

                context.Manufacturers.AddRange(manufact);
                context.SaveChanges();

            }
                return sb.ToString().TrimEnd();


        }
    
    

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();
            XmlRootAttribute root = new XmlRootAttribute("Shells");
            XmlSerializer xmlserial = new XmlSerializer(typeof(importshelldto[]), root);
            using StringReader sr = new StringReader(xmlString);
            importshelldto[] sheldto = (importshelldto[])xmlserial.Deserialize(sr);
            HashSet<Shell> shels = new HashSet<Shell>();
            foreach (var item in sheldto)
            {
                if (!IsValid(item))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Shell s = new Shell
                {
                    ShellWeight = item.ShellWeight,
                    Caliber = item.Caliber,
                };
                shels.Add(s);
                sb.AppendLine(string.Format(SuccessfulImportShell, s.Caliber, s.ShellWeight));
            }
            context.Shells.AddRange(shels);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
                var sb = new StringBuilder();

                var gunsdtos = JsonConvert.DeserializeObject<importdtoguns[]>(jsonString);

                List<Gun> guns = new List<Gun>();

                foreach (importdtoguns gundto in gunsdtos)
                {
                    if (!IsValid(gundto))
                    {
                        sb.AppendLine("Invalid data.");
                        continue;
                    }

                    bool isvaldigun = Enum.TryParse(typeof(GunType), gundto.GunType, out object gunout);

                    if (!isvaldigun)
                    {
                        sb.AppendLine("Invalid data.");
                        continue;
                    }

                    Gun gun = new Gun
                    {
                        ManufacturerId = gundto.ManufacturerId,
                        GunWeight = gundto.GunWeight,
                        BarrelLength = gundto.BarrelLength,
                        NumberBuild = gundto.NumberBuild,
                        Range = gundto.Range,
                        GunType = (GunType)gunout,
                        ShellId = gundto.ShellId
                    };

                    foreach (importguncount countrygun in gundto.Countries)
                    {
                        var country = context.Countries.FirstOrDefault(x => x.Id == countrygun.Id);

                        CountryGun countrygunn = new CountryGun
                        {
                            Gun = gun,
                            Country = country
                        };

                        gun.CountriesGuns.Add(countrygunn);
                    }

                    guns.Add(gun);
                    sb.AppendLine($"Successfully import gun {gun.GunType} with a total weight of {gun.GunWeight} kg. and barrel length of {gun.BarrelLength} m.");
                }

                context.Guns.AddRange(guns);
                context.SaveChanges();
                return sb.ToString().TrimEnd();
            }
        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
