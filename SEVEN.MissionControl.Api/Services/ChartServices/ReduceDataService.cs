using SEVEN.Core.Models;

namespace SEVEN.MissionControl.Api.Services.ChartServices;

public static class ReduceDataService
{
    public static IEnumerable<Measurement>  ReduceData(IEnumerable<Measurement>? measurements)
    {
        if (measurements is null || !measurements.Any()) return new List<Measurement>();
       
        return measurements.Where((x, i) => i == 0 || x.Value != measurements.ElementAt(i - 1).Value).ToList();
    }
}