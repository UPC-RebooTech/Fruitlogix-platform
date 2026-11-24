using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Traceability;
using RebooTech.Fruitlogix.Platform.OrderManagement.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.OrderManagement.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Transform;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[DomainExceptionFilter]
public class OrdersController(
    IOrderCommandService orderCommandService,
    IOrderQueryService orderQueryService,
    IAccessScopeService accessScope,
    IOrderDependencyService orderDependencies,
    IDeliveryCommandService deliveryCommandService,
    IProducerQueryService producerQueryService,
    IHarvestBatchQueryService harvestBatchQueryService,
    IProductQueryService productQueryService,
    IOrderTraceabilityService traceabilityService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(400)]
    [ProducesResponseType(typeof(OrderResource), 201)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateOrderResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var command = CreateOrderCommandFromResourceAssembler.ToCommandFromResource(resource);
        // Un cliente solo crea pedidos a su nombre: se ignora el commercialClientId del body.
        if (me.IsCommercialClient) command = command with { CommercialClientId = me.Id };

        var (catalogCommand, catalogError) = await ApplyCatalogAsync(command);
        if (catalogError is not null) return catalogError;
        command = catalogCommand!;
        var order = await orderCommandService.Handle(command);
        if (order is null) return BadRequest();
        var orderResource = await ToResourceAsync(order);
        return StatusCode(201, orderResource);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResource>), 200)]
    public async Task<IActionResult> GetAll()
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var orders = await orderQueryService.Handle(new GetAllOrdersQuery());
        var visible = await accessScope.GetVisibleOrderIdsAsync(me);
        var resources = await ToResourcesAsync(orders.WhereVisible(visible, o => o.Id));
        return Ok(resources);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(OrderResource), 200)]
    public async Task<IActionResult> GetById(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var order = await orderQueryService.Handle(new GetOrderByIdQuery(id));
        if (order is null) return NotFound();
        var visible = await accessScope.GetVisibleOrderIdsAsync(me);
        if (visible is not null && !visible.Contains(order.Id)) return NotFound();
        return Ok(await ToResourceAsync(order));
    }

    [HttpGet("client/{clientId:int}")]
    [ProducesResponseType(typeof(IEnumerable<OrderResource>), 200)]
    public async Task<IActionResult> GetByClientId(int clientId)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var orders = await orderQueryService.Handle(new GetOrdersByClientIdQuery(clientId));
        var visible = await accessScope.GetVisibleOrderIdsAsync(me);
        var resources = await ToResourcesAsync(orders.WhereVisible(visible, o => o.Id));
        return Ok(resources);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(OrderResource), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!await CanAccessOrderAsync(me, id)) return NotFound();

        var command = UpdateOrderCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var order = await orderCommandService.Handle(command);
        if (order is null) return NotFound();
        return Ok(await ToResourceAsync(order));
    }

    [HttpPatch("{id:int}/assign-producer")]
    [ProducesResponseType(typeof(OrderResource), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> AssignProducer(int id, [FromBody] AssignProducerResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!await CanAccessOrderAsync(me, id)) return NotFound();

        if (resource?.ProducerId == null)
            return BadRequest(new { message = "ProducerId is required." });
    
        var command = AssignProducerCommandFromResourceAssembler
            .ToCommandFromResource(id, resource);
        var order = await orderCommandService.Handle(command);
        if (order is null) return NotFound();
        return Ok(await ToResourceAsync(order));
    }

    /// <summary>Vincula el lote de cosecha con el pedido (productor del pedido, Distributor o Admin).</summary>
    [HttpPatch("{id:int}/batch")]
    [ProducesResponseType(typeof(OrderResource), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> AssignBatch(int id, [FromBody] AssignOrderBatchResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!await CanAccessOrderAsync(me, id)) return NotFound();
        if (me.IsCommercialClient)
            return StatusCode(403, new { message = "Only the order's producer, a distributor or an admin can link a batch." });
        if (resource?.BatchId is not > 0)
            return BadRequest(new { message = "BatchId is required." });

        var order = await orderQueryService.Handle(new GetOrderByIdQuery(id));
        if (order is null) return NotFound();

        var batch = await harvestBatchQueryService.Handle(new GetHarvestBatchByIdQuery(resource.BatchId.Value));
        if (batch is null)
            return NotFound(new { message = $"Harvest batch {resource.BatchId} not found." });
        if (order.ProducerId is null || order.ProducerId.Value != batch.ProducerId)
            return BadRequest(new { message = "The batch must belong to the producer assigned to the order." });
        if (batch.Status == BatchStatus.Rejected)
            return Conflict(new { message = "A rejected batch cannot be linked to an order." });

        var updated = await orderCommandService.Handle(new AssignOrderBatchCommand(id, batch.Id));
        if (updated is null) return NotFound();
        return Ok(await ToResourceAsync(updated));
    }

    [HttpPatch("{id:int}/cancel")]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(OrderResource), 200)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelOrderResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!await CanAccessOrderAsync(me, id)) return NotFound();

        var command = new Domain.Model.Commands.CancelOrderCommand(id, resource.Reason);
        var order = await orderCommandService.Handle(command);
        if (order is null) return NotFound();
        return Ok(await ToResourceAsync(order));
    }
    
    [HttpPost("{orderId:int}/reception")]
    [ProducesResponseType(typeof(OrderResource), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> ConfirmOrderReception(
        int orderId, [FromBody] ConfirmOrderReceptionResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!await CanAccessOrderAsync(me, orderId)) return NotFound($"Order with id {orderId} not found.");

        // Usamos el nuevo Assembler que acabas de crear
        var command = ConfirmOrderReceptionCommandFromResourceAssembler.ToCommandFromResource(orderId, resource);
    
        var order = await orderCommandService.Handle(command);

        if (order is null)
            return NotFound($"Order with id {orderId} not found.");

        // La recepción cierra también la entrega del pedido.
        await deliveryCommandService.Handle(new MarkDeliveriesDeliveredByOrderCommand(orderId, me.Id));

        // Aquí sí usamos el Assembler viejo porque vamos de Entity a Resource para mostrar el resultado
        return Ok(await ToResourceAsync(order));
    }
    
    /// <summary>
    ///     Trazabilidad del pedido (US72): cosecha, inspección, despacho, recepción y factura,
    ///     cada etapa con fecha, responsable y estado (Completed, Pending o NotApplicable).
    /// </summary>
    [HttpGet("{orderId:int}/traceability")]
    [ProducesResponseType(typeof(OrderTraceabilityResource), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrderTraceability(int orderId)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!await CanAccessOrderAsync(me, orderId)) return NotFound($"Order with id {orderId} not found.");

        var traceability = await traceabilityService.GetAsync(orderId);
        if (traceability is null) return NotFound($"Order with id {orderId} not found.");
        return Ok(traceability);
    }

    /// <summary>Recepción confirmada del pedido (calificación, comentario, sello y temperatura).</summary>
    [HttpGet("{orderId:int}/reception")]
    [ProducesResponseType(typeof(OrderReceptionResource), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrderReception(int orderId)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!await CanAccessOrderAsync(me, orderId)) return NotFound($"Order with id {orderId} not found.");

        var order = await orderQueryService.Handle(new GetOrderByIdQuery(orderId));
        if (order is null) return NotFound($"Order with id {orderId} not found.");

        var reception = OrderResourceFromEntityAssembler.ToReceptionResourceFromEntity(order);
        if (reception is null) return NotFound($"Order with id {orderId} has no confirmed reception yet.");
        return Ok(reception);
    }

    [HttpGet("producer/{producerId:int}")]
    [ProducesResponseType(typeof(IEnumerable<OrderResource>), 200)]
    public async Task<IActionResult> GetByProducerId(int producerId)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var orders = await orderQueryService.Handle(
            new GetOrdersByProducerIdQuery(producerId));
        var visible = await accessScope.GetVisibleOrderIdsAsync(me);
        var resources = await ToResourcesAsync(orders.WhereVisible(visible, o => o.Id));
        return Ok(resources);
    }
    /// <summary>
    ///     Deletes an order permanently.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order to delete.</param>
    /// <returns>204 No Content on success.</returns>
    [HttpDelete("{orderId:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!await CanAccessOrderAsync(me, orderId)) return NotFound();

        // Un pedido con entrega, factura o pago no se borra: se cancela (PATCH /orders/{id}/cancel).
        var dependencies = await orderDependencies.GetDependenciesAsync(orderId);
        if (dependencies.Count > 0)
            return Conflict(new
            {
                message = $"Order {orderId} cannot be deleted because it has an associated {string.Join(", ", dependencies)}. "
                          + $"Use PATCH /api/v1/orders/{orderId}/cancel instead.",
                dependencies
            });

        try
        {
            var command = new DeleteOrderCommand(orderId);
            await orderCommandService.Handle(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    ///     Admin y Distributor acceden a todo; Producer y CommercialClient solo a sus pedidos.
    ///     Un pedido ajeno se trata como inexistente (404).
    /// </summary>
    private async Task<bool> CanAccessOrderAsync(CurrentUser me, int orderId)
    {
        var visible = await accessScope.GetVisibleOrderIdsAsync(me);
        return visible is null || visible.Contains(orderId);
    }

    /// <summary>
    ///     Transición al catálogo (opción B): los ítems cuyo productId existe en /products se validan
    ///     (activo, pedido mínimo, stock) y toman nombre y precio del catálogo; el pedido queda con el
    ///     productor de esos productos. Los productId desconocidos se aceptan como antes.
    ///     No descuenta stock.
    /// </summary>
    private async Task<(CreateOrderCommand? Command, IActionResult? Error)> ApplyCatalogAsync(CreateOrderCommand command)
    {
        var items = new List<CreateOrderItemCommand>();
        var producerIds = new HashSet<int>();

        foreach (var item in command.Items ?? [])
        {
            var product = item.ProductId > 0
                ? await productQueryService.Handle(new GetProductByIdQuery(item.ProductId))
                : null;
            if (product is null)
            {
                items.Add(item);
                continue;
            }

            if (!product.IsActive)
                return (null, BadRequest(new { message = $"Product {product.Id} ({product.Name}) is not available." }));
            if (item.QuantityKg < product.MinOrderKg)
                return (null, BadRequest(new { message = $"Minimum order for product {product.Id} ({product.Name}) is {product.MinOrderKg} kg." }));
            if (item.QuantityKg > product.AvailableKg)
                return (null, Conflict(new { message = $"Only {product.AvailableKg} kg of product {product.Id} ({product.Name}) are available." }));

            producerIds.Add(product.ProducerId);
            items.Add(item with { ProductName = product.Name, UnitPrice = product.UnitPrice });
        }

        if (producerIds.Count > 1)
            return (null, BadRequest(new { message = "All catalog products in an order must belong to the same producer." }));

        return (command with { Items = items, ProducerId = producerIds.Count == 1 ? producerIds.First() : null }, null);
    }

    private static string ProducerDisplayName(Profiles.Domain.Model.Aggregates.Producer producer) =>
        string.IsNullOrWhiteSpace(producer.FullName) ? producer.LegalName : producer.FullName;

    private async Task<OrderResource> ToResourceAsync(Order order)
    {
        string? producerName = null;
        if (order.ProducerId is not null)
        {
            var producer = await producerQueryService.Handle(new GetProducerByIdQuery(order.ProducerId.Value));
            if (producer is not null) producerName = ProducerDisplayName(producer);
        }
        return OrderResourceFromEntityAssembler.ToResourceFromEntity(order, producerName);
    }

    private async Task<List<OrderResource>> ToResourcesAsync(IEnumerable<Order> orders)
    {
        var list = orders.ToList();
        if (list.Count == 0) return [];
        var names = (await producerQueryService.Handle(new GetAllProducersQuery()))
            .ToDictionary(p => p.Id, ProducerDisplayName);
        return list.Select(o => OrderResourceFromEntityAssembler.ToResourceFromEntity(o,
            o.ProducerId is not null && names.TryGetValue(o.ProducerId.Value, out var name) ? name : null)).ToList();
    }
}
