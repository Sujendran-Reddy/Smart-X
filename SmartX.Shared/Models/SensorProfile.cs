namespace SmartX.Shared.Models;

public sealed class SensorProfile
{
    public required Guid Id { get; init; }

    public required string DeviceIdentifier { get; init; }

    public required string DeploymentLocation { get; init; }

    public required SensorCategory Category { get; init; }

    public required TelemetryDataType DataType { get; init; }

    public required DateTimeOffset RegisteredAtUtc { get; init; }
}