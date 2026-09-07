namespace SmartX.Shared.Models;

public sealed class TelemetryPacket<T> where T : struct
{
    public required Guid SensorId { get; init; }

    public required T Value { get; init; }

    public required DateTimeOffset RecordedAtUtc { get; init; }
}