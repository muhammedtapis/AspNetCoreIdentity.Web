using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentity.Web.Controllers
{
    public class OrderController : Controller
    {
        //stock read order create catalog delete yapacak bir action istiyoruz o yüzden oluşturduk bu controllerı ve program cs te policy ekleyeceğiz.

        //[Authorize(Policy = "StockDeleteOrderDeleteAndReadPermissionPolicy")]

        [Authorize(Policy = "Permission.Order.ReadPolicy")] //sadece bu policy sahip olanlar bu sayfayı görüntüleyebilir.
        public IActionResult Index()
        {
            return View();
        }
    }
}