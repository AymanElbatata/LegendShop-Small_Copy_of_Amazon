using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AymanStore.DAL.BaseEntity;
using AymanStore.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AymanStore.DAL.Contexts
{
    public class AymanStoreDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AymanStoreDbContext()
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }
        public AymanStoreDbContext(DbContextOptions<AymanStoreDbContext> options) : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseSqlServer("server=.; database= DemoDb; integrated security= true;");

        public DbSet<CategoryTBL> CategoryTBLs { get; set; }
        public DbSet<SubCategoryTBL> SubCategoryTBLs { get; set; }
        public DbSet<GenderTBL> GenderTBLs { get; set; }
        public DbSet<CountryTBL> CountryTBLs { get; set; }
        public DbSet<ProductTBL> ProductTBLs { get; set; }
        public DbSet<ProductPhotoTBL> ProductPhotoTBLs { get; set; }
        public DbSet<ProductRatingTBL> ProductRatingTBLs { get; set; }
        public DbSet<ProductSpecificationTBL> ProductSpecificationTBLs { get; set; }
        public DbSet<ManufacturerTBL> ManufacturerTBLs { get; set; }
        public DbSet<ShippingServiceTBL> ShippingServiceTBLs { get; set; }
        public DbSet<OrderTBL> OrderTBLs { get; set; }
        public DbSet<OrderDetailTBL> OrderDetailTBLs { get; set; }
        public DbSet<ShippingStatusTBL> ShippingStatusTBLs { get; set; }
        public DbSet<ShippingCompanyTBL> ShippingCompanyTBLs { get; set; }
        public DbSet<ShippingCompanyCostTBL> ShippingCompanyCostTBLs { get; set; }
        public DbSet<EmailTBL> EmailTBLs { get; set; }
        public DbSet<AppErrorTBL> AppErrorTBLs { get; set; }
    }
}
