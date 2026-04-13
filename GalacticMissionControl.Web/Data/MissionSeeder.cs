using GalacticMissionControl.Web.Models;

namespace GalacticMissionControl.Web.Data;

/// <summary>
/// Seeds the database with initial mission data for local development and testing.
/// Only runs when the Missions table is empty.
/// </summary>
public static class MissionSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Missions.Any())
        {
            return; // Already seeded
        }

        var missions = new List<Mission>
        {
            new Mission
            {
                Title = "Operation Solar Veil",
                Description = "Intercept and neutralize rogue satellite network broadcasting encrypted transmissions from sector 7.",
                AssignedSector = "Sector 7 - Outer Belt",
                Commander = "Cmdr. Raya Solano",
                Priority = "Critical",
                Status = "Active",
                ThreatLevel = "High",
                LaunchDate = new DateTime(2025, 3, 12)
            },
            new Mission
            {
                Title = "Operation Frozen Horizon",
                Description = "Establish a forward listening post on the dark side of Europa to monitor deep-space anomalies.",
                AssignedSector = "Sector 14 - Europa Station",
                Commander = "Cmdr. Idris Vance",
                Priority = "High",
                Status = "Active",
                ThreatLevel = "Medium",
                LaunchDate = new DateTime(2025, 4, 1)
            },
            new Mission
            {
                Title = "Operation Crimson Tide",
                Description = "Decommission three derelict warships drifting into inhabited shipping lanes near the Mars corridor.",
                AssignedSector = "Sector 3 - Mars Corridor",
                Commander = "Cmdr. Lexa Morrow",
                Priority = "High",
                Status = "Completed",
                ThreatLevel = "Low",
                LaunchDate = new DateTime(2025, 1, 20)
            },
            new Mission
            {
                Title = "Operation Phantom Signal",
                Description = "Trace the origin of repeating xenomorphic radio signals detected along the outer rim waypoints.",
                AssignedSector = "Sector 22 - Outer Rim",
                Commander = null,
                Priority = "Medium",
                Status = "Pending",
                ThreatLevel = "Medium",
                LaunchDate = new DateTime(2025, 6, 15)
            },
            new Mission
            {
                Title = "Operation Dark Meridian",
                Description = "Provide covert escort to a diplomatic convoy transiting through contested neutral zone.",
                AssignedSector = "Sector 9 - Neutral Zone",
                Commander = "Cmdr. Tomas Drexel",
                Priority = "Critical",
                Status = "Active",
                ThreatLevel = "High",
                LaunchDate = new DateTime(2025, 2, 28)
            },
            new Mission
            {
                Title = "Operation Starfall",
                Description = "Recover classified data cores from a disabled research vessel before it enters Jovian atmosphere.",
                AssignedSector = "Sector 11 - Jupiter Approach",
                Commander = "Cmdr. Yuki Harshan",
                Priority = "High",
                Status = "Active",
                ThreatLevel = "Critical",
                LaunchDate = new DateTime(2025, 5, 4)
            },
            new Mission
            {
                Title = "Operation Pale Ember",
                Description = "Survey and catalog newly detected asteroid cluster for potential mineral extraction viability.",
                AssignedSector = "Sector 18 - Kuiper Survey Zone",
                Commander = "Cmdr. Orion Blake",
                Priority = "Low",
                Status = "Pending",
                ThreatLevel = "Low",
                LaunchDate = new DateTime(2025, 8, 10)
            },
            new Mission
            {
                Title = "Operation Iron Wake",
                Description = "Neutralize the pirate blockade disrupting supply lines to the Titan colony outposts.",
                AssignedSector = "Sector 5 - Titan Approaches",
                Commander = "Cmdr. Sela Korvik",
                Priority = "Critical",
                Status = "Completed",
                ThreatLevel = "High",
                LaunchDate = new DateTime(2025, 1, 5)
            },
            new Mission
            {
                Title = "Operation Mirage Protocol",
                Description = "Deploy stealth reconnaissance drones to map undocumented structure on asteroid 2029-RX.",
                AssignedSector = "Sector 22 - Outer Rim",
                Commander = "Cmdr. Raya Solano",
                Priority = "Medium",
                Status = "Active",
                ThreatLevel = "Medium",
                LaunchDate = new DateTime(2025, 4, 22)
            },
            new Mission
            {
                Title = "Operation Silent Reach",
                Description = "Evacuate stranded science team from compromised deep-space research station Delta-9.",
                AssignedSector = "Sector 30 - Deep Space Outpost",
                Commander = null,
                Priority = "High",
                Status = "Pending",
                ThreatLevel = "Medium",
                LaunchDate = new DateTime(2025, 7, 1)
            }
        };

        context.Missions.AddRange(missions);
        context.SaveChanges();
    }
}
