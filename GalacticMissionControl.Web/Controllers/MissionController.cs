using GalacticMissionControl.Web.Models;
using GalacticMissionControl.Web.Services;
using GalacticMissionControl.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GalacticMissionControl.Web.Controllers;

public class MissionController : Controller
{
    private static readonly string[] StatusChoices =
    [
        "Planned",
        "Preparing",
        "Launched",
        "En Route",
        "Active",
        "Completed",
        "Aborted"
    ];

    private static readonly string[] ThreatLevelChoices =
    [
        "Low",
        "Moderate",
        "High",
        "Critical"
    ];

    private static readonly string[] PriorityChoices =
    [
        "Low",
        "Normal",
        "High",
        "Urgent"
    ];

    private readonly IMissionService _missionService;

    public MissionController(IMissionService missionService)
    {
        _missionService = missionService;
    }

    public async Task<IActionResult> Index(string? status, string? threatLevel)
    {
        var missions = await _missionService.GetMissionsByFiltersAsync(status, threatLevel);

        var viewModel = new MissionIndexViewModel
        {
            Missions = missions,
            SelectedStatus = status,
            SelectedThreatLevel = threatLevel,
            StatusOptions = BuildOptions(StatusChoices, status, "All statuses"),
            ThreatLevelOptions = BuildOptions(ThreatLevelChoices, threatLevel, "All threat levels")
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var mission = await _missionService.GetMissionByIdAsync(id);
        if (mission is null)
        {
            return NotFound();
        }

        return View(mission);
    }

    public IActionResult Create()
    {
        var viewModel = BuildFormViewModel(new Mission
        {
            LaunchDate = DateTime.UtcNow.Date
        });

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MissionFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            HydrateFormLists(viewModel);
            return View(viewModel);
        }

        await _missionService.CreateMissionAsync(viewModel.Mission);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var mission = await _missionService.GetMissionByIdAsync(id);
        if (mission is null)
        {
            return NotFound();
        }

        return View(BuildFormViewModel(mission));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MissionFormViewModel viewModel)
    {
        if (id != viewModel.Mission.MissionId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            HydrateFormLists(viewModel);
            return View(viewModel);
        }

        var updated = await _missionService.UpdateMissionAsync(viewModel.Mission);
        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Details), new { id = viewModel.Mission.MissionId });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var mission = await _missionService.GetMissionByIdAsync(id);
        if (mission is null)
        {
            return NotFound();
        }

        return View(mission);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _missionService.DeleteMissionAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private MissionFormViewModel BuildFormViewModel(Mission mission)
    {
        var viewModel = new MissionFormViewModel
        {
            Mission = mission
        };

        HydrateFormLists(viewModel);
        return viewModel;
    }

    private void HydrateFormLists(MissionFormViewModel viewModel)
    {
        viewModel.StatusOptions = BuildOptions(StatusChoices, viewModel.Mission.Status, null);
        viewModel.ThreatLevelOptions = BuildOptions(ThreatLevelChoices, viewModel.Mission.ThreatLevel, null);
        viewModel.PriorityOptions = BuildOptions(PriorityChoices, viewModel.Mission.Priority, null);
    }

    private static IReadOnlyList<SelectListItem> BuildOptions(IEnumerable<string> values, string? selectedValue, string? placeholder)
    {
        var items = new List<SelectListItem>();

        if (placeholder is not null)
        {
            items.Add(new SelectListItem(placeholder, string.Empty, string.IsNullOrWhiteSpace(selectedValue)));
        }

        items.AddRange(values.Select(value => new SelectListItem(value, value, value == selectedValue)));
        return items;
    }
}
