using System.Data.Common;
using GalacticMissionControl.Web.Data;
using GalacticMissionControl.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GalacticMissionControl.Tests.E2E;

public sealed class E2ETestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DbConnection _connection = new SqliteConnection("DataSource=:memory:");
    private string? _serverAddress;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseKestrel();
        builder.UseUrls("http://127.0.0.1:0");

        builder.ConfigureServices(services =>
        {
            var descriptorsToRemove = services
                .Where(descriptor =>
                    descriptor.ServiceType == typeof(ApplicationDbContext)
                    || descriptor.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
                    || descriptor.ServiceType == typeof(DbConnection)
                    || (descriptor.ServiceType.IsGenericType
                        && descriptor.ServiceType.GenericTypeArguments.Contains(typeof(ApplicationDbContext))))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
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

    public string ServerAddress
    {
        get
        {
            if (_serverAddress == null)
            {
                var addresses = Server.Features.Get<IServerAddressesFeature>()
                    ?? throw new InvalidOperationException("Server addresses feature not available.");

                _serverAddress = addresses.Addresses.FirstOrDefault()
                    ?? throw new InvalidOperationException("No server addresses configured.");
            }

            return _serverAddress;
        }
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
