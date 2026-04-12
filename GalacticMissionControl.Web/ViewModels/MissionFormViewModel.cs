using GalacticMissionControl.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GalacticMissionControl.Web.ViewModels;

public class MissionFormViewModel
{
    public Mission Mission { get; set; } = new();
    public IReadOnlyList<SelectListItem> StatusOptions { get; set; } = [];
    public IReadOnlyList<SelectListItem> ThreatLevelOptions { get; set; } = [];
    public IReadOnlyList<SelectListItem> PriorityOptions { get; set; } = [];
}
