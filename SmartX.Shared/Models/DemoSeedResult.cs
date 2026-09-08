namespace SmartX.Shared.Models;

public sealed class DemoSeedResult
{
    public required int SensorsCreated { get; init; }

    public required int ReadingsCreated { get; init; }

    public required int DaysPerSensor { get; init; }

    public required long ElapsedMilliseconds { get; init; }

    public required DateTimeOffset CompletedAtUtc { get; init; }
}