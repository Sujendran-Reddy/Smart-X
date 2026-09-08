namespace SmartX.Shared.Models;

public sealed class MonitoringSnapshot
{
    public required DateTimeOffset CheckedAtUtc { get; init; }

    public required List<SensorMonitorStatus> Sensors { get; init; }

    public int TotalSensors => Sensors.Count;

    public int NormalCount => Sensors.Count(sensor => sensor.State == "Normal");

    public int WarningCount => Sensors.Count(sensor => sensor.State == "Warning");

    public int StaleCount => Sensors.Count(sensor => sensor.State == "Stale");

    public int NoDataCount => Sensors.Count(sensor => sensor.State == "No data");
}