namespace SmartX.Shared.Models;

public sealed class SensorAttachment
{
    public required Guid Id { get; init; }

    public required Guid SensorId { get; init; }

    public required string FileName { get; init; }

    public required long SizeBytes { get; init; }

    public required DateTimeOffset UploadedAtUtc { get; init; }
}