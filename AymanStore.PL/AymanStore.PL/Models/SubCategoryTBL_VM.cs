using AymanStore.DAL.BaseEntity;
using AymanStore.DAL.Entities;

namespace AymanStore.PL.Models
{
    public class SubCategoryTBL_VM : BaseEntity<int>
    {
        public int? CategoryTBLId { get; set; }
        public string? Name { get; set; } = null!;
        public string? Image { get; set; } = null!;
        public string? Description { get; set; } = null!;

        public virtual CategoryTBL? CategoryTBL { get; set; }
    }
}
