using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlackSwanPlat.Controllers
{
    [Authorize(Policy = ("SellerPlicy"))]
    public class SellerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
