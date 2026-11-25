using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Fleet.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Fleet.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Fleet.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.Fleet.Interfaces.REST.Transform;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.FleetUsage;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest;

namespace RebooTech.Fruitlogix.Platform.Fleet.Interfaces.REST;

/// <summary>Choferes de la flota. Solo Distributor y Admin; Producer y CommercialClient reciben 403.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[DomainExceptionFilter]
public class DriversController(
    IDriverCommandService commandService,
    IDriverQueryService queryService,
    IFleetUsageService fleetUsage) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DriverResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDrivers()
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!FleetAccess.CanManage(me)) return FleetAccess.Forbidden(this);

        var drivers = await queryService.Handle(new GetAllDriversQuery());
        return Ok(drivers.Select(FleetResourceAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DriverResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDriverById(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!FleetAccess.CanManage(me)) return FleetAccess.Forbidden(this);

        var driver = await queryService.Handle(new GetDriverByIdQuery(id));
        return driver is null
            ? NotFound(new { message = $"Driver {id} not found." })
            : Ok(FleetResourceAssembler.ToResourceFromEntity(driver));
    }

    [HttpPost]
    [ProducesResponseType(typeof(DriverResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateDriver([FromBody] SaveDriverResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!FleetAccess.CanManage(me)) return FleetAccess.Forbidden(this);

        var driver = await commandService.Handle(FleetResourceAssembler.ToCreateCommand(resource, me.Id));
        var result = FleetResourceAssembler.ToResourceFromEntity(driver);
        return CreatedAtAction(nameof(GetDriverById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(DriverResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDriver(int id, [FromBody] SaveDriverResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!FleetAccess.CanManage(me)) return FleetAccess.Forbidden(this);

        var driver = await commandService.Handle(FleetResourceAssembler.ToUpdateCommand(id, resource));
        return driver is null
            ? NotFound(new { message = $"Driver {id} not found." })
            : Ok(FleetResourceAssembler.ToResourceFromEntity(driver));
    }

    /// <summary>Borra el chofer. 409 si está asignado a una entrega activa.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteDriver(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!FleetAccess.CanManage(me)) return FleetAccess.Forbidden(this);

        if (await fleetUsage.FindActiveDeliveryForDriverAsync(id) is { } deliveryId)
            return Conflict(new { message = $"Driver {id} is assigned to active delivery {deliveryId} and cannot be deleted." });

        return await commandService.Handle(new DeleteDriverCommand(id))
            ? NoContent()
            : NotFound(new { message = $"Driver {id} not found." });
    }
}

/// <summary>Regla de acceso de la flota: Distributor y Admin gestionan todo; los demás roles reciben 403.</summary>
internal static class FleetAccess
{
    public static bool CanManage(CurrentUser me) => me.IsDistributor || me.IsAdmin;

    public static ObjectResult Forbidden(ControllerBase controller) =>
        controller.StatusCode(StatusCodes.Status403Forbidden,
            new { message = "Only distributors or admins can manage the fleet." });
}
