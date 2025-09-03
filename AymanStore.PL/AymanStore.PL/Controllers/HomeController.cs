using AutoMapper;
using AymanStore.BLL.Interfaces;
using AymanStore.BLL.Repositories;
using AymanStore.DAL.Entities;
using AymanStore.PL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AymanStore.PL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper Mapper;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IMapper Mapper)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
            this.Mapper = Mapper;

        }

        public IActionResult Index()
        {
            var data = new Index_VM();
            //var SubCategory = unitOfWork.SubCategoryTBLRepository.GetAllCustomized(
            //            filter: a => a.IsDeleted == false,
            //            includes: ue => ue.CategoryTBL);
            //data.SubCategoryTBL_VM = Mapper.Map<List<SubCategoryTBL_VM>>(SubCategory);

            //var Category = unitOfWork.CategoryTBLRepository.GetAllCustomized(
            //            filter: a => a.IsDeleted == false);


            data.ProductRatingTBL_VM = Mapper.Map<List<ProductRatingTBL_VM>>(unitOfWork.ProductRatingTBLRepository.GetAllCustomized(
                        filter: a => a.IsDeleted == false));

            var Product = unitOfWork.ProductTBLRepository.GetAllCustomized(
            filter: a => a.IsDeleted == false, orderBy: q => q.OrderByDescending(p => p.CreationDate), includes: new Expression<Func<ProductTBL, object>>[]
                    {
                                             p => p.SubCategoryTBL,
                                             p => p.SubCategoryTBL.CategoryTBL
                    }).Take(6);
            data.ProductTBL_VM = Mapper.Map<List<ProductTBL_VM>>(Product);

            return View(data);
        }

        public IActionResult Search(string? MainSearchWord, int? CategorySearchId)
        {
            if (!string.IsNullOrEmpty(MainSearchWord) && CategorySearchId > 0)
            {
                var Product = unitOfWork.ProductTBLRepository.GetAllCustomized(
                    filter: a => a.IsDeleted == false && a.SubCategoryTBL.CategoryTBL.ID == CategorySearchId && (a.Name.Contains(MainSearchWord) || a.Description.Contains(MainSearchWord)), orderBy: q => q.OrderByDescending(p => p.CreationDate),
                    includes: new Expression<Func<ProductTBL, object>>[]
                    {
                                             p => p.SubCategoryTBL,
                                             p => p.ManufacturerTBL
                    });
                return View(Mapper.Map<List<ProductTBL_VM>>(Product));
            }
            else if (!string.IsNullOrEmpty(MainSearchWord) && CategorySearchId == 0)
            {
                var Product = unitOfWork.ProductTBLRepository.GetAllCustomized(
                    filter: a => a.IsDeleted == false && (a.Name.Contains(MainSearchWord) || a.Description.Contains(MainSearchWord)), orderBy: q => q.OrderByDescending(p => p.CreationDate),
                    includes: new Expression<Func<ProductTBL, object>>[]
                    {
                                             p => p.SubCategoryTBL,
                                             p => p.ManufacturerTBL
                    });
                return View(Mapper.Map<List<ProductTBL_VM>>(Product));

            }
            return View(new List<ProductTBL_VM>());
        }

        public async Task<IActionResult> ProductDetail(int? ProductId)
        {
            if (ProductId == 0 || !unitOfWork.ProductTBLRepository.GetAllCustomized(
                filter: a => a.IsDeleted == false && a.ID == ProductId).Any())
               return RedirectToAction("Index","Home");

            var data = new ProductDetails_VM();
                var Product = unitOfWork.ProductTBLRepository.GetAllCustomized(
                filter: a => a.IsDeleted == false && a.ID == ProductId,
                includes: new Expression<Func<ProductTBL, object>>[]
                {
                                                        p => p.SubCategoryTBL.CategoryTBL,
                                                        p => p.SubCategoryTBL,
                                                        p => p.ManufacturerTBL,
                                                        p => p.SupplierTBL
                }).FirstOrDefault();
                if (Product != null) {
                    data.ProductTBL_VM = Mapper.Map<ProductTBL_VM>(Product);

                    var productPhotos = unitOfWork.ProductPhotoTBLRepository.GetAllCustomized(
                             filter: a => a.IsDeleted == false && a.ProductTBLId == ProductId, orderBy: q => q.OrderByDescending(p => p.CreationDate));
                    data.ProductPhotoTBL_VM = Mapper.Map<List<ProductPhotoTBL_VM>>(productPhotos);
                }

                   var fromCountryId = unitOfWork.CountryTBLRepository.GetAllCustomized(
                      filter: a => a.IsDeleted == false && a.ID == Product.CountryTBLPlaceId).FirstOrDefault().ID;

                  var IncomingcountryName = HttpContext.Session.GetString("currentUserCountry");

                  var shippingCompanyCost_shippingCountriesToThisUser = unitOfWork.ShippingCompanyCostTBLRepository.GetAllCustomized(
                    filter: a => a.IsDeleted == false
                    && a.ShippingCompanyTBL.CountryTBLId == fromCountryId, orderBy: q => q.OrderBy(p => p.Cost),
                    includes: new Expression<Func<ShippingCompanyCostTBL, object>>[]
                    {
                                                                            p => p.ShippingCompanyTBL,
                                                                            p => p.CountryTBLSendTo
                    });

                    var Countries = new List<CountryTBL>();
                    foreach (var item in shippingCompanyCost_shippingCountriesToThisUser.OrderBy(a => a.CountryTBLSendTo.Name))
                    {
                        Countries.Add(item.CountryTBLSendTo);
                    }

                    data.CountryTBL_VM = Mapper.Map<List<CountryTBL_VM>>(Countries);

                    if (IncomingcountryName == null)
                    {
                        string UserIdLogged = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        if(!string.IsNullOrEmpty(UserIdLogged))
                        IncomingcountryName = unitOfWork.UserManager.FindByIdAsync(UserIdLogged).Result.CountryTBL.Name;
                    }
                var countryName = unitOfWork.CountryTBLRepository.GetAllCustomized(
                      filter: a => a.IsDeleted == false && a.Name.Contains(IncomingcountryName)).Any();

                     if (countryName) 
                     { 

                     var toCountryId = unitOfWork.CountryTBLRepository.GetAllCustomized(
                      filter: a => a.IsDeleted == false && a.Name.Contains(IncomingcountryName)).FirstOrDefault().ID;
                 
                     data.ShippingCompanyCostTBL_VM = Mapper.Map<ShippingCompanyCostTBL_VM>(shippingCompanyCost_shippingCountriesToThisUser.Where(a=>a.CountryTBLSendToId == toCountryId).FirstOrDefault());
                     }

                var ProductRatingList = unitOfWork.ProductRatingTBLRepository.GetAllCustomized(
                       filter: a => a.IsDeleted == false && a.ProductTBLId == Product.ID, orderBy: q => q.OrderByDescending(p => p.CreationDate),
                 includes: new Expression<Func<ProductRatingTBL, object>>[]
                 {
                                                        p => p.AppUserWhoRated,
                 }); ;
                data.ProductRatingTBL_VM = Mapper.Map<List<ProductRatingTBL_VM>>(ProductRatingList);
                data.ProductRate = GetAverageRatingAsync(Product.ID);
                data.ProductSpecificationTBL_VM = Mapper.Map<List<ProductSpecificationTBL_VM>>(unitOfWork.ProductSpecificationTBLRepository.GetAllCustomized(
                             filter: a => a.IsDeleted == false && a.ProductTBLId == ProductId, orderBy: q => q.OrderByDescending(p => p.CreationDate)));
            
            return View(data);
        }

        [HttpPost]
        public IActionResult SaveCurrentUserCountry(string? currentUserCountry)
        {
            // Example: save to DB or log it
            if (currentUserCountry != null)
            {
                HttpContext.Session.SetString("currentUserCountry", currentUserCountry);
            }
            return Json(new { success = true});
        }

        public double GetAverageRatingAsync(int productId)
        {
            var ratings = unitOfWork.ProductRatingTBLRepository.GetAllCustomized(
                       filter: a => a.IsDeleted == false && a.ProductTBLId == productId);
            if (!ratings.Any())
                return 0.0; // No ratings yet

            return Math.Round(ratings.Average(r => r.Stars), 1); // Example: 4.2

        }


        public IActionResult ProductsByCategoryId(int? CategoryId)
        {
            if (CategoryId > 0)
            {
                var Products = unitOfWork.ProductTBLRepository.GetAllCustomized(
                    filter: a => a.IsDeleted == false && a.SubCategoryTBL.CategoryTBLId == CategoryId, orderBy: q => q.OrderByDescending(p => p.CreationDate),
                    includes: new Expression<Func<ProductTBL, object>>[]
                    {
                                             p => p.SubCategoryTBL.CategoryTBL,
                                             p => p.SubCategoryTBL,
                                             p => p.ManufacturerTBL
                    });
                if (Products.Count() > 0) {
                    return View(Mapper.Map<List<ProductTBL_VM>>(Products));
                }
            }
            ViewBag.CategoryName = unitOfWork.CategoryTBLRepository.GetAllCustomized(
                        filter: a => a.IsDeleted == false && a.ID == CategoryId).FirstOrDefault().Name ?? "NA";
            return View(new List<ProductTBL_VM>());
        }

        public IActionResult ProductsBySubCategoryId(int? SubCategoryId)
        {
            if (SubCategoryId > 0)
            {
                var Products = unitOfWork.ProductTBLRepository.GetAllCustomized(
                    filter: a => a.IsDeleted == false && a.SubCategoryTBL.ID == SubCategoryId, orderBy: q => q.OrderByDescending(p => p.CreationDate),
                    includes: new Expression<Func<ProductTBL, object>>[]
                    {
                                             p => p.SubCategoryTBL.CategoryTBL,
                                             p => p.SubCategoryTBL,
                                             p => p.ManufacturerTBL
                    });
                if (Products.Count() > 0)
                {
                    return View(Mapper.Map<List<ProductTBL_VM>>(Products));
                }
            }

            var SubCategory = unitOfWork.SubCategoryTBLRepository.GetAllCustomized(
                        filter: a => a.IsDeleted == false && a.ID == SubCategoryId,
                        includes: ue => ue.CategoryTBL).FirstOrDefault();
            if (SubCategory != null)
            {
                ViewBag.SubCategoryName = SubCategory.Name;
                ViewBag.CategoryName = SubCategory.CategoryTBL.Name;
            }


            return View(new List<ProductTBL_VM>());
        }


        public IActionResult ConditionsofUse()
        {
            return View();
        }

        public IActionResult PrivacyNotice()
        {
            return View();
        }

        public IActionResult HelpPage()
        {
            return View();
        }

        public IActionResult ShippingDetails(int countryId)
        {
            var Country = unitOfWork.CountryTBLRepository.GetAllCustomized(
                            filter: a => a.IsDeleted == false && a.ID == countryId).FirstOrDefault();
            return View();
        }

        public IActionResult ReportAbuseProductRating(int productReviewId)
        {
            var Country = unitOfWork.ProductRatingTBLRepository.GetAllCustomized(
                            filter: a => a.IsDeleted == false && a.ID == productReviewId).FirstOrDefault();
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
