using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.OrderManagement.Application.CommandServices;

public interface IOrderCommandService
{
    Task<Order?> Handle(CreateOrderCommand command);
    Task<Order?> Handle(UpdateOrderCommand command);
    Task<Order?> Handle(AssignProducerCommand command);
    Task<Order?> Handle(CancelOrderCommand command);
    Task<Order?> Handle(ConfirmOrderReceptionCommand command);
    Task Handle(DeleteOrderCommand command);
}