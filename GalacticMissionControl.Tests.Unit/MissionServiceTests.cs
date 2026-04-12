using GalacticMissionControl.Web.Data;
using GalacticMissionControl.Web.Models;
using GalacticMissionControl.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace GalacticMissionControl.Tests.Unit;

public class MissionServiceTests
{
    [Fact]
    public async Task GetAllMissionsAsync_ReturnsAllMissionsOrderedByMissionId()
    {
        // Arrange
        await using var dbContext = CreateContextWithSeedData();
        var service = new MissionService(dbContext);

        // Act
        var missions = await service.GetAllMissionsAsync();

        // Assert
        Assert.Equal(3, missions.Count);
        Assert.Equal(new[] { 1, 2, 3 }, missions.Select(m => m.MissionId));
    }

    [Fact]
    public async Task GetMissionByIdAsync_WhenMissionExists_ReturnsMatchingMission()
    {
        // Arrange
        await using var dbContext = CreateContextWithSeedData();
        var service = new MissionService(dbContext);

        // Act
        var mission = await service.GetMissionByIdAsync(2);

        // Assert
        Assert.NotNull(mission);
        Assert.Equal("Nebula Survey", mission!.Title);
    }

    [Fact]
    public async Task GetMissionByIdAsync_WhenMissionDoesNotExist_ReturnsNull()
    {
        // Arrange
        await using var dbContext = CreateContextWithSeedData();
        var service = new MissionService(dbContext);

        // Act
        var mission = await service.GetMissionByIdAsync(999);

        // Assert
        Assert.Null(mission);
    }

    [Fact]
    public async Task CreateMissionAsync_AddsMissionAndPersistsValues()
    {
        // Arrange
        await using var dbContext = CreateContextWithSeedData();
        var service = new MissionService(dbContext);
        var newMission = BuildMission(
            missionId: 0,
            title: "Lunar Relay Repair",
            status: "Planned",
            threatLevel: "Low");

        // Act
        var createdMission = await service.CreateMissionAsync(newMission);

        // Assert
        Assert.True(createdMission.MissionId > 0);
        var saved = await dbContext.Missions.AsNoTracking().SingleAsync(m => m.MissionId == createdMission.MissionId);
        Assert.Equal("Lunar Relay Repair", saved.Title);
        Assert.Equal("Planned", saved.Status);
        Assert.Equal("Low", saved.ThreatLevel);
    }

    [Fact]
    public async Task UpdateMissionAsync_WhenMissionExists_UpdatesMissionAndReturnsTrue()
    {
        // Arrange
        await using var dbContext = CreateContextWithSeedData();
        var service = new MissionService(dbContext);
        var updatedMission = BuildMission(
            missionId: 1,
            title: "Orion Relay Upgrade",
            status: "In Progress",
            threatLevel: "Medium");

        // Act
        var updated = await service.UpdateMissionAsync(updatedMission);

        // Assert
        Assert.True(updated);
        var saved = await dbContext.Missions.AsNoTracking().SingleAsync(m => m.MissionId == 1);
        Assert.Equal("Orion Relay Upgrade", saved.Title);
        Assert.Equal("In Progress", saved.Status);
        Assert.Equal("Medium", saved.ThreatLevel);
    }

    [Fact]
    public async Task UpdateMissionAsync_WhenMissionDoesNotExist_ReturnsFalse()
    {
        // Arrange
        await using var dbContext = CreateContextWithSeedData();
        var service = new MissionService(dbContext);
        var missingMission = BuildMission(
            missionId: 404,
            title: "Unknown Mission",
            status: "Cancelled",
            threatLevel: "Critical");

        // Act
        var updated = await service.UpdateMissionAsync(missingMission);

        // Assert
        Assert.False(updated);
    }

    [Fact]
    public async Task DeleteMissionAsync_WhenMissionExists_RemovesMissionAndReturnsTrue()
    {
        // Arrange
        await using var dbContext = CreateContextWithSeedData();
        var service = new MissionService(dbContext);

        // Act
        var deleted = await service.DeleteMissionAsync(3);

        // Assert
        Assert.True(deleted);
        Assert.Equal(2, await dbContext.Missions.CountAsync());
        Assert.Null(await dbContext.Missions.FindAsync(3));
    }

    [Fact]
    public async Task DeleteMissionAsync_WhenMissionDoesNotExist_ReturnsFalse()
    {
        // Arrange
        await using var dbContext = CreateContextWithSeedData();
        var service = new MissionService(dbContext);

        // Act
        var deleted = await service.DeleteMissionAsync(999);

        // Assert
        Assert.False(deleted);
    }

    [Fact]
    public async Task GetMissionsByStatusAsync_ReturnsOnlyMissionsWithMatchingStatus()
    {
        // Arrange
        await using var dbContext = CreateContextWithSeedData();
        var service = new MissionService(dbContext);

        // Act
        var missions = await service.GetMissionsByStatusAsync("Planned");

        // Assert
        Assert.Equal(2, missions.Count);
        Assert.All(missions, mission => Assert.Equal("Planned", mission.Status));
    }

    [Fact]
    public async Task GetMissionsByThreatLevelAsync_ReturnsOnlyMissionsWithMatchingThreatLevel()
    {
        // Arrange
        await using var dbContext = CreateContextWithSeedData();
        var service = new MissionService(dbContext);

        // Act
        var missions = await service.GetMissionsByThreatLevelAsync("High");

        // Assert
        Assert.Single(missions);
        Assert.Equal("Titan Shield", missions[0].Title);
    }

    private static ApplicationDbContext CreateContextWithSeedData()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"missions-{Guid.NewGuid()}")
            .Options;

        var dbContext = new ApplicationDbContext(options);
        dbContext.Missions.AddRange(
            BuildMission(1, "Orion Relay", "Planned", "Low"),
            BuildMission(2, "Nebula Survey", "In Progress", "Medium"),
            BuildMission(3, "Titan Shield", "Planned", "High"));
        dbContext.SaveChanges();

        return dbContext;
    }

    private static Mission BuildMission(int missionId, string title, string status, string threatLevel)
    {
        return new Mission
        {
            MissionId = missionId,
            Title = title,
            Description = $"Description for {title}",
            AssignedSector = "Sector 7",
            Commander = "Cmdr. Vega",
            Priority = "High",
            Status = status,
            ThreatLevel = threatLevel,
            LaunchDate = new DateTime(2030, 1, 15)
        };
    }
}
