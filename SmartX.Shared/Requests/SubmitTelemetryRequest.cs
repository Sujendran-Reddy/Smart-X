using System.ComponentModel.DataAnnotations;

namespace SmartX.Shared.Requests;

public sealed class SubmitTelemetryRequest<T> where T : struct
{
    [Required(ErrorMessage = "Provide a telemetry value.")]
    public T? Value { get; set; }
}