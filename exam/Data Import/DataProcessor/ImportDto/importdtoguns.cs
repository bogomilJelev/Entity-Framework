using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Artillery.DataProcessor.ImportDto
{
    public class importdtoguns
    {
        [Required]

        public int ManufacturerId { get; set; }

        [Required]
        [Range(100, 1350000)]

        public int GunWeight { get; set; }

        [Required]
        [Range(2.00, 35.00)]

        public double BarrelLength { get; set; }

        public int? NumberBuild { get; set; }

        [Range(1, 100000)]

        public int Range { get; set; }

        [Required]

        public string GunType { get; set; }

        [Required]

        public int ShellId { get; set; }

        public importguncount[] Countries { get; set; }
    }
}
