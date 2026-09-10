using SmartX.Shared.Models;
using SmartX.Shared.Requests;

namespace SmartX.Api.Services;

public sealed class SensorRegistry
{
    private readonly Dictionary<Guid, SensorProfile> sensors = new();
    private readonly HashSet<string> deviceIdentifiers =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly object syncRoot = new();

    public SensorProfile[] GetAll()
    {
        lock (syncRoot)
        {
            return sensors.Values
                .OrderBy(sensor => sensor.DeviceIdentifier)
                .ToArray();
        }
    }

    public SensorProfile? GetById(Guid id)
    {
        lock (syncRoot)
        {
            return sensors.GetValueOrDefault(id);
        }
    }

    public SensorProfile? Register(RegisterSensorRequest request)
    {

        // keeps the uniqueness check and insertion together so they cannt register the same identifier
        var deviceIdentifier = request.DeviceIdentifier.Trim();

        lock (syncRoot)
        {
            if (deviceIdentifiers.Contains(deviceIdentifier))
            {
                return null;
            }

            var sensor = new SensorProfile
            {
                Id = Guid.NewGuid(),
                DeviceIdentifier = deviceIdentifier,
                DeploymentLocation = request.DeploymentLocation.Trim(),
                Category = request.Category!.Value,
                DataType = request.DataType!.Value,
                RegisteredAtUtc = DateTimeOffset.UtcNow
            };

            sensors.Add(sensor.Id, sensor);
            deviceIdentifiers.Add(deviceIdentifier);

            return sensor;
        }
    }
}