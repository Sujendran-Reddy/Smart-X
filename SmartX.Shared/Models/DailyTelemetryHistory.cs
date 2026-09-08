namespace SmartX.Shared.Models;

public sealed class DailyTelemetryHistory<T> where T : struct
{
    public required Guid SensorId { get; init; }

    public required DateOnly[] Dates { get; init; }

    public required TelemetryPacket<T>[][] Readings { get; init; }

    public int TotalReadings => Readings.Sum(day => day.Length);
}