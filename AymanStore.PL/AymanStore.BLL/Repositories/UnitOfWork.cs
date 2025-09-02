using AymanStore.BLL.Interfaces;
using AymanStore.DAL.BaseEntity;
using Microsoft.AspNetCore.Identity;

namespace AymanStore.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IGenderTBLRepository GenderTBLRepository { get; }
        public ICountryTBLRepository CountryTBLRepository { get; }
        public IProductTBLRepository ProductTBLRepository { get; }
        public IProductPhotoTBLRepository ProductPhotoTBLRepository { get; }
        public IProductRatingTBLRepository ProductRatingTBLRepository { get; }
        public IProductSpecificationTBLRepository ProductSpecificationTBLRepository { get; }
        public ICategoryTBLRepository CategoryTBLRepository { get; }
        public ISubCategoryTBLRepository SubCategoryTBLRepository { get; }
        public IEmailTBLRepository EmailTBLRepository { get; }
        public IShippingStatusTBLRepository ShippingStatusTBLRepository { get; }
        public IShippingCompanyTBLRepository ShippingCompanyTBLRepository { get; }
        public IShippingCompanyCostTBLRepository ShippingCompanyCostTBLRepository { get; }
        public IShippingServiceTBLRepository ShippingServiceTBLRepository { get; }
        public IOrderTBLRepository OrderTBLRepository { get; }
        public IOrderDetailTBLRepository OrderDetailTBLRepository { get; }
        public IAppErrorTBLRepository AppErrorTBLRepository { get; }
        public SignInManager<AppUser> SignInManager { get; }
        public RoleManager<AppRole> RoleManager { get; }
        public UserManager<AppUser> UserManager { get; }
        public IMySPECIALGUID MySPECIALGUID { get; }
        public IGeoLocationHelper GeoLocationHelper { get; }


        public UnitOfWork( IGenderTBLRepository GenderTBLRepository
            ,ICountryTBLRepository CountryTBLRepository,
            IProductTBLRepository ProductTBLRepository,
            IProductPhotoTBLRepository ProductPhotoTBLRepository,
            IProductRatingTBLRepository ProductRatingTBLRepository,
            IProductSpecificationTBLRepository ProductSpecificationTBLRepository,
            ICategoryTBLRepository CategoryTBLRepository,
            ISubCategoryTBLRepository SubCategoryTBLRepository,
            IEmailTBLRepository EmailTBLRepository,
            IShippingStatusTBLRepository ShippingStatusTBLRepository,
            IShippingCompanyTBLRepository ShippingCompanyTBLRepository,
            IShippingCompanyCostTBLRepository ShippingCompanyCostTBLRepository,
            IShippingServiceTBLRepository ShippingServiceTBLRepository,
            IOrderTBLRepository OrderTBLRepository,
            IOrderDetailTBLRepository OrderDetailTBLRepository,
            IAppErrorTBLRepository AppErrorTBLRepository,

            SignInManager<AppUser> SignInManager,
            RoleManager<AppRole> RoleManager, UserManager<AppUser> UserManager,
            IMySPECIALGUID MySPECIALGUID,
            IGeoLocationHelper GeoLocationHelper
            )
        {
            this.CountryTBLRepository = CountryTBLRepository;
            this.GenderTBLRepository = GenderTBLRepository;
            this.ProductTBLRepository = ProductTBLRepository;
            this.ProductPhotoTBLRepository = ProductPhotoTBLRepository;
            this.ProductRatingTBLRepository = ProductRatingTBLRepository;
            this.ProductSpecificationTBLRepository = ProductSpecificationTBLRepository;
            this.CategoryTBLRepository = CategoryTBLRepository;
            this.SubCategoryTBLRepository = SubCategoryTBLRepository;
            this.EmailTBLRepository = EmailTBLRepository;
            this.ShippingStatusTBLRepository = ShippingStatusTBLRepository;
            this.ShippingCompanyTBLRepository = ShippingCompanyTBLRepository;
            this.ShippingCompanyCostTBLRepository = ShippingCompanyCostTBLRepository;
            this.ShippingServiceTBLRepository = ShippingServiceTBLRepository;
            this.OrderTBLRepository = OrderTBLRepository;
            this.OrderDetailTBLRepository = OrderDetailTBLRepository;
            this.AppErrorTBLRepository = AppErrorTBLRepository;
            this.SignInManager = SignInManager;
            this.RoleManager = RoleManager;
            this.UserManager = UserManager;
            this.MySPECIALGUID = MySPECIALGUID;
            this.GeoLocationHelper = GeoLocationHelper;
        }
    }
}
