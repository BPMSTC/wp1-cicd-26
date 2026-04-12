using System.Data.Common;
using GalacticMissionControl.Web.Data;
using GalacticMissionControl.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GalacticMissionControl.Tests.Integration;

public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DbConnection _connection = new SqliteConnection("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            var connectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
            if (connectionDescriptor is not null)
            {
                services.Remove(connectionDescriptor);
            }

            services.AddSingleton(_connection);
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var connection = serviceProvider.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            _connection.Open();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            SeedData(dbContext);
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        await SeedDataAsync(dbContext);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection.Dispose();
        }
    }

    private static void SeedData(ApplicationDbContext dbContext)
    {
        dbContext.Missions.AddRange(
            new Mission
            {
                MissionId = 1,
                Title = "Europa Ice Survey",
                Description = "Collect ice-core telemetry from Europa stations.",
                AssignedSector = "Jupiter Frontier",
                Commander = "Cmdr. Patel",
                Priority = "High",
                Status = "Preparing",
                ThreatLevel = "Moderate",
                LaunchDate = new DateTime(2032, 5, 14)
            },
            new Mission
            {
                MissionId = 2,
                Title = "Titan Shield Calibration",
                Description = "Verify defensive grid harmonics.",
                AssignedSector = "Saturn Ring",
                Commander = "Cmdr. Alvarez",
                Priority = "Urgent",
                Status = "Active",
                ThreatLevel = "High",
                LaunchDate = new DateTime(2032, 7, 3)
            });

        dbContext.SaveChanges();
    }

    private static async Task SeedDataAsync(ApplicationDbContext dbContext)
    {
        dbContext.Missions.AddRange(
            new Mission
            {
                MissionId = 1,
                Title = "Europa Ice Survey",
                Description = "Collect ice-core telemetry from Europa stations.",
                AssignedSector = "Jupiter Frontier",
                Commander = "Cmdr. Patel",
                Priority = "High",
                Status = "Preparing",
                ThreatLevel = "Moderate",
                LaunchDate = new DateTime(2032, 5, 14)
            },
            new Mission
            {
                MissionId = 2,
                Title = "Titan Shield Calibration",
                Description = "Verify defensive grid harmonics.",
                AssignedSector = "Saturn Ring",
                Commander = "Cmdr. Alvarez",
                Priority = "Urgent",
                Status = "Active",
                ThreatLevel = "High",
                LaunchDate = new DateTime(2032, 7, 3)
            });

        await dbContext.SaveChangesAsync();
    }
}
