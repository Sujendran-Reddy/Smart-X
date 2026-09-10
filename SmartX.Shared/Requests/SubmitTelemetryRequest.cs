using System.ComponentModel.DataAnnotations;

namespace SmartX.Shared.Requests;

public sealed class SubmitTelemetryRequest<T> where T : struct
{
    /* 
    nullable makes it easier to distinguish a missing 
    value from valid readings like 0 or false
    */

    [Required(ErrorMessage = "Provide a telemetry value.")]
    public T? Value { get; set; }
}