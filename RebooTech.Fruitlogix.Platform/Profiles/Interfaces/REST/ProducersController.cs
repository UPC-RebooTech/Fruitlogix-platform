using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.Profiles.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Profiles.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Profiles.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.Profiles.Interfaces.REST.Transform;

namespace RebooTech.Fruitlogix.Platform.Profiles.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
public class ProducersController(
    IProducerCommandService producerCommandService,
    IProducerQueryService producerQueryService,
    IAccessScopeService accessScope) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ProducerResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateProducer([FromBody] CreateProducerResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        // Una cuenta Producer = un perfil de productor (producers.user_id). Otros roles crean perfiles sin dueño.
        if (me.IsProducer && await accessScope.GetProducerIdAsync(me) is not null)
            return Conflict(new { message = "This user already has a producer profile." });

        try
        {
            var command  = ProducerResourceAssembler.ToCommandFromResource(resource)
                with { UserId = me.IsProducer ? me.Id : null };
            var producer = await producerCommandService.Handle(command);
            var result   = ProducerResourceAssembler.ToResourceFromEntity(producer);
            return CreatedAtAction(nameof(CreateProducer), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        catch (ArgumentException ex)         { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProducerResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProducers()
    {
        var query     = new GetAllProducersQuery();
        var producers = await producerQueryService.Handle(query);
        var result    = producers.Select(ProducerResourceAssembler.ToResourceFromEntity);
        return Ok(result);
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProducerResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProducerById(int id)
    {
        var query    = new GetProducerByIdQuery(id);
        var producer = await producerQueryService.Handle(query);
        if (producer is null) return NotFound(new { message = $"Producer with id {id} not found." });
        return Ok(ProducerResourceAssembler.ToResourceFromEntity(producer));
    }
    
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProducerResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateProducer(int id, [FromBody] UpdateProducerResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only the producer, a distributor or an admin can change a producer profile." });
        if (!await CanManageProducerAsync(me, id)) return NotFound(new { message = $"Producer with id {id} not found." });

        try
        {
            var command  = ProducerResourceAssembler.ToCommandFromResource(id, resource);
            var producer = await producerCommandService.Handle(command);
            var result   = ProducerResourceAssembler.ToResourceFromEntity(producer);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)      { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        catch (ArgumentException ex)         { return BadRequest(new { message = ex.Message }); }
    }
    
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProducer(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only the producer, a distributor or an admin can delete a producer profile." });
        if (!await CanManageProducerAsync(me, id)) return NotFound(new { message = $"Producer with id {id} not found." });

        try
        {
            await producerCommandService.Handle(new DeleteProducerCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    /// <summary>Admin y Distributor: cualquier perfil. Producer: solo el suyo (producers.user_id).</summary>
    private async Task<bool> CanManageProducerAsync(CurrentUser me, int producerId)
    {
        if (me.IsAdmin || me.IsDistributor) return true;
        return me.IsProducer && await accessScope.GetProducerIdAsync(me) == producerId;
    }
}
