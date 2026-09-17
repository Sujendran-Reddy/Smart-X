using System.ComponentModel.DataAnnotations;

namespace SmartX.Shared.Requests;

public sealed class IssueCommandRequest
{
    [Required(ErrorMessage = "Choose whether the device should be on or off.")]
    public bool? RequestedState { get; set; }
}