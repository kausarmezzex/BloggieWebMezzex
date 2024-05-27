using Microsoft.AspNetCore.Mvc;

namespace Bloggie.web.Controllers
{
    public class ErrorController : Controller
    {
        // Handle 404 errors
        [Route("Error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        // Handle other errors
        [Route("Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            ViewBag.StatusCode = statusCode;
            return View();
        }
    }
}
