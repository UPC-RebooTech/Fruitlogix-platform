namespace RebooTech.Fruitlogix.Platform.Shared.Application.Internal.OutboundServices;

/// <summary>
///     Almacenamiento de archivos públicos (imágenes de evidencia, fotos de productos...).
///     La implementación actual usa Cloudinary.
/// </summary>
public interface IFileStorageService
{
    /// <summary>Sube una imagen y devuelve su URL pública (https).</summary>
    /// <param name="content">Contenido del archivo.</param>
    /// <param name="fileName">Nombre original (solo como referencia).</param>
    /// <param name="folder">Subcarpeta lógica, p. ej. "incidents/12".</param>
    /// <exception cref="FileStorageException">El almacenamiento no está configurado o la subida falló.</exception>
    Task<string> UploadImageAsync(Stream content, string fileName, string folder,
        CancellationToken cancellationToken = default);
}

/// <summary>Error del almacenamiento externo, con el código HTTP que debe devolver la API.</summary>
public class FileStorageException(string message, int statusCode, Exception? innerException = null)
    : Exception(message, innerException)
{
    /// <summary>503 si no está configurado, 502 si el proveedor rechazó o falló la subida.</summary>
    public int StatusCode { get; } = statusCode;
}
