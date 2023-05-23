namespace SEVEN.Rover.Core.Models
{
    public class RoverStatus
    {
        public string Id { get; set; }

        public List<SwitchStatus> SwitchStatuses { get; set; } = new List<SwitchStatus>();

        public string? ImageData { get; set; }
    }
}
