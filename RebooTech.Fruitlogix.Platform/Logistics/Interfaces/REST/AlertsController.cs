using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Logistics.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Transform;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;

namespace RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class AlertsController : ControllerBase
{
    private readonly IAlertCommandService _alertCommandService;
    private readonly IAlertQueryService   _alertQueryService;
    private readonly IAccessScopeService  _accessScope;

    public AlertsController(
        IAlertCommandService alertCommandService,
        IAlertQueryService   alertQueryService,
        IAccessScopeService  accessScope)
    {
        _accessScope         = accessScope;
        _alertCommandService = alertCommandService;
        _alertQueryService   = alertQueryService;
    }

    /// <summary>Returns unresolved alerts visible to the user (clients and producers: only their deliveries).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveAlerts()
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var query  = new GetAllActiveAlertsQuery();
        var alerts = await _alertQueryService.Handle(query);
        var visible = await _accessScope.GetVisibleDeliveryIdsAsync(me);
        var result = alerts.WhereVisible(visible, a => a.DeliveryId).Select(AlertResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(result);
    }

    /// <summary>Resolves an existing alert.</summary>
    [HttpPut("{id:int}/resolve")]
    [ProducesResponseType(typeof(AlertResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveAlert(int id)
    {
        var command = new ResolveAlertCommand(id);
        var alert   = await _alertCommandService.Handle(command);
        return Ok(AlertResourceFromEntityAssembler.ToResourceFromEntity(alert));
    }
}