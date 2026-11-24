using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
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
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveryCommandService _deliveryCommandService;
    private readonly IDeliveryQueryService   _deliveryQueryService;
    private readonly IAccessScopeService     _accessScope;

    public DeliveriesController(
        IDeliveryCommandService deliveryCommandService,
        IDeliveryQueryService deliveryQueryService,
        IAccessScopeService accessScope)
    {
        _accessScope            = accessScope;
        _deliveryCommandService = deliveryCommandService;
        _deliveryQueryService   = deliveryQueryService;
    }

    /// <summary>Creates a new delivery.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(DeliveryResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDelivery([FromBody] CreateDeliveryResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var command  = CreateDeliveryCommandFromResourceAssembler.ToCommandFromResource(resource);
        // distributor_user_id: el Distributor que la registra; el valor del body solo se acepta de un Admin.
        command = command with
        {
            DistributorUserId = me.IsAdmin ? resource.DistributorUserId : me.IsDistributor ? me.Id : null
        };
        var delivery = await _deliveryCommandService.Handle(command);
        var result   = DeliveryResourceFromEntityAssembler.ToResourceFromEntity(delivery);
        return CreatedAtAction(nameof(CreateDelivery), new { id = result.Id }, result);
    }

    /// <summary>Returns all deliveries.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DeliveryResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllDeliveries()
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var query      = new GetAllDeliveriesQuery();
        var deliveries = await _deliveryQueryService.Handle(query);
        var visible    = await _accessScope.GetVisibleDeliveryIdsAsync(me);
        var result     = deliveries.WhereVisible(visible, d => d.Id).Select(
            DeliveryResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(result);
    }

    /// <summary>Starts the dispatch of a delivery.</summary>
    [HttpPost("{id:int}/start-dispatch")]
    [ProducesResponseType(typeof(DeliveryResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartDispatch(int id)
    {
        var command  = new StartDispatchCommand(id);
        var delivery = await _deliveryCommandService.Handle(command);
        return Ok(DeliveryResourceFromEntityAssembler.ToResourceFromEntity(delivery));
    }

    /// <summary>Reports a delay for a delivery.</summary>
    [HttpPost("{id:int}/report-delay")]
    [ProducesResponseType(typeof(DeliveryResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReportDelay(int id, [FromBody] ReportDelayResource resource)
    {
        var command  = new ReportDelayCommand(id, resource.Reason);
        var delivery = await _deliveryCommandService.Handle(command);
        return Ok(DeliveryResourceFromEntityAssembler.ToResourceFromEntity(delivery));
    }
}