namespace SEVEN.Core.Models;

public class ProbeToken
{
    public Guid ProbeId { get; set; }
    public DateTime DateOfExpiry { get; set; }
    public string Token { get; set; }
    public string Type { get; set; }
}