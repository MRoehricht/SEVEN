namespace SEVEN.MissionControl.API.Models
{
    public class Rover
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<RoverTask> OpenTasks { get; set; } = new List<RoverTask>();
        public List<RoverTask> CompletedTasks { get; set; } = new List<RoverTask>();
    }
}
