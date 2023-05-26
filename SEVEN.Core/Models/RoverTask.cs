using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEVEN.Core.Models;
public class RoverTask
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public int Position { get; set; }
    [Required]
    public Guid RoverId { get; set; }
    public RoverTaskCommands Command { get; set; }
    public RoverTaskStatus Status { get; set; } = RoverTaskStatus.Ready;
    public DateTime? StatusUpdate { get; set; }
    public string? StatusInfo { get; set; }
    [JsonIgnore]
    public virtual Rover? Rover { get; set; }
}

