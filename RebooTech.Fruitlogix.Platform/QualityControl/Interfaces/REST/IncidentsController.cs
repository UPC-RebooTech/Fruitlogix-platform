using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Transform;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Shared.Application.Internal.OutboundServices;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[DomainExceptionFilter]
public class IncidentsController(
    IIncidentCommandService commandService,
    IIncidentQueryService queryService,
    IAccessScopeService accessScope,
    IFileStorageService fileStorage
) : ControllerBase
{
    // GET api/v1/incidents
    [HttpGet]
    public async Task<IActionResult> GetAllIncidents()
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var query     = new GetAllIncidentsQuery();
        var incidents = await queryService.Handle(query);
        var filter    = await accessScope.GetIncidentFilterAsync(me);
        var result    = (filter is null ? incidents : incidents.Where(filter)).Select(IncidentResourceAssembler.ToResourceFromEntity);
        return Ok(result);
    }

    // POST api/v1/incidents
    [HttpPost]
    public async Task<IActionResult> CreateIncident([FromBody] CreateIncidentResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var command  = IncidentResourceAssembler.ToCommandFromResource(resource)
            with { ReportedByUserId = me.Id, OrderId = resource.OrderId };
        var incident = await commandService.Handle(command);
        var result   = IncidentResourceAssembler.ToResourceFromEntity(incident);
        return CreatedAtAction(nameof(CreateIncident), new { id = result.Id }, result);
    }
    
    // POST api/v1/incidents/{id}/evidence  (multipart, campo "files", 1 a 5 imágenes)
    /// <summary>
    ///     Sube imágenes de evidencia (jpg, png o webp, máximo 5 MB cada una y 5 por incidencia)
    ///     y agrega sus URLs a evidenceUrls. Pueden subir quien reportó la incidencia, Distributor y Admin.
    /// </summary>
    [HttpPost("{id:int}/evidence")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(EvidenceRequestLimit)]
    [RequestFormLimits(MultipartBodyLengthLimit = EvidenceRequestLimit)]
    [ProducesResponseType(typeof(IncidentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> UploadEvidence(int id, List<IFormFile> files, CancellationToken cancellationToken)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();

        var incident = await queryService.Handle(new GetIncidentByIdQuery(id));
        var filter = await accessScope.GetIncidentFilterAsync(me);
        if (incident is null || (filter is not null && !filter(incident))) return NotFound();
        if (!(me.IsAdmin || me.IsDistributor || incident.ReportedByUserId == me.Id))
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = "Only the reporter, a distributor or an admin can add evidence." });

        if (files is null || files.Count == 0)
            return BadRequest(new { message = "At least one file is required (multipart field 'files')." });
        if (files.Count > incident.RemainingEvidenceSlots)
            return Conflict(new
            {
                message = $"An incident can have at most {Incident.MaxEvidenceFiles} evidence files; "
                          + $"{incident.RemainingEvidenceSlots} slot(s) left."
            });

        foreach (var file in files)
            if (await ImageUploadValidator.ValidateAsync(file, cancellationToken) is { } error)
                return BadRequest(new { message = error });

        var urls = new List<string>();
        try
        {
            foreach (var file in files)
            {
                await using var stream = file.OpenReadStream();
                urls.Add(await fileStorage.UploadImageAsync(stream, file.FileName, $"incidents/{id}", cancellationToken));
            }
        }
        catch (FileStorageException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }

        var updated = await commandService.Handle(new AddIncidentEvidenceCommand(id, urls));
        if (updated is null) return NotFound();
        return Ok(IncidentResourceAssembler.ToResourceFromEntity(updated));
    }

    private const long EvidenceRequestLimit = Incident.MaxEvidenceFiles * ImageUploadValidator.MaxBytes + 1024 * 1024;

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateIncidentStatus(
        int id, [FromBody] UpdateIncidentStatusResource resource)
    {
        try
        {
            var command  = IncidentResourceAssembler.ToCommandFromResource(id, resource);
            var incident = await commandService.Handle(command);

            if (incident is null)
                return NotFound($"Incident with id {id} not found.");

            return Ok(IncidentResourceAssembler.ToResourceFromEntity(incident));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    
}