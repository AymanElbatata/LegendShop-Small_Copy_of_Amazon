using AymanStore.DAL.BaseEntity;
using AymanStore.DAL.Entities;

namespace AymanStore.PL.Models
{
    public class ShippingCompanyTBL_VM : BaseEntity<int>
    {
        public int? CountryTBLId { get; set; }
        public virtual CountryTBL? CountryTBL { get; set; }

        public string? Name { get; set; } = null!;
        public string? Image { get; set; } = null!;
        public string? Contact { get; set; } = null!;
    }
}
