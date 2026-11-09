using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.Api.Services;

/// <summary>Stores images in Azure Blob Storage. The MVC app never talks to Azure directly.</summary>
public class AzureBlobImageStorage : IImageStorage
{
    private readonly BlobContainerClient _container;

    public AzureBlobImageStorage(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlob:ConnectionString"]
            ?? throw new InvalidOperationException("AzureBlob:ConnectionString is not configured.");
        var containerName = configuration["AzureBlob:ContainerName"] ?? "raceday-images";
        _container = new BlobContainerClient(connectionString, containerName);
    }

    public async Task<string> UploadAsync(Stream content, string fileName, string contentType, string folder, CancellationToken cancellationToken = default)
    {
        await _container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension)) extension = ".bin";
        var blobName = $"{folder}/{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

        var blob = _container.GetBlobClient(blobName);
        await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);
        return blob.Uri.ToString();
    }
}
