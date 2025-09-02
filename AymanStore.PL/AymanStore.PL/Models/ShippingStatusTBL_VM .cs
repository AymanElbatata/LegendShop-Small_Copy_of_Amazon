using AymanStore.DAL.BaseEntity;
using AymanStore.DAL.Entities;

namespace AymanStore.PL.Models
{
    public class ShippingStatusTBL_VM : BaseEntity<int>
    {
        public string? Name { get; set; } = null!;
        public string? Image { get; set; } = null!;
    }
}
