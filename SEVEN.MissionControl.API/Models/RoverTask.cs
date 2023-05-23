namespace SEVEN.MissionControl.API.Models
{
    public class RoverTask
    {
        public Guid Id { get; set; }
        public Guid RoverId { get; set; }
        public RoverTaskCommands Command { get; set; }
        public RoverTaskStatus Status { get; set; } = RoverTaskStatus.Ready;
        public DateTime? StatusUpdate { get; set; }
        public string? StatusInfo { get; set; }
    }
}
