using AutoMapper;
using AymanStore.BLL.Interfaces;
using AymanStore.DAL.BaseEntity;
using AymanStore.DAL.Entities;
using AymanStore.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AymanStore.PL.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper Mapper;
        private readonly IConfiguration configuration;

        public UserController(ILogger<UserController> logger, IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
            Mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Orders & Order Details
        public IActionResult Orders()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var data = new Orders_VM();

            var Orders = unitOfWork.OrderTBLRepository.GetAllCustomized(
                    filter: a => a.IsDeleted == false && a.OrderByUserTBLId == userId && a.IsСurrentOrder == false,
                    includes: new Expression<Func<OrderTBL, object>>[]
                    {
                                p => p.OrderByUserTBL,
                                p => p.CountryTBL,
                                p => p.ShippingServiceTBL,
                                p => p.ShippingStatusTBL,
                                p => p.ShippingCompanyTBL

                   });
            if (Orders != null)
            {
                data.OrderTBL_VM = Mapper.Map<List<OrderTBL_VM>>(Orders.OrderByDescending(p => p.CreationDate));
                data.OrderDetailTBL_VM = Mapper.Map<List<OrderDetailTBL_VM>>(GetCurrentUserOrderDetails().OrderByDescending(p => p.CreationDate));
                return View(data);
            }
            return View(data);
        }

        public IActionResult OrderDetail(int? orderId)
        {
            var data = new ShoppingCart_VM();

            if (orderId > 0 || GetUserOrder(Convert.ToInt32(orderId)) != null)
            { 
            var CurrentOrder = GetUserOrder(Convert.ToInt32(orderId));
            data.OrderTBL_VM = Mapper.Map<OrderTBL_VM>(CurrentOrder);
            data.OrderDetailTBL_VM = Mapper.Map<List<OrderDetailTBL_VM>>(GetCompletedUserOrderDetails(CurrentOrder.ID));
                var vv = 0;
            }
            return View(data);
        }
        #endregion

        #region ShoppingCart + Add-Update-Delete
        public IActionResult ShoppingCart()
        {
            var data = new ShoppingCart_VM();

            var CurrentOrder = GetCurrentUserOrder();
            if (CurrentOrder != null)
            {
                data.OrderTBL_VM = Mapper.Map<OrderTBL_VM>(CurrentOrder);
                data.OrderDetailTBL_VM = Mapper.Map<List<OrderDetailTBL_VM>>(GetCurrentUserOrderDetails(CurrentOrder.ID));
            }
            return View(data);
        }

        public async Task<IActionResult> AddToCart(int? productIdAddToCart, int? quantityAddToCart)
        {
            if (unitOfWork.ProductTBLRepository.GetAllCustomized(
                     filter: a => a.IsDeleted == false && a.ID == productIdAddToCart).Any())
            {
                var CurrentOrder = GetCurrentUserOrder();
                if (CurrentOrder == null)
                {
                    await AddNewOrderForCurrentUser();
                    CurrentOrder = GetCurrentUserOrder();
                }
                if (!GetCurrentUserOrderDetails(CurrentOrder.ID).Where(a => a.ProductTBLId == productIdAddToCart).Any())
                {
                    var newOrder = new OrderDetailTBL();
                    newOrder.ProductTBLId = productIdAddToCart;
                    newOrder.Quantity = quantityAddToCart;
                    newOrder.Price = unitOfWork.ProductTBLRepository.GetById(newOrder.ProductTBLId).SellingPrice;
                    newOrder.OrderTBLId = CurrentOrder.ID;
                    newOrder.ShippingCost = GetShippingCompanyCost(Convert.ToInt32(newOrder.ProductTBLId), Convert.ToInt32(CurrentOrder.CountryTBLId)).Cost;
                    unitOfWork.OrderDetailTBLRepository.Add(newOrder);
                }

            }
            return RedirectToAction("ShoppingCart", "User");
        }


        public IActionResult UpdateQuantityCart(int? productId, int? productQuantity)
        {
            var CurrentOrder = GetCurrentUserOrder();
            if (CurrentOrder != null)
            {
              var UpdatedOrderDetail = GetCurrentUserOrderDetails(CurrentOrder.ID).Where(a => a.ProductTBLId == productId).FirstOrDefault();
                if (UpdatedOrderDetail != null)
                {
                    UpdatedOrderDetail.Quantity = productQuantity;
                    unitOfWork.OrderDetailTBLRepository.Update(UpdatedOrderDetail);
                }
            }
            return RedirectToAction("ShoppingCart", "User");
        }

        public IActionResult RemoveFromCart(int? productId)
        {

            var CurrentOrder = GetCurrentUserOrder();
            if (CurrentOrder != null)
            {
                var UpdatedOrderDetail = GetCurrentUserOrderDetails(CurrentOrder.ID).Where(a=>a.ProductTBLId == productId).FirstOrDefault();
                if (UpdatedOrderDetail != null)
                {
                    UpdatedOrderDetail.IsDeleted = true;
                    unitOfWork.OrderDetailTBLRepository.Update(UpdatedOrderDetail);
                }
            }
            return RedirectToAction("ShoppingCart", "User");
        }
        #endregion

        #region CheckOut
        public IActionResult CheckOutOrder()
        {
            var data = new CheckOut_VM();

            var CurrentOrder = GetCurrentUserOrder();
            if (CurrentOrder != null)
            {
                data.OrderTBL_VM = Mapper.Map<OrderTBL_VM>(CurrentOrder);
                data.CountryTBL_VM = Mapper.Map<List<CountryTBL_VM>>(unitOfWork.CountryTBLRepository
                    .GetAllCustomized(filter: a => a.IsDeleted == false));
                data.ShippingServiceTBL_VM = Mapper.Map<List<ShippingServiceTBL_VM>>(unitOfWork.ShippingServiceTBLRepository
                     .GetAllCustomized(filter: a => a.IsDeleted == false));
            }
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> CheckOutOrder(CheckOut_VM model)
        {
            try
            {
            var CurrentOrder = GetCurrentUserOrder();
            if (CurrentOrder != null)
            {
                var OrderDetailsShipping = GetCurrentUserOrderDetails(CurrentOrder.ID);
                var ShippingCompanyCost = GetShippingCompanyCost(Convert.ToInt32(OrderDetailsShipping.FirstOrDefault().ProductTBLId), Convert.ToInt32(model.OrderTBL_VM.CountryTBLId));
                if (ShippingCompanyCost != null)
                    CurrentOrder.ShippingCompanyTBLId = ShippingCompanyCost.ShippingCompanyTBLId;
                else
                {
                    ModelState.AddModelError("", "One or more products cannot be shipped for you because the country of shipping, please select products can be shipped for your country!");
                    model.CountryTBL_VM = Mapper.Map<List<CountryTBL_VM>>(unitOfWork.CountryTBLRepository
                          .GetAllCustomized(filter: a => a.IsDeleted == false));
                    model.ShippingServiceTBL_VM = Mapper.Map<List<ShippingServiceTBL_VM>>(unitOfWork.ShippingServiceTBLRepository
                         .GetAllCustomized(filter: a => a.IsDeleted == false));

                    return View(model);
                }

                CurrentOrder.Phones = model.OrderTBL_VM.Phones;
                CurrentOrder.Address = model.OrderTBL_VM.Address;
                CurrentOrder.ShippingStatusTBLId = 1;
                CurrentOrder.ShippingServiceTBLId = model.OrderTBL_VM.ShippingServiceTBLId;
                CurrentOrder.CountryTBLId = model.OrderTBL_VM.CountryTBLId;
                CurrentOrder.IsСurrentOrder = false;
                CurrentOrder.DateOfSubmit = DateTime.Now;

                int OrderDetailAndQuantity = 0;
                int OrderDetailShipping = 0;

                List<ConfirmationOrder_VM> items = new List<ConfirmationOrder_VM>();

                foreach (var item in OrderDetailsShipping)
                {
                    OrderDetailAndQuantity += Convert.ToInt32(item.Price * item.Quantity);
                    OrderDetailShipping += Convert.ToInt32(item.Quantity * item.ShippingCost);
                    items.Add(new ConfirmationOrder_VM
                    {
                        ItemName = item.ProductTBL.Name,
                        Barcode = item.ProductTBL.Barcode,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Shipping = item.Quantity * item.ShippingCost
                    });
                }
                // Calculate Total Amount
                //
                CurrentOrder.TotalAmount = OrderDetailAndQuantity + OrderDetailShipping;
                unitOfWork.OrderTBLRepository.Update(CurrentOrder);

                    try
                    {
                        // Sending Email               
                        var ActivateLink = configuration["AymanStore.Pl.Url"] + "User/OrderDetail?orderId=" + CurrentOrder.ID;

                        var Email = new EmailTBL_VM();
                        Email.To = CurrentOrder.OrderByUserTBL.Email;
                        Email.Subject = configuration["AymanStore.Pl.Name"] + " - Order Confirmation";
                        Email.Body = await GetOrderConfirmationTemplateAsync(GetCurrentUser().Result.FirstName?? "", CurrentOrder.HashCode?? "", (DateTime)CurrentOrder.DateOfSubmit, BuildOrderItemsTable(items), CurrentOrder.TotalAmount.ToString()?? "", ActivateLink);
                        var newEmail = Mapper.Map<EmailTBL>(Email);

                        // Send email
                        await unitOfWork.EmailTBLRepository.SendEmailAsync(newEmail,2);
                        // Save Email
                        unitOfWork.EmailTBLRepository.Add(newEmail);

                        newEmail.ID = 0;
                        newEmail.To = OrderDetailsShipping.FirstOrDefault().ProductTBL.SupplierTBL.Email;
                        await unitOfWork.EmailTBLRepository.SendEmailAsync(newEmail,2);
                        unitOfWork.EmailTBLRepository.Add(newEmail);
                        // Ending Sending Emails
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.AppErrorTBLRepository.Add(new AppErrorTBL
                        {
                            Message = ex.Message,
                            StackTrace = ex.StackTrace ?? "",
                            Controller = nameof(UserController),
                            Action = nameof(CheckOutOrder)
                        });
                    }

                return RedirectToAction("Orders", "User");
            }
            }
            catch (Exception ex)
            {
                // Log to console / file
                _logger.LogError(ex, "Error in {Controller}.{Action}", nameof(UserController), nameof(CheckOutOrder));

                // Save to DB
                unitOfWork.AppErrorTBLRepository.Add(new AppErrorTBL
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace ?? "",
                    Controller = nameof(UserController),
                    Action = nameof(CheckOutOrder)
                });
                System.IO.File.AppendAllText("C:\\Temp\\FallbackLog.txt", ex.ToString());

            }
            return View(model);
        }
        #endregion

        #region Helper Methods
        private OrderTBL GetUserOrder(int orderId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return unitOfWork.OrderTBLRepository.GetAllCustomized(
                  filter: a => a.IsDeleted == false && a.ID == orderId && a.IsСurrentOrder == false,
                   includes: new Expression<Func<OrderTBL, object>>[]
                      {
                                p => p.OrderByUserTBL,
                                p => p.CountryTBL,
                                p => p.ShippingServiceTBL,
                                p => p.ShippingStatusTBL,
                                p => p.ShippingCompanyTBL

                     }).FirstOrDefault();
        }
        private OrderTBL GetCurrentUserOrder()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return unitOfWork.OrderTBLRepository.GetAllCustomized(
                  filter: a => a.IsDeleted == false && a.OrderByUserTBLId == userId && a.IsСurrentOrder == true,
                   includes: new Expression<Func<OrderTBL, object>>[]
                      {
                                p => p.OrderByUserTBL,
                                p => p.CountryTBL,
                                p => p.ShippingServiceTBL,
                                p => p.ShippingStatusTBL,
                                p => p.ShippingCompanyTBL

                     }).FirstOrDefault();
        }

        private List<OrderDetailTBL> GetCurrentUserOrderDetails()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return unitOfWork.OrderDetailTBLRepository.GetAllCustomized(
                filter: a => a.IsDeleted == false && a.OrderTBL.OrderByUserTBL.Id == userId && a.OrderTBL.IsСurrentOrder == false,
                includes: new Expression<Func<OrderDetailTBL, object>>[]
                {
                                        p => p.OrderTBL,
                                        p => p.ProductTBL

                }).OrderByDescending(p => p.CreationDate).ToList();
        }

        private List<OrderDetailTBL> GetCurrentUserOrderDetails(int orderId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return unitOfWork.OrderDetailTBLRepository.GetAllCustomized(
                filter: a => a.IsDeleted == false && a.OrderTBL.OrderByUserTBL.Id == userId && a.OrderTBL.IsСurrentOrder == true && a.OrderTBLId == orderId,
                includes: new Expression<Func<OrderDetailTBL, object>>[]
                {
                                        p => p.OrderTBL,
                                        p => p.ProductTBL,

                }).OrderByDescending(p => p.CreationDate).ToList();
        }

        private List<OrderDetailTBL> GetCompletedUserOrderDetails(int orderId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return unitOfWork.OrderDetailTBLRepository.GetAllCustomized(
                filter: a => a.IsDeleted == false && a.OrderTBL.OrderByUserTBL.Id == userId && a.OrderTBLId == orderId,
                includes: new Expression<Func<OrderDetailTBL, object>>[]
                {
                                        p => p.OrderTBL,
                                        p => p.ProductTBL,

                }).OrderByDescending(p => p.CreationDate).ToList();
        }

        private async Task AddNewOrderForCurrentUser()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var user = unitOfWork.UserManager.FindByIdAsync(userId);
            unitOfWork.OrderTBLRepository.Add(new OrderTBL() { IsСurrentOrder = true, OrderByUserTBLId = userId, HashCode= unitOfWork.MySPECIALGUID.GetUniqueKey(15), CountryTBLId = GetCurrentUser().Result.CountryTBLId });
        }

        private async Task<AppUser> GetCurrentUser()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await unitOfWork.UserManager.FindByIdAsync(userId);
        }

        private ShippingCompanyCostTBL GetShippingCompanyCost(int productId, int countryTo)
        {
            var ProductFrom = unitOfWork.ProductTBLRepository.GetById(productId).CountryTBLPlaceId;
            return unitOfWork.ShippingCompanyCostTBLRepository
                 .GetAllCustomized(filter: a => a.IsDeleted == false && a.ShippingCompanyTBL.CountryTBLId == ProductFrom && a.CountryTBLSendToId == countryTo).FirstOrDefault();
        }


        private async Task<string> GetOrderConfirmationTemplateAsync(string firstName, string orderNumber, DateTime orderDate, string orderItemsHtml,string totalPrice,string orderLink)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "template2", "Order-Confirmation.html");
            string html = await System.IO.File.ReadAllTextAsync(templatePath);

            html = html.Replace("{{FirstName}}", firstName);
            html = html.Replace("{{OrderNumber}}", orderNumber);
            html = html.Replace("{{OrderDate}}", orderDate.ToString("MMMM dd, yyyy"));
            html = html.Replace("{{OrderItems}}", orderItemsHtml);
            html = html.Replace("{{TotalPrice}}", totalPrice);
            html = html.Replace("{{OrderLink}}", orderLink);
            html = html.Replace("{{Year}}", DateTime.Now.Year.ToString());

            return html;
        }

        private string BuildOrderItemsTable(List<ConfirmationOrder_VM> items)
        {
            if (items == null || items.Count == 0)
                return "<tr><th><td>No items in the order.</td></th></tr>";

            var sb = new StringBuilder();
            //sb.Append("<table style='width:100%; border-collapse:collapse;'>");
            //sb.Append("<tr><th>Item</th><th>Qty</th><th>Price</th><th>Shipping</th></tr>");

            foreach (var item in items)
            {
                sb.Append($@"
            <tr>
                <td>{item.ItemName}</td>
                <td>{item.Barcode}</td>
                <td>{item.Quantity}</td>
                <td>${item.Price:F2}</td>
                <td>${item.Shipping:F2}</td>
            </tr>");
            }

            sb.Append("</table>");
            return sb.ToString();
        }
        #endregion


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
