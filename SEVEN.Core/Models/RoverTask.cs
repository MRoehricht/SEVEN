using System.ComponentModel.DataAnnotations;

namespace SEVEN.Core.Models;
public class RoverTask
{
    [Key]
    public Guid Id { get; set; }
    public Guid RoverId { get; set; }
    public RoverTaskCommands Command { get; set; }
    public RoverTaskStatus Status { get; set; } = RoverTaskStatus.Ready;
    public DateTime? StatusUpdate { get; set; }
    public string? StatusInfo { get; set; }
    public Rover? Rover { get; set; }
}

