using SEVEN.Core.Models;

namespace SEVEN.MissionControl.Api.Services.ChartServices;

public static class ReduceDataService
{
    public static IEnumerable<Measurement>  ReduceData(IEnumerable<Measurement>? measurements)
    {
        var reduceDataList = new List<Measurement>();
        if (measurements is null || !measurements.Any()) return reduceDataList;
       
        return measurements.Where((x, i) => i == 0 || x.Value != measurements.ElementAt(i - 1).Value).ToList();
    }
}