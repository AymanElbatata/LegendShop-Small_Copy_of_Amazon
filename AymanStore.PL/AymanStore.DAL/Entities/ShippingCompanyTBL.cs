using AymanStore.DAL.BaseEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AymanStore.DAL.Entities
{
    public class ShippingCompanyTBL : BaseEntity<int>
    {
        public int? CountryTBLId { get; set; }
        public virtual CountryTBL? CountryTBL { get; set; }

        public string? Name { get; set; } = null!;
        public string? Image { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public string? Contact { get; set; } = null!;
    }
}
