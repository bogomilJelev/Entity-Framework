using Artillery.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Artillery.Data.Models
{
    public class Gun
    {
        public Gun()
        {
            this.CountriesGuns = new HashSet<CountryGun>();
        }
        public int Id { get; set; }

        [ForeignKey(nameof(Manufacturer))]
        public int ManufacturerId  { get; set; }
        public Manufacturer Manufacturer { get; set; }
        [Required]
        [Range(100,350000)]
        public int GunWeight { get; set; }
        [Required]
        [Range(2.00,35.00)]
        public double BarrelLength  { get; set; }
        public int? NumberBuild  { get; set; }
        [Required]
        [Range(1,100000)]
        public int Range { get; set; }
        public GunType GunType  { get; set; }

        [ForeignKey(nameof(Shell))]
        [Required]
        public int ShellId { get; set; }
        [Required]
        public Shell Shell { get; set; }
        public ICollection<CountryGun> CountriesGuns  { get; set; }
    }
}
