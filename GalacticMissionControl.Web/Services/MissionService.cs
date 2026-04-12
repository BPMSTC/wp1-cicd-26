using GalacticMissionControl.Web.Data;
using GalacticMissionControl.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GalacticMissionControl.Web.Services;

/// <summary>
/// Handles mission CRUD and filtering logic using EF Core.
/// </summary>
public class MissionService : IMissionService
{
    private readonly ApplicationDbContext _dbContext;

    public MissionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Mission>> GetAllMissionsAsync()
    {
        return await _dbContext.Missions
            .AsNoTracking()
            .OrderBy(m => m.MissionId)
            .ToListAsync();
    }

    public async Task<Mission?> GetMissionByIdAsync(int missionId)
    {
        return await _dbContext.Missions
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MissionId == missionId);
    }

    public async Task<List<Mission>> GetMissionsByStatusAsync(string status)
    {
        return await _dbContext.Missions
            .AsNoTracking()
            .Where(m => m.Status == status)
            .OrderBy(m => m.MissionId)
            .ToListAsync();
    }

    public async Task<List<Mission>> GetMissionsByThreatLevelAsync(string threatLevel)
    {
        return await _dbContext.Missions
            .AsNoTracking()
            .Where(m => m.ThreatLevel == threatLevel)
            .OrderBy(m => m.MissionId)
            .ToListAsync();
    }

    public async Task<List<Mission>> GetMissionsByFiltersAsync(string? status, string? threatLevel)
    {
        IQueryable<Mission> query = _dbContext.Missions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(m => m.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(threatLevel))
        {
            query = query.Where(m => m.ThreatLevel == threatLevel);
        }

        return await query
            .OrderBy(m => m.MissionId)
            .ToListAsync();
    }

    public async Task<Mission> CreateMissionAsync(Mission mission)
    {
        _dbContext.Missions.Add(mission);
        await _dbContext.SaveChangesAsync();
        return mission;
    }

    public async Task<bool> UpdateMissionAsync(Mission mission)
    {
        var existingMission = await _dbContext.Missions
            .FirstOrDefaultAsync(m => m.MissionId == mission.MissionId);

        if (existingMission is null)
        {
            return false;
        }

        existingMission.Title = mission.Title;
        existingMission.Description = mission.Description;
        existingMission.AssignedSector = mission.AssignedSector;
        existingMission.Commander = mission.Commander;
        existingMission.Priority = mission.Priority;
        existingMission.Status = mission.Status;
        existingMission.ThreatLevel = mission.ThreatLevel;
        existingMission.LaunchDate = mission.LaunchDate;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteMissionAsync(int missionId)
    {
        var mission = await _dbContext.Missions
            .FirstOrDefaultAsync(m => m.MissionId == missionId);

        if (mission is null)
        {
            return false;
        }

        _dbContext.Missions.Remove(mission);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
