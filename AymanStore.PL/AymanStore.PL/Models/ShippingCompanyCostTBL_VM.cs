using AymanStore.DAL.BaseEntity;
using AymanStore.DAL.Entities;

namespace AymanStore.PL.Models
{
    public class ShippingCompanyCostTBL_VM : BaseEntity<int>
    {
        public int? CountryTBLSendToId { get; set; }
        public virtual CountryTBL? CountryTBLSendTo { get; set; }

        public int? Cost { get; set; } = 0;
    }
}
