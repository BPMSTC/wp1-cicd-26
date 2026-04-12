using GalacticMissionControl.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace GalacticMissionControl.Web.Controllers;

public class MissionController : Controller
{
    public IActionResult Index()
    {
        var missions = new List<MissionSummary>
        {
            new("ARGO-1", "Mars Orbital Survey", "En Route"),
            new("LUNA-3", "Lunar Relay Deployment", "On Schedule"),
            new("TITAN-7", "Atmospheric Sampling", "Planning")
        };

        return View(missions);
    }
}
