namespace RaceDay.Api.Services;

public interface IImageStorage
{
    Task<string> UploadAsync(Stream content, string fileName, string contentType, string folder, CancellationToken cancellationToken = default);
}
