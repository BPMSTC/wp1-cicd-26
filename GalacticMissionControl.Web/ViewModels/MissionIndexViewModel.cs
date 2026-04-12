using GalacticMissionControl.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GalacticMissionControl.Web.ViewModels;

public class MissionIndexViewModel
{
    public IReadOnlyList<Mission> Missions { get; set; } = [];
    public string? SelectedStatus { get; set; }
    public string? SelectedThreatLevel { get; set; }
    public IReadOnlyList<SelectListItem> StatusOptions { get; set; } = [];
    public IReadOnlyList<SelectListItem> ThreatLevelOptions { get; set; } = [];
}
