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
    private SqliteConnection? _connection;
    private bool _databaseInitialized;
    private readonly string _serverAddress;
    private IHost? _realHost;

    public E2ETestWebApplicationFactory()
    {
        int port = GetFreePort();
        _serverAddress = $"http://127.0.0.1:{port}";

        // Ensure native SQLite engine is initialized exactly once per process.
        SQLitePCL.Batteries_V2.Init();
    }

    public string ServerAddress => _serverAddress;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Start an in-process TestServer host for test services (DI/database reset/seeding).
        var testHost = builder.Build();

        // Start a second real Kestrel host that Playwright can connect to over HTTP.
        builder.ConfigureWebHost(b => b.UseKestrel().UseUrls(_serverAddress));
        _realHost = builder.Build();
        _realHost.Start();

        testHost.Start();
        return testHost;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            if (_connection is null)
            {
                _connection = new SqliteConnection("Data Source=:memory:");
                _connection.Open();
            }

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

            services.AddSingleton<DbConnection>(_connection);
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var connection = serviceProvider.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!_databaseInitialized)
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                SeedData(dbContext);
                _databaseInitialized = true;
            }
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
        if (disposing)
        {
            _realHost?.Dispose();
            _connection?.Dispose();
            _connection = null;
        }

        base.Dispose(disposing);
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
