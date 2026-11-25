using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Logistics.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Transform;
using RebooTech.Fruitlogix.Platform.OrderManagement.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Fleet.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Queries;
using FleetDriver = RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Aggregates.Driver;
using FleetVehicle = RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Aggregates.Vehicle;

namespace RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[DomainExceptionFilter]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveryCommandService _deliveryCommandService;
    private readonly IDeliveryQueryService   _deliveryQueryService;
    private readonly IAccessScopeService     _accessScope;
    private readonly IOrderCommandService    _orderCommandService;
    private readonly IDriverQueryService     _driverQueryService;
    private readonly IVehicleQueryService    _vehicleQueryService;

    public DeliveriesController(
        IDeliveryCommandService deliveryCommandService,
        IDeliveryQueryService deliveryQueryService,
        IAccessScopeService accessScope,
        IOrderCommandService orderCommandService,
        IDriverQueryService driverQueryService,
        IVehicleQueryService vehicleQueryService)
    {
        _driverQueryService     = driverQueryService;
        _vehicleQueryService    = vehicleQueryService;
        _accessScope            = accessScope;
        _orderCommandService    = orderCommandService;
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
            DistributorUserId = me.IsAdmin ? resource.DistributorUserId : me.IsDistributor ? me.Id : null,
            ActorUserId       = me.Id
        };

        // Flota opcional: driverId / vehicleId completan los datos de texto del chofer y del vehículo.
        var (driver, vehicle, fleetError) = await ResolveFleetAsync(resource.DriverId, resource.VehicleId);
        if (fleetError is not null) return fleetError;
        command = command with
        {
            DriverId     = driver?.Id,
            VehicleId    = vehicle?.Id,
            DriverName   = driver?.Name ?? command.DriverName,
            DriverPhone  = driver is null ? command.DriverPhone : driver.Phone,
            VehiclePlate = vehicle?.Plate ?? command.VehiclePlate,
            VehicleType  = vehicle?.Type.ToString() ?? command.VehicleType
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
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient)
            return StatusCode(403, new { message = "Only a distributor, the order's producer or an admin can update a delivery." });
        var visible = await _accessScope.GetVisibleDeliveryIdsAsync(me);
        if (visible is not null && !visible.Contains(id)) return NotFound();

        var command  = new StartDispatchCommand(id, me.Id);
        var delivery = await _deliveryCommandService.Handle(command);
        // El pedido acompaña a la entrega: Pending/InPreparation pasan a InTransit (requisito de la recepción).
        await _orderCommandService.Handle(new MarkOrderDispatchedCommand(delivery.OrderId));
        return Ok(DeliveryResourceFromEntityAssembler.ToResourceFromEntity(delivery));
    }

    /// <summary>Reports a delay for a delivery.</summary>
    [HttpPost("{id:int}/report-delay")]
    [ProducesResponseType(typeof(DeliveryResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReportDelay(int id, [FromBody] ReportDelayResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient)
            return StatusCode(403, new { message = "Only a distributor, the order's producer or an admin can update a delivery." });
        var visible = await _accessScope.GetVisibleDeliveryIdsAsync(me);
        if (visible is not null && !visible.Contains(id)) return NotFound();

        var command  = new ReportDelayCommand(id, resource.Reason, me.Id);
        var delivery = await _deliveryCommandService.Handle(command);
        return Ok(DeliveryResourceFromEntityAssembler.ToResourceFromEntity(delivery));
    }

    /// <summary>Entrega por id (según el rol: Admin/Distributor todas, cliente y productor las de sus pedidos).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DeliveryResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeliveryById(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!await CanSeeDeliveryAsync(me, id)) return NotFound();
        var delivery = await _deliveryQueryService.Handle(new GetDeliveryByIdQuery(id));
        if (delivery is null) return NotFound();
        return Ok(DeliveryResourceFromEntityAssembler.ToResourceFromEntity(delivery));
    }

    /// <summary>Entregas de un pedido (normalmente una). Lista vacía si el pedido no es visible.</summary>
    [HttpGet("order/{orderId:int}")]
    [ProducesResponseType(typeof(IEnumerable<DeliveryResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDeliveriesByOrderId(int orderId)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var deliveries = await _deliveryQueryService.Handle(new GetDeliveriesByOrderIdQuery(orderId));
        var visible    = await _accessScope.GetVisibleDeliveryIdsAsync(me);
        return Ok(deliveries.WhereVisible(visible, d => d.Id)
            .Select(DeliveryResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Historial de la entrega: creada, despacho, retrasos, puntos de control, llegada y entrega.</summary>
    [HttpGet("{id:int}/events")]
    [ProducesResponseType(typeof(IEnumerable<DeliveryEventResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeliveryEvents(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!await CanSeeDeliveryAsync(me, id)) return NotFound();
        if (await _deliveryQueryService.Handle(new GetDeliveryByIdQuery(id)) is null) return NotFound();
        var events = await _deliveryQueryService.Handle(new GetDeliveryEventsQuery(id));
        return Ok(events.Select(DeliveryEventResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Marca la llegada al destino (InTransit o Delayed → Arrived). Body opcional con coordenadas y nota.</summary>
    [HttpPost("{id:int}/arrive")]
    [ProducesResponseType(typeof(DeliveryResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Arrive(int id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ArriveDeliveryResource? resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient)
            return StatusCode(403, new { message = "Only a distributor, the order's producer or an admin can update a delivery." });
        if (!await CanSeeDeliveryAsync(me, id)) return NotFound();

        var delivery = await _deliveryCommandService.Handle(new ArriveDeliveryCommand(
            id, me.Id, resource?.Latitude, resource?.Longitude, resource?.Note));
        return Ok(DeliveryResourceFromEntityAssembler.ToResourceFromEntity(delivery));
    }

    /// <summary>Asigna o cambia conductor, teléfono, vehículo y placa (antes de la llegada).</summary>
    [HttpPatch("{id:int}/assignment")]
    [ProducesResponseType(typeof(DeliveryResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAssignment(int id, [FromBody] UpdateDeliveryAssignmentResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient)
            return StatusCode(403, new { message = "Only a distributor, the order's producer or an admin can update a delivery." });
        if (!await CanSeeDeliveryAsync(me, id)) return NotFound();

        var (driver, vehicle, fleetError) = await ResolveFleetAsync(resource.DriverId, resource.VehicleId);
        if (fleetError is not null) return fleetError;

        var delivery = await _deliveryCommandService.Handle(new UpdateDeliveryAssignmentCommand(
            id,
            driver?.Name ?? resource.DriverName,
            driver is null ? resource.DriverPhone : driver.Phone ?? string.Empty,
            vehicle?.Plate ?? resource.VehiclePlate,
            vehicle?.Type.ToString() ?? resource.VehicleType,
            driver?.Id,
            vehicle?.Id));
        return Ok(DeliveryResourceFromEntityAssembler.ToResourceFromEntity(delivery));
    }

    /// <summary>Registra un punto de control con nombre (solo en tránsito o con retraso).</summary>
    [HttpPost("{id:int}/checkpoints")]
    [ProducesResponseType(typeof(DeliveryEventResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddCheckpoint(int id, [FromBody] AddDeliveryCheckpointResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient)
            return StatusCode(403, new { message = "Only a distributor, the order's producer or an admin can update a delivery." });
        if (!await CanSeeDeliveryAsync(me, id)) return NotFound();

        var checkpoint = await _deliveryCommandService.Handle(new AddDeliveryCheckpointCommand(
            id, me.Id, resource.Name, resource.Latitude, resource.Longitude, resource.Note, resource.OccurredAt));
        return StatusCode(StatusCodes.Status201Created,
            DeliveryEventResourceFromEntityAssembler.ToResourceFromEntity(checkpoint));
    }

    /// <summary>Llegadas de las entregas visibles (geocerca o manuales), más recientes primero.</summary>
    [HttpGet("arrivals")]
    [ProducesResponseType(typeof(IEnumerable<DeliveryArrivalResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetArrivals([FromQuery] DateTimeOffset? since)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();

        var visible = await _accessScope.GetVisibleDeliveryIdsAsync(me);
        var arrivals = (await _deliveryQueryService.Handle(new GetDeliveryArrivalsQuery(since)))
            .WhereVisible(visible, e => e.DeliveryId)
            .ToList();
        if (arrivals.Count == 0) return Ok(Array.Empty<DeliveryArrivalResource>());

        var orderByDelivery = (await _deliveryQueryService.Handle(new GetAllDeliveriesQuery()))
            .ToDictionary(d => d.Id, d => d.OrderId);
        return Ok(arrivals.Select(e => new DeliveryArrivalResource(
            e.DeliveryId,
            orderByDelivery.GetValueOrDefault(e.DeliveryId),
            e.OccurredAt,
            e.Latitude,
            e.Longitude,
            e.CreatedByUserId is null ? "Geofence" : "Manual",
            e.Note)));
    }

    /// <summary>Busca chofer y vehículo de la flota si vienen sus ids; 400 si alguno no existe.</summary>
    private async Task<(FleetDriver? Driver, FleetVehicle? Vehicle, IActionResult? Error)> ResolveFleetAsync(
        int? driverId, int? vehicleId)
    {
        FleetDriver? driver = null;
        FleetVehicle? vehicle = null;
        if (driverId is { } dId)
        {
            driver = await _driverQueryService.Handle(new GetDriverByIdQuery(dId));
            if (driver is null) return (null, null, BadRequest(new { message = $"Driver {dId} not found." }));
        }
        if (vehicleId is { } vId)
        {
            vehicle = await _vehicleQueryService.Handle(new GetVehicleByIdQuery(vId));
            if (vehicle is null) return (null, null, BadRequest(new { message = $"Vehicle {vId} not found." }));
        }
        return (driver, vehicle, null);
    }

    private async Task<bool> CanSeeDeliveryAsync(CurrentUser me, int deliveryId)
    {
        var visible = await _accessScope.GetVisibleDeliveryIdsAsync(me);
        return visible is null || visible.Contains(deliveryId);
    }
}
