using Microsoft.AspNetCore.Mvc;

namespace BlackSwanPlat.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            if (statusCode == 404)
            {
        
                return View();
            }
            else if (statusCode == 500)
            {
                return View();
            } 
            else if (statusCode == 301)
            {
                return View();
            }
             else if (statusCode == 100)
            {
                return View("Continue To Submit The application.");
            }

            return View("UnKnown Error.");
        }
    }
}
