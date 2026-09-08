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
}