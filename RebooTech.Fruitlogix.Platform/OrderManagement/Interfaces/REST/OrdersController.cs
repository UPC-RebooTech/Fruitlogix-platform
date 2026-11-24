using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.OrderManagement.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.OrderManagement.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Transform;

namespace RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
public class OrdersController(
    IOrderCommandService orderCommandService,
    IOrderQueryService orderQueryService,
    IAccessScopeService accessScope,
    IOrderDependencyService orderDependencies) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateOrderResource resource)
    {
        var command = CreateOrderCommandFromResourceAssembler.ToCommandFromResource(resource);
        var order = await orderCommandService.Handle(command);
        if (order is null) return BadRequest();
        var orderResource = OrderResourceFromEntityAssembler.ToResourceFromEntity(order);
        return StatusCode(201, orderResource);
    }

    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAll()
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var orders = await orderQueryService.Handle(new GetAllOrdersQuery());
        var visible = await accessScope.GetVisibleOrderIdsAsync(me);
        var resources = orders.WhereVisible(visible, o => o.Id).Select(OrderResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var order = await orderQueryService.Handle(new GetOrderByIdQuery(id));
        if (order is null) return NotFound();
        var visible = await accessScope.GetVisibleOrderIdsAsync(me);
        if (visible is not null && !visible.Contains(order.Id)) return NotFound();
        return Ok(OrderResourceFromEntityAssembler.ToResourceFromEntity(order));
    }

    [HttpGet("client/{clientId:int}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetByClientId(int clientId)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var orders = await orderQueryService.Handle(new GetOrdersByClientIdQuery(clientId));
        var visible = await accessScope.GetVisibleOrderIdsAsync(me);
        var resources = orders.WhereVisible(visible, o => o.Id).Select(OrderResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!await CanAccessOrderAsync(me, id)) return NotFound();

        var command = UpdateOrderCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var order = await orderCommandService.Handle(command);
        if (order is null) return NotFound();
        return Ok(OrderResourceFromEntityAssembler.ToResourceFromEntity(order));
    }

    [HttpPatch("{id:int}/assign-producer")]
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
        return Ok(OrderResourceFromEntityAssembler.ToResourceFromEntity(order));
    }

    [HttpPatch("{id:int}/cancel")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelOrderResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!await CanAccessOrderAsync(me, id)) return NotFound();

        var command = new Domain.Model.Commands.CancelOrderCommand(id, resource.Reason);
        var order = await orderCommandService.Handle(command);
        if (order is null) return NotFound();
        return Ok(OrderResourceFromEntityAssembler.ToResourceFromEntity(order));
    }
    
    [HttpPost("{orderId:int}/reception")]
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

        // Aquí sí usamos el Assembler viejo porque vamos de Entity a Resource para mostrar el resultado
        return Ok(OrderResourceFromEntityAssembler.ToResourceFromEntity(order));
    }
    
    [HttpGet("producer/{producerId:int}")]
    public async Task<IActionResult> GetByProducerId(int producerId)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var orders = await orderQueryService.Handle(
            new GetOrdersByProducerIdQuery(producerId));
        var visible = await accessScope.GetVisibleOrderIdsAsync(me);
        var resources = orders.WhereVisible(visible, o => o.Id).Select(
            OrderResourceFromEntityAssembler.ToResourceFromEntity);
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
}
