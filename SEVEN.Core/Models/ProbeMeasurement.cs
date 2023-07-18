namespace SEVEN.Core.Models;

[Flags]
public enum ProbeMeasurement
{
    Temperature = 0,
    Percent = 1,
    StateOfCharge = 2,
    Humidity = 4,
    UvRadiation = 8,
    LightIntensity = 16,
    SwitchingState = 32,
}