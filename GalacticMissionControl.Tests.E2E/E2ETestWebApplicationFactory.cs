using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using GalacticMissionControl.Web.Data;
using GalacticMissionControl.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GalacticMissionControl.Tests.E2E;

public sealed class E2ETestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DbConnection _connection = new SqliteConnection("DataSource=:memory:");
    private readonly string _serverAddress;
    private IHost? _realHost;

    public E2ETestWebApplicationFactory()
    {
        int port = GetFreePort();
        _serverAddress = $"http://127.0.0.1:{port}";
    }

    public string ServerAddress => _serverAddress;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Configure real Kestrel on the pre-selected port so Playwright's browser can reach it.
        // The base TestServer is in-process only and is not reachable by a real browser.
        builder.ConfigureWebHost(b => b.UseKestrel().UseUrls(_serverAddress));
        _realHost = builder.Build();
        _realHost.Start();

        // Return the in-process test host, used for DI/database access (ResetDatabaseAsync etc.).
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

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
            _realHost?.Dispose();
            _connection.Dispose();
        }
    }

    private static int GetFreePort()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            ExclusiveAddressUse = true
        };
        socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        return ((IPEndPoint)socket.LocalEndPoint!).Port;
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
