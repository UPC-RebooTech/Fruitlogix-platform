using RebooTech.Fruitlogix.Platform.Logistics.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;

namespace RebooTech.Fruitlogix.Platform.Logistics.Application.Internal.QueryServices;

public class DeliveryQueryService(
    IDeliveryRepository deliveryRepository,
    IDeliveryEventRepository deliveryEventRepository) : IDeliveryQueryService
{
    public async Task<IEnumerable<Delivery>> Handle(GetAllDeliveriesQuery query) =>
        await deliveryRepository.ListAsync();

    public async Task<Delivery?> Handle(GetDeliveryByIdQuery query) =>
        await deliveryRepository.FindByIdAsync(query.DeliveryId);

    public async Task<IEnumerable<Delivery>> Handle(GetDeliveriesByOrderIdQuery query) =>
        await deliveryRepository.FindByOrderIdAsync(query.OrderId);

    public async Task<IEnumerable<DeliveryEvent>> Handle(GetDeliveryEventsQuery query) =>
        await deliveryEventRepository.FindByDeliveryIdAsync(query.DeliveryId);

    public async Task<IEnumerable<DeliveryEvent>> Handle(GetDeliveryArrivalsQuery query) =>
        await deliveryEventRepository.FindByTypeAsync(DeliveryEventType.Arrived, query.Since);
}
