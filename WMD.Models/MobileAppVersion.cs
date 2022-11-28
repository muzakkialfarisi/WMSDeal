using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class MobileAppVersion
    {
        [Key]
        public Guid Id { get; set; }

        public string Version { get; set; }

        public string MinVersion { get; set; }

        public string Link { get; set; }

        public string Device { get; set; }

        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public FlagEnum Flag { get; set; } = FlagEnum.Active;
    }
}