namespace SmartX.Shared.Models;

public sealed class SensorMonitorStatus
{
    public required Guid SensorId { get; init; }

    public required string DeviceIdentifier { get; init; }

    public required string DeploymentLocation { get; init; }

    public required string State { get; init; }

    public required string Message { get; init; }

    public required string LatestValue { get; init; }

    public DateTimeOffset? LastReadingAtUtc { get; init; }
}