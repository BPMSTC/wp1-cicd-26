using System.ComponentModel.DataAnnotations;

namespace GalacticMissionControl.Web.Models;

/// <summary>
/// Represents a mission that can be validated from a form submission.
/// </summary>
public class Mission
{
    public int MissionId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string AssignedSector { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Commander { get; set; }

    [Required]
    [StringLength(20)]
    public string Priority { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string ThreatLevel { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime LaunchDate { get; set; }
}
