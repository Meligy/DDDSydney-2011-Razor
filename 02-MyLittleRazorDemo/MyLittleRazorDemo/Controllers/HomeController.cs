using System.Linq;
using System.Web.Mvc;

namespace MyLittleRazorDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult Helpers_Functions()
        {
            return View();
        }

        public ActionResult Foreach()
        {
            var numbers = new[]
                              {
                                  "Oaane", "Twooo",
                                  "Threeyy", "Fo_r",
                                  "Five", "Six",
                                  "Sefen", "Eight",
                                  "Nayne", "Teaaaaaan"
                              };

            return View(numbers);
        }
    }
}
