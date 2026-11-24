using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Application.Internal.OutboundServices;

namespace RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest;

/// <summary>Respuesta de una subida: URL pública https del archivo.</summary>
public record UploadedFileResource(string Url, string FileName, long SizeBytes, string ContentType);

/// <summary>
///     Subida genérica de imágenes (p. ej. evidencia antes de crear la incidencia).
///     Devuelve la URL para enviarla luego en <c>evidenceUrls</c> de POST /incidents.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class FilesController(IFileStorageService fileStorage) : ControllerBase
{
    /// <summary>Sube una imagen (jpg, png o webp, máximo 5 MB). Campo multipart: <c>file</c>.</summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(ImageUploadValidator.MaxBytes + 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = ImageUploadValidator.MaxBytes + 1024 * 1024)]
    [ProducesResponseType(typeof(UploadedFileResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();

        if (await ImageUploadValidator.ValidateAsync(file, cancellationToken) is { } error)
            return BadRequest(new { message = error });

        try
        {
            await using var stream = file.OpenReadStream();
            var url = await fileStorage.UploadImageAsync(stream, file.FileName, $"uploads/{me.Id}", cancellationToken);
            return StatusCode(StatusCodes.Status201Created,
                new UploadedFileResource(url, file.FileName, file.Length, file.ContentType));
        }
        catch (FileStorageException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}
