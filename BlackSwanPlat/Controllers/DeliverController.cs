using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlackSwanPlat.Controllers
{
    [Authorize(Policy = ("DeliverPolicy"))]
    public class DeliverController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
