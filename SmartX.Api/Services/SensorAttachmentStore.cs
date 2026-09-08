using SmartX.Shared.Models;

namespace SmartX.Api.Services;

public sealed class SensorAttachmentStore
{
    private readonly Dictionary<Guid, StoredAttachment> attachments = new();
    private readonly object syncRoot = new();

    public SensorAttachment Add(
        Guid sensorId,
        string fileName,
        byte[] content)
    {
        var metadata = new SensorAttachment
        {
            Id = Guid.NewGuid(),
            SensorId = sensorId,
            FileName = fileName,
            SizeBytes = content.LongLength,
            UploadedAtUtc = DateTimeOffset.UtcNow
        };

        lock (syncRoot)
        {
            attachments.Add(
                metadata.Id,
                new StoredAttachment(metadata, content));
        }

        return metadata;
    }

    public SensorAttachment[] GetAll(Guid sensorId)
    {
        lock (syncRoot)
        {
            return attachments.Values
                .Where(attachment => attachment.Metadata.SensorId == sensorId)
                .Select(attachment => attachment.Metadata)
                .OrderByDescending(attachment => attachment.UploadedAtUtc)
                .ToArray();
        }
    }

    public StoredAttachment? GetById(Guid sensorId, Guid attachmentId)
    {
        lock (syncRoot)
        {
            if (!attachments.TryGetValue(attachmentId, out var attachment))
            {
                return null;
            }

            return attachment.Metadata.SensorId == sensorId
                ? attachment
                : null;
        }
    }

    public sealed record StoredAttachment(
        SensorAttachment Metadata,
        byte[] Content);
}