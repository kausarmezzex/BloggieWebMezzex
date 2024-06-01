using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BaseController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
