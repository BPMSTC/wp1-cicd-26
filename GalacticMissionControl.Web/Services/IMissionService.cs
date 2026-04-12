using GalacticMissionControl.Web.Models;

namespace GalacticMissionControl.Web.Services;

/// <summary>
/// Defines mission-focused business operations for MVC controllers.
/// </summary>
public interface IMissionService
{
    Task<List<Mission>> GetAllMissionsAsync();
    Task<Mission?> GetMissionByIdAsync(int missionId);
    Task<List<Mission>> GetMissionsByStatusAsync(string status);
    Task<List<Mission>> GetMissionsByThreatLevelAsync(string threatLevel);
    Task<List<Mission>> GetMissionsByFiltersAsync(string? status, string? threatLevel);
    Task<Mission> CreateMissionAsync(Mission mission);
    Task<bool> UpdateMissionAsync(Mission mission);
    Task<bool> DeleteMissionAsync(int missionId);
}
