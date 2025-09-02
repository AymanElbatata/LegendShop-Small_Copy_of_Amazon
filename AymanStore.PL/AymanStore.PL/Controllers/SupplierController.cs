using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AymanStore.PL.Controllers
{
    [Authorize(Roles = "Supplier")]
    public class SupplierController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
