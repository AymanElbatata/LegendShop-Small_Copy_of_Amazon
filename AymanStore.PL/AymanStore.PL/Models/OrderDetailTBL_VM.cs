using AymanStore.DAL.BaseEntity;
using AymanStore.DAL.Entities;

namespace AymanStore.PL.Models
{
    public class OrderDetailTBL_VM : BaseEntity<int>
    {
        public int? OrderTBLId { get; set; }
        public virtual OrderTBL? OrderTBL { get; set; }

        public int? ProductTBLId { get; set; }
        public virtual ProductTBL? ProductTBL { get; set; }

        public int? Price { get; set; }
        public int? Quantity { get; set; }
        public int? ShippingCost { get; set; }

    }
}
