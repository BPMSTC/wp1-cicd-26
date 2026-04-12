using Microsoft.AspNetCore.Mvc;

namespace GalacticMissionControl.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
