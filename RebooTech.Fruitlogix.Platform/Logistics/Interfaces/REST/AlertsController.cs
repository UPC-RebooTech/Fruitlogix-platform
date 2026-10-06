using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Logistics.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Transform;

namespace RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class AlertsController : ControllerBase
{
    private readonly IAlertCommandService _alertCommandService;
    private readonly IAlertQueryService   _alertQueryService;

    public AlertsController(
        IAlertCommandService alertCommandService,
        IAlertQueryService   alertQueryService)
    {
        _alertCommandService = alertCommandService;
        _alertQueryService   = alertQueryService;
    }

    /// <summary>Returns all unresolved alerts.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveAlerts()
    {
        var query  = new GetAllActiveAlertsQuery();
        var alerts = await _alertQueryService.Handle(query);
        var result = alerts.Select(AlertResourceFromEntityAssembler.ToResourceFromEntity);
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