namespace SmartX.Shared.Models;

public sealed class DeviceCommand
{
    public required Guid Id { get; init; }

    public required Guid SensorId { get; init; }

    public required bool PreviousState { get; init; }

    public required bool RequestedState { get; init; }

    public required DateTimeOffset IssuedAtUtc { get; init; }

    public DateTimeOffset? UndoneAtUtc { get; set; }
}