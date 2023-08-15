using SEVEN.Core.Models;

namespace SEVEN.MissionControl.Api.Services.ChartServices;

public static class ReduceDataService
{
    public static IEnumerable<Measurement>  ReduceData(IEnumerable<Measurement>? measurements)
    {
        var reduceDataList = new List<Measurement>();
        if (measurements is null || !measurements.Any()) return reduceDataList;

        Measurement? lastMeasurement = null;
        foreach (var measurement in measurements)
        {
            if (lastMeasurement == null && !string.IsNullOrWhiteSpace(measurement.Value))
            {
                reduceDataList.Add(measurement);
            }
            else if(!string.IsNullOrWhiteSpace(measurement.Value) && lastMeasurement.Value != measurement.Value)
            {
                reduceDataList.Add(measurement);
            }
            else if (measurement == measurements.Last())
            {
                reduceDataList.Add(measurement);
            }
            
            lastMeasurement = measurement;
        }

        return reduceDataList;
    }
}