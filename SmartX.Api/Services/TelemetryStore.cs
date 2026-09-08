using SmartX.Shared.Models;

namespace SmartX.Api.Services;

public sealed class TelemetryStore<T> where T : struct
{
    private readonly Dictionary<Guid, List<TelemetryPacket<T>>> readings = new();
    private readonly object syncRoot = new();

    public TelemetryPacket<T> Add(Guid sensorId, T value)
    {
        lock (syncRoot)
        {
            var packet = new TelemetryPacket<T>
            {
                SensorId = sensorId,
                Value = value,
                RecordedAtUtc = DateTimeOffset.UtcNow
            };

            if (!readings.TryGetValue(sensorId, out var sensorReadings))
            {
                sensorReadings = new List<TelemetryPacket<T>>();
                readings.Add(sensorId, sensorReadings);
            }

            sensorReadings.Add(packet);

            return packet;
        }
    }

    public TelemetryPacket<T>[] GetHistory(Guid sensorId)
    {
        lock (syncRoot)
        {
            return readings.TryGetValue(sensorId, out var sensorReadings)
                ? sensorReadings.ToArray()
                : [];
        }
    }

    public DailyTelemetryHistory<T> GetDailyHistory(Guid sensorId)
    {
        var snapshot = GetHistory(sensorId);

        var days = snapshot
            .GroupBy(packet => DateOnly.FromDateTime(
                packet.RecordedAtUtc.UtcDateTime))
            .OrderBy(group => group.Key)
            .ToArray();

        var dates = new DateOnly[days.Length];
        var dailyReadings = new TelemetryPacket<T>[days.Length][];

        for (var dayIndex = 0; dayIndex < days.Length; dayIndex++)
        {
            dates[dayIndex] = days[dayIndex].Key;

            dailyReadings[dayIndex] = days[dayIndex]
                .OrderBy(packet => packet.RecordedAtUtc)
                .ToArray();
        }

        return new DailyTelemetryHistory<T>
        {
            SensorId = sensorId,
            Dates = dates,
            Readings = dailyReadings
        };
    }
}