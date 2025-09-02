using AymanStore.BLL.Repositories;
using AymanStore.DAL.BaseEntity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AymanStore.BLL.Interfaces
{
    public interface IUnitOfWork
    {
        ICountryTBLRepository CountryTBLRepository { get; }
        IGenderTBLRepository GenderTBLRepository { get; }
        IProductTBLRepository ProductTBLRepository { get; }
        IProductPhotoTBLRepository ProductPhotoTBLRepository { get; }
        IProductRatingTBLRepository ProductRatingTBLRepository { get; }
        IProductSpecificationTBLRepository ProductSpecificationTBLRepository { get; }
        ICategoryTBLRepository CategoryTBLRepository { get; }
        ISubCategoryTBLRepository SubCategoryTBLRepository { get; }
        IEmailTBLRepository EmailTBLRepository { get; }
        IShippingStatusTBLRepository ShippingStatusTBLRepository { get; }
        IShippingCompanyTBLRepository ShippingCompanyTBLRepository { get; }
        IShippingCompanyCostTBLRepository ShippingCompanyCostTBLRepository { get; }
        IShippingServiceTBLRepository ShippingServiceTBLRepository { get; }
        IOrderTBLRepository OrderTBLRepository { get; }
        IOrderDetailTBLRepository OrderDetailTBLRepository { get; }
        IAppErrorTBLRepository AppErrorTBLRepository { get; }
        SignInManager<AppUser> SignInManager { get; }
        RoleManager<AppRole> RoleManager { get; }
        UserManager<AppUser> UserManager { get; }
        IMySPECIALGUID MySPECIALGUID { get; }
        IGeoLocationHelper GeoLocationHelper { get; }
    }
}
