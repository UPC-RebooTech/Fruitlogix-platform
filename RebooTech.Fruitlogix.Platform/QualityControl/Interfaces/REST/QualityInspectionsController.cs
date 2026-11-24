using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Transform;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[DomainExceptionFilter]
public class QualityInspectionsController(
    IQualityInspectionCommandService commandService,
    IQualityInspectionQueryService   queryService,
    IHarvestBatchQueryService        harvestBatchQueryService,
    IAccessScopeService              accessScope
) : ControllerBase
{
    // GET api/v1/quality-inspections/batch/{batchId}
    [HttpGet("batch/{batchId:int}")]
    public async Task<IActionResult> GetByBatchId(int batchId)
    {
        var query      = new GetQualityInspectionByBatchIdQuery(batchId);
        var inspection = await queryService.Handle(query);

        if (inspection is null)
            return NotFound($"No quality inspection found for batch {batchId}.");

        return Ok(QualityInspectionResourceAssembler.ToResourceFromEntity(inspection));
    }

    // POST api/v1/quality-inspections
    [HttpPost]
    [ProducesResponseType(typeof(QualityInspectionResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateQualityInspection(
        [FromBody] CreateQualityInspectionResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient)
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = "Only the batch producer, a distributor or an admin can register an inspection." });

        // Admin/Distributor: cualquier lote. Producer: solo los de su perfil.
        var batch = await harvestBatchQueryService.Handle(new GetHarvestBatchByIdQuery(resource.BatchId));
        if (batch is null || (me.IsProducer && await accessScope.GetProducerIdAsync(me) != batch.ProducerId))
            return NotFound(new { message = $"HarvestBatch with id {resource.BatchId} not found." });

        var command    = QualityInspectionResourceAssembler.ToCommandFromResource(resource) with
        {
            InspectorUserId = me.Id,
            InspectorName   = string.IsNullOrWhiteSpace(resource.InspectorName) ? me.Username : resource.InspectorName
        };
        var inspection = await commandService.Handle(command);
        var result     = QualityInspectionResourceAssembler.ToResourceFromEntity(inspection);

        return CreatedAtAction(nameof(GetByBatchId),
            new { batchId = result.BatchId }, result);
    }
}