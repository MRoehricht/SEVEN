using System.ComponentModel.DataAnnotations;

namespace SEVEN.Core.Models;

public class Probe
{
    [Key]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public ProbeMeasurement Measurements { get; set; }
    public int SendingIntervalMinutes { get; set; }
}