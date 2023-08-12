namespace SEVEN.Core.Models;

[Flags]
public enum MeasurementType
{
    Without = 0,
    Temperature = 1,
    Percent = 2,
    StateOfCharge = 4,
    Humidity = 8,
    UvRadiation = 16,
    LightIntensity = 32,
    SoilMoisture= 64,
    SwitchingState= 128
}