namespace RaceDay.Api.Services;


public class LocalImageStorage : IImageStorage
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalImageStorage(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
    {
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> UploadAsync(Stream content, string fileName, string contentType, string folder, CancellationToken cancellationToken = default)
    {
        var webRoot = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRoot))
        {
            webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
            Directory.CreateDirectory(webRoot);
            _environment.WebRootPath = webRoot;
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension)) extension = ".bin";
        var storedName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var directory = Path.Combine(webRoot, "uploads", folder);
        Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, storedName);
        await using (var file = File.Create(path))
        {
            await content.CopyToAsync(file, cancellationToken);
        }

        var request = _httpContextAccessor.HttpContext?.Request;
        var prefix = request is null ? "" : $"{request.Scheme}://{request.Host}";
        return $"{prefix}/uploads/{folder}/{storedName}";
    }
}
