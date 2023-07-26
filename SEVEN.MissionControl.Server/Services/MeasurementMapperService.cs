using System.Text.Json;
using SEVEN.Core.Models;
using SEVEN.Core.Models.Messages;

namespace SEVEN.MissionControl.Server.Services;

public static class MeasurementMapperService
{
    public static IEnumerable<Measurement> Map(string? message)
    {
        var measurements = new List<Measurement>();
        if (string.IsNullOrWhiteSpace(message)) return measurements;

        var measurementMessage = JsonSerializer.Deserialize<MeasurementMessage>(message);
        if (measurementMessage is null) return measurements;

        foreach (var item in measurementMessage.MeasurementItems)
        {
            if (string.IsNullOrWhiteSpace(item.Value)) continue;
            measurements.Add(new Measurement
            {
                Id = Guid.NewGuid(), 
                ProbeId = measurementMessage.ProbeId, 
                Time = DateTime.UtcNow, 
                MeasurementType = (MeasurementType)item.Type,
                Value = item.Value
            });
        }

        return measurements;
    }
}