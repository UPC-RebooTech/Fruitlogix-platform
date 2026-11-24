using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Profiles.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Transform;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
public class HarvestBatchesController(
    IHarvestBatchCommandService commandService,
    IHarvestBatchQueryService queryService,
    IProducerQueryService producerQueryService,
    IAccessScopeService accessScope
) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HarvestBatchResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllHarvestBatches()
    {
        var query   = new GetAllHarvestBatchesQuery();
        var batches = await queryService.Handle(query);
        var result  = batches.Select(HarvestBatchResourceAssembler.ToResourceFromEntity);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HarvestBatchResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateHarvestBatch([FromBody] CreateHarvestBatchResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only producers, distributors or admins can register harvest batches." });

        var command = HarvestBatchResourceAssembler.ToCommandFromResource(resource);
        if (me.IsProducer)
        {
            // Un Producer registra lotes solo en su propio perfil: se ignora el producerId del body.
            if (await accessScope.GetProducerIdAsync(me) is not { } ownProducerId)
                return Conflict(new { message = "Create your producer profile first (POST /api/v1/producers)." });
            command = command with { ProducerId = ownProducerId };
        }
        else if (await producerQueryService.Handle(new GetProducerByIdQuery(command.ProducerId)) is null)
        {
            return BadRequest(new { message = $"Producer {command.ProducerId} not found." });
        }

        var batch   = await commandService.Handle(command);
        var result  = HarvestBatchResourceAssembler.ToResourceFromEntity(batch);
        return CreatedAtAction(nameof(CreateHarvestBatch), new { id = result.Id }, result);
    }
    
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(HarvestBatchResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateHarvestBatchStatus(
        int id, [FromBody] UpdateHarvestBatchStatusResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only the batch producer, a distributor or an admin can change a batch status." });
        var existing = await queryService.Handle(new GetHarvestBatchByIdQuery(id));
        if (existing is null || !await CanManageBatchAsync(me, existing))
            return NotFound($"HarvestBatch with id {id} not found.");

        try
        {
            var command = HarvestBatchResourceAssembler.ToCommandFromResource(id, resource);
            var batch   = await commandService.Handle(command);

            if (batch is null)
                return NotFound($"HarvestBatch with id {id} not found.");

            return Ok(HarvestBatchResourceAssembler.ToResourceFromEntity(batch));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Admin y Distributor: cualquier lote. Producer: solo los de su perfil.</summary>
    private async Task<bool> CanManageBatchAsync(CurrentUser me, HarvestBatch batch)
    {
        if (me.IsAdmin || me.IsDistributor) return true;
        return me.IsProducer && await accessScope.GetProducerIdAsync(me) == batch.ProducerId;
    }
}
