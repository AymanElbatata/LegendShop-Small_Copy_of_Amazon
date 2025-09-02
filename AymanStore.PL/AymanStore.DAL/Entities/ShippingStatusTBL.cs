﻿using AymanStore.DAL.BaseEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AymanStore.DAL.Entities
{
    public class ShippingStatusTBL : BaseEntity<int>
    {
        public string? Name { get; set; } = null!;
        public string? Image { get; set; } = null!;
    }
}
