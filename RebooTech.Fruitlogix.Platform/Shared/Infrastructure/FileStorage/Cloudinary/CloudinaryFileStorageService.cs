using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using RebooTech.Fruitlogix.Platform.Shared.Application.Internal.OutboundServices;

namespace RebooTech.Fruitlogix.Platform.Shared.Infrastructure.FileStorage.Cloudinary;

public class CloudinaryFileStorageService : IFileStorageService
{
    private readonly CloudinaryDotNet.Cloudinary? _cloudinary;
    private readonly string _rootFolder;
    private readonly ILogger<CloudinaryFileStorageService> _logger;

    public CloudinaryFileStorageService(IOptions<CloudinarySettings> options,
        ILogger<CloudinaryFileStorageService> logger)
    {
        _logger = logger;
        var settings = options.Value;
        _rootFolder = string.IsNullOrWhiteSpace(settings.RootFolder) ? "fruitlogix" : settings.RootFolder.Trim('/');

        if (!settings.IsConfigured)
        {
            _logger.LogWarning("Cloudinary is not configured: file uploads will return 503.");
            return;
        }

        _cloudinary = new CloudinaryDotNet.Cloudinary(
            new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret));
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadImageAsync(Stream content, string fileName, string folder,
        CancellationToken cancellationToken = default)
    {
        if (_cloudinary is null)
            throw new FileStorageException("File storage is not configured.", StatusCodes.Status503ServiceUnavailable);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, content),
            Folder = $"{_rootFolder}/{folder.Trim('/')}",
            UseFilename = false,
            UniqueFilename = true,
            Overwrite = false
        };

        ImageUploadResult result;
        try
        {
            result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Cloudinary upload failed for {FileName}", fileName);
            throw new FileStorageException("The file could not be uploaded.", StatusCodes.Status502BadGateway, ex);
        }

        if (result.Error is not null || result.SecureUrl is null)
        {
            _logger.LogError("Cloudinary rejected {FileName}: {Error}", fileName, result.Error?.Message);
            throw new FileStorageException("The file could not be uploaded.", StatusCodes.Status502BadGateway);
        }

        return result.SecureUrl.ToString();
    }
}
