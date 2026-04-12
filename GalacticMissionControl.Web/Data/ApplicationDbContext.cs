using GalacticMissionControl.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GalacticMissionControl.Web.Data;

/// <summary>
/// Application database context used to access mission data in SQL Server.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Mission> Missions => Set<Mission>();
}
