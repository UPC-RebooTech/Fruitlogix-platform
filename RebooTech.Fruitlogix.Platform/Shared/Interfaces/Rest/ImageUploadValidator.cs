namespace RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest;

/// <summary>
///     Valida imágenes subidas por multipart: jpg, png o webp, máximo <see cref="MaxBytes" />.
///     Revisa la extensión, el Content-Type y la firma real del archivo (magic bytes).
/// </summary>
public static class ImageUploadValidator
{
    public const long MaxBytes = 5 * 1024 * 1024;

    private static readonly Dictionary<string, string[]> ExtensionsByContentType = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = [".jpg", ".jpeg"],
        ["image/png"] = [".png"],
        ["image/webp"] = [".webp"]
    };

    /// <summary>null si el archivo es válido; si no, el mensaje de error.</summary>
    public static async Task<string?> ValidateAsync(IFormFile? file, CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
            return "The file is empty.";

        var name = string.IsNullOrWhiteSpace(file.FileName) ? "file" : file.FileName;

        if (file.Length > MaxBytes)
            return $"'{name}' exceeds the {MaxBytes / (1024 * 1024)} MB limit.";

        var extension = Path.GetExtension(name);
        if (!ExtensionsByContentType.TryGetValue(file.ContentType ?? string.Empty, out var extensions)
            || !extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            return $"'{name}' must be a JPG, PNG or WEBP image.";

        var header = new byte[12];
        await using var stream = file.OpenReadStream();
        var read = await stream.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false, cancellationToken);
        if (!MatchesSignature(header.AsSpan(0, read), file.ContentType!))
            return $"'{name}' content does not match its image type.";

        return null;
    }

    private static bool MatchesSignature(ReadOnlySpan<byte> header, string contentType) =>
        contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            "image/png" => header.Length >= 8 && header[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            "image/webp" => header.Length >= 12
                            && header[..4].SequenceEqual("RIFF"u8)
                            && header[8..12].SequenceEqual("WEBP"u8),
            _ => false
        };
}
