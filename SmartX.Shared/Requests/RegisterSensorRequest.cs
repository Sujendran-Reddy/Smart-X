using System.ComponentModel.DataAnnotations;
using SmartX.Shared.Models;

namespace SmartX.Shared.Requests;

public sealed class RegisterSensorRequest
{
    [Required(ErrorMessage = "Enter a device identifier.")]
    [StringLength(100, ErrorMessage = "The device identifier cannot exceed 100 characters.")]
    public string DeviceIdentifier { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter a deployment location.")]
    [StringLength(200, ErrorMessage = "The deployment location cannot exceed 200 characters.")]
    public string DeploymentLocation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Select a sensor category.")]
    [EnumDataType(typeof(SensorCategory), ErrorMessage = "Select a valid sensor category.")]
    public SensorCategory? Category { get; set; }

    [Required(ErrorMessage = "Select a telemetry data type.")]
    [EnumDataType(typeof(TelemetryDataType), ErrorMessage = "Select a valid telemetry data type.")]
    public TelemetryDataType? DataType { get; set; }
}