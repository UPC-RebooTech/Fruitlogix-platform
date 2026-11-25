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

/// <summary>Vehículos refrigerados de la flota. Solo Distributor y Admin; Producer y CommercialClient reciben 403.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[DomainExceptionFilter]
public class VehiclesController(
    IVehicleCommandService commandService,
    IVehicleQueryService queryService,
    IFleetUsageService fleetUsage) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetVehicles()
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!FleetAccess.CanManage(me)) return FleetAccess.Forbidden(this);

        var vehicles = await queryService.Handle(new GetAllVehiclesQuery());
        return Ok(vehicles.Select(FleetResourceAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VehicleResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVehicleById(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!FleetAccess.CanManage(me)) return FleetAccess.Forbidden(this);

        var vehicle = await queryService.Handle(new GetVehicleByIdQuery(id));
        return vehicle is null
            ? NotFound(new { message = $"Vehicle {id} not found." })
            : Ok(FleetResourceAssembler.ToResourceFromEntity(vehicle));
    }

    /// <summary>Registra un vehículo. 409 si la placa ya existe.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateVehicle([FromBody] SaveVehicleResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!FleetAccess.CanManage(me)) return FleetAccess.Forbidden(this);

        var vehicle = await commandService.Handle(FleetResourceAssembler.ToCreateCommand(resource, me.Id));
        var result = FleetResourceAssembler.ToResourceFromEntity(vehicle);
        return CreatedAtAction(nameof(GetVehicleById), new { id = result.Id }, result);
    }

    /// <summary>Reemplaza los datos del vehículo. 409 si la placa ya la usa otra unidad.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(VehicleResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateVehicle(int id, [FromBody] SaveVehicleResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!FleetAccess.CanManage(me)) return FleetAccess.Forbidden(this);

        var vehicle = await commandService.Handle(FleetResourceAssembler.ToUpdateCommand(id, resource));
        return vehicle is null
            ? NotFound(new { message = $"Vehicle {id} not found." })
            : Ok(FleetResourceAssembler.ToResourceFromEntity(vehicle));
    }

    /// <summary>Borra el vehículo. 409 si está asignado a una entrega activa.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!FleetAccess.CanManage(me)) return FleetAccess.Forbidden(this);

        if (await fleetUsage.FindActiveDeliveryForVehicleAsync(id) is { } deliveryId)
            return Conflict(new { message = $"Vehicle {id} is assigned to active delivery {deliveryId} and cannot be deleted." });

        return await commandService.Handle(new DeleteVehicleCommand(id))
            ? NoContent()
            : NotFound(new { message = $"Vehicle {id} not found." });
    }
}
