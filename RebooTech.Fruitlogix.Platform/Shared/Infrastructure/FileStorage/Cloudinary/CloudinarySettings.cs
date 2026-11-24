namespace RebooTech.Fruitlogix.Platform.Shared.Infrastructure.FileStorage.Cloudinary;

/// <summary>
///     Sección "Cloudinary" de la configuración. Los valores van en user-secrets (local)
///     o en variables de entorno de Render: Cloudinary__CloudName, Cloudinary__ApiKey, Cloudinary__ApiSecret.
/// </summary>
public class CloudinarySettings
{
    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    /// <summary>Carpeta raíz en Cloudinary.</summary>
    public string RootFolder { get; set; } = "fruitlogix";

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(CloudName) &&
        !string.IsNullOrWhiteSpace(ApiKey) &&
        !string.IsNullOrWhiteSpace(ApiSecret);
}
