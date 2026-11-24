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
    public async Task<IActionResult> GetAllProducers()
    {
        var query     = new GetAllProducersQuery();
        var producers = await producerQueryService.Handle(query);
        var result    = producers.Select(ProducerResourceAssembler.ToResourceFromEntity);
        return Ok(result);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProducerById(int id)
    {
        var query    = new GetProducerByIdQuery(id);
        var producer = await producerQueryService.Handle(query);
        if (producer is null) return NotFound(new { message = $"Producer with id {id} not found." });
        return Ok(ProducerResourceAssembler.ToResourceFromEntity(producer));
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProducer(int id, [FromBody] UpdateProducerResource resource)
    {
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
    public async Task<IActionResult> DeleteProducer(int id)
    {
        try
        {
            await producerCommandService.Handle(new DeleteProducerCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}