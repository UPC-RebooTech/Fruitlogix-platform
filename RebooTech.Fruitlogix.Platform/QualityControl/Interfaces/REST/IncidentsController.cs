using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Transform;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
public class IncidentsController(
    IIncidentCommandService commandService,
    IIncidentQueryService queryService,
    IAccessScopeService accessScope
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