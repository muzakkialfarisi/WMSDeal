﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Models.Incoming
{
    public class DeliveryOrder
    {
        [Key]
        public string DoNumber { get; set; }
        public string TenantId { get; set; }
        public string Status { get; set; }
        public DateTime DateDelivered { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string KodePos { get; set; }
        public string ProName { get; set; }
        public string ProfileImageUrl { get; set; }
        public int Count { get; set; }
        public virtual List<ProductData> ProductData { get; set; } = new List<ProductData>();
    }
}
