using Microsoft.AspNetCore.DataProtection;
using SmartX.Shared.Models;

namespace SmartX.Api.Services;

public sealed class SensorAttachmentStore
{
    private readonly Dictionary<Guid, EncryptedAttachment> attachments = new();
    private readonly object syncRoot = new();
    private readonly IDataProtector protector;

    public SensorAttachmentStore(IDataProtectionProvider provider)
    {
        protector = provider.CreateProtector(
            "SmartX.SensorAttachments.v1");
    }

    public SensorAttachment Add(
        Guid sensorId,
        string fileName,
        byte[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        var metadata = new SensorAttachment
        {
            Id = Guid.NewGuid(),
            SensorId = sensorId,
            FileName = fileName,
            SizeBytes = content.LongLength,
            UploadedAtUtc = DateTimeOffset.UtcNow
        };

        var fileProtector = CreateFileProtector(
            sensorId,
            metadata.Id);

        var protectedContent = fileProtector.Protect(content);

        lock (syncRoot)
        {
            attachments.Add(
                metadata.Id,
                new EncryptedAttachment(
                    metadata,
                    protectedContent));
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

    public StoredAttachment? GetById(
        Guid sensorId,
        Guid attachmentId)
    {
        EncryptedAttachment attachment;

        lock (syncRoot)
        {
            if (!attachments.TryGetValue(attachmentId, out var stored) ||
                stored.Metadata.SensorId != sensorId)
            {
                return null;
            }

            attachment = stored;
        }

        var fileProtector = CreateFileProtector(
            sensorId,
            attachmentId);

        var content = fileProtector.Unprotect(
            attachment.ProtectedContent);

        return new StoredAttachment(
            attachment.Metadata,
            content);
    }

    private IDataProtector CreateFileProtector(
        Guid sensorId,
        Guid attachmentId)
    {
        return protector.CreateProtector(
            $"{sensorId:N}:{attachmentId:N}");
    }

    private sealed record EncryptedAttachment(
        SensorAttachment Metadata,
        byte[] ProtectedContent);

    public sealed record StoredAttachment(
        SensorAttachment Metadata,
        byte[] Content);
}