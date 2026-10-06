using RebooTech.Fruitlogix.Platform.OrderManagement.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.OrderManagement.Application.Internal.QueryServices;

public class OrderQueryService(IOrderRepository orderRepository) : IOrderQueryService
{
    public async Task<IEnumerable<Order>> Handle(GetAllOrdersQuery query) =>
        await orderRepository.GetAllOrdersWithItemsAsync();

    public async Task<Order?> Handle(GetOrderByIdQuery query) =>
        await orderRepository.GetOrderByIdWithItemsAsync(query.OrderId);

    public async Task<IEnumerable<Order>> Handle(GetOrdersByClientIdQuery query) =>
        await orderRepository.FindByClientIdAsync(query.CommercialClientId);
    
    public async Task<IEnumerable<Order>> Handle(GetOrdersByProducerIdQuery query) =>
        await orderRepository.FindByProducerIdAsync(query.ProducerId);
}