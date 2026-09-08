using System.Globalization;
using SmartX.Shared.Models;

namespace SmartX.Api.Services;

public sealed class MonitoringService
{
    private readonly SensorRegistry registry;
    private readonly TelemetryStore<float> floatStore;
    private readonly TelemetryStore<int> integerStore;
    private readonly TelemetryStore<bool> booleanStore;
    private readonly float minimumMoisture;
    private readonly float maximumMoisture;
    private readonly PowerReading powerLimit;
    private readonly TimeSpan staleAfter;

    public MonitoringService(
        SensorRegistry registry,
        TelemetryStore<float> floatStore,
        TelemetryStore<int> integerStore,
        TelemetryStore<bool> booleanStore,
        IConfiguration configuration)
    {
        this.registry = registry;
        this.floatStore = floatStore;
        this.integerStore = integerStore;
        this.booleanStore = booleanStore;

        minimumMoisture = configuration.GetValue<float>(
            "Monitoring:MinimumMoisturePercent", 20);

        maximumMoisture = configuration.GetValue<float>(
            "Monitoring:MaximumMoisturePercent", 80);

        var maximumPower = configuration.GetValue<int>(
            "Monitoring:MaximumPowerWatts", 1200);

        var staleSeconds = configuration.GetValue<int>(
            "Monitoring:StaleAfterSeconds", 120);

        if (!float.IsFinite(minimumMoisture) ||
            !float.IsFinite(maximumMoisture) ||
            minimumMoisture < 0 ||
            maximumMoisture > 100 ||
            minimumMoisture >= maximumMoisture ||
            maximumPower <= 0 ||
            staleSeconds <= 0)
        {
            throw new InvalidOperationException(
                "Monitoring thresholds are invalid.");
        }

        powerLimit = new PowerReading(maximumPower);
        staleAfter = TimeSpan.FromSeconds(staleSeconds);
    }

    public MonitoringSnapshot GetSnapshot()
    {
        var now = DateTimeOffset.UtcNow;
        var sensors = registry.GetAll();
        var statuses = new List<SensorMonitorStatus>(sensors.Length);

        foreach (var sensor in sensors)
        {
            DateTimeOffset? recordedAt = null;
            var latestValue = "—";
            string? warning = null;

            switch (sensor.DataType)
            {
                case TelemetryDataType.Float:
                    {
                        var packet = floatStore.GetLatest(sensor.Id);

                        if (packet is not null)
                        {
                            recordedAt = packet.RecordedAtUtc;
                            latestValue = packet.Value.ToString(
                                "0.##",
                                CultureInfo.InvariantCulture);

                            if (sensor.Category == SensorCategory.Environmental)
                            {
                                latestValue += " %";

                                if (packet.Value < minimumMoisture ||
                                    packet.Value > maximumMoisture)
                                {
                                    warning = $"Soil moisture is outside the configured {minimumMoisture}–{maximumMoisture}% demo range.";
                                }
                            }
                        }

                        break;
                    }

                case TelemetryDataType.Integer:
                    {
                        var packet = integerStore.GetLatest(sensor.Id);

                        if (packet is not null)
                        {
                            recordedAt = packet.RecordedAtUtc;
                            latestValue = packet.Value.ToString(
                                CultureInfo.InvariantCulture);

                            if (sensor.Category == SensorCategory.PowerConsumption)
                            {
                                latestValue += " W";

                                if (packet.Value < 0)
                                {
                                    warning = "Power consumption cannot be negative in this demo.";
                                }
                                else if (new PowerReading(packet.Value) > powerLimit)
                                {
                                    warning = $"Power consumption exceeds the configured {powerLimit.Watts} W demo limit.";
                                }
                            }
                        }

                        break;
                    }

                case TelemetryDataType.Boolean:
                    {
                        var packet = booleanStore.GetLatest(sensor.Id);

                        if (packet is not null)
                        {
                            recordedAt = packet.RecordedAtUtc;
                            latestValue = packet.Value ? "True" : "False";
                        }

                        break;
                    }
            }

            string state;
            string message;

            if (!recordedAt.HasValue)
            {
                state = "No data";
                message = "No telemetry has been recorded for this sensor.";
            }
            else if (now - recordedAt.Value > staleAfter)
            {
                state = "Stale";
                message = $"No reading received within {staleAfter.TotalSeconds:0} seconds.";

                if (warning is not null)
                {
                    message += $" Last recorded value: {warning}";
                }
            }
            else if (warning is not null)
            {
                state = "Warning";
                message = warning;
            }
            else
            {
                state = "Normal";
                message = "A recent reading is available with no configured threshold warning.";
            }

            statuses.Add(new SensorMonitorStatus
            {
                SensorId = sensor.Id,
                DeviceIdentifier = sensor.DeviceIdentifier,
                DeploymentLocation = sensor.DeploymentLocation,
                State = state,
                Message = message,
                LatestValue = latestValue,
                LastReadingAtUtc = recordedAt
            });
        }

        return new MonitoringSnapshot
        {
            CheckedAtUtc = now,
            Sensors = statuses
        };
    }
}