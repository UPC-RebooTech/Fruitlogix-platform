using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Logistics.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Transform;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest;

namespace RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST;

[ApiController]
[Route("api/v1/tracking-logs")]
[Produces(MediaTypeNames.Application.Json)]
[DomainExceptionFilter]
public class TrackingLogsController : ControllerBase
{
    private readonly ITrackingLogCommandService _trackingLogCommandService;
    private readonly ITrackingLogQueryService   _trackingLogQueryService;
    private readonly IAccessScopeService        _accessScope;

    public TrackingLogsController(
        ITrackingLogCommandService trackingLogCommandService,
        ITrackingLogQueryService   trackingLogQueryService,
        IAccessScopeService        accessScope)
    {
        _accessScope               = accessScope;
        _trackingLogCommandService = trackingLogCommandService;
        _trackingLogQueryService   = trackingLogQueryService;
    }

    /// <summary>Receives a telemetry reading from an IoT device.</summary>
    /// <remarks>Latitude (-90..90) y longitude (-180..180) son obligatorias; fuera de rango responde 400.</remarks>
    [HttpPost]
    [ProducesResponseType(typeof(TrackingLogResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTrackingLog([FromBody] CreateTrackingLogResource resource)
    {
        var command = CreateTrackingLogCommandFromResourceAssembler.ToCommandFromResource(resource);
        var log     = await _trackingLogCommandService.Handle(command);
        var result  = TrackingLogResourceFromEntityAssembler.ToResourceFromEntity(log);
        return CreatedAtAction(nameof(CreateTrackingLog), new { id = result.Id }, result);
    }

    /// <summary>Returns all tracking logs for a specific delivery.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TrackingLogResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrackingLogsByDeliveryId([FromQuery] int deliveryId)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var visible = await _accessScope.GetVisibleDeliveryIdsAsync(me);
        if (visible is not null && !visible.Contains(deliveryId))
            return Ok(Array.Empty<TrackingLogResource>());

        var query  = new GetTrackingLogsByDeliveryIdQuery(deliveryId);
        var logs   = await _trackingLogQueryService.Handle(query);
        var result = logs.Select(TrackingLogResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(result);
    }
}