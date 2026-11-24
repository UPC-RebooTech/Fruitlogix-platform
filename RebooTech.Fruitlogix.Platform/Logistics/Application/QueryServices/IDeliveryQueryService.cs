using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.Logistics.Application.QueryServices;

public interface IDeliveryQueryService
{
    Task<IEnumerable<Delivery>> Handle(GetAllDeliveriesQuery query);
    Task<Delivery?> Handle(GetDeliveryByIdQuery query);
    Task<IEnumerable<Delivery>> Handle(GetDeliveriesByOrderIdQuery query);
    Task<IEnumerable<DeliveryEvent>> Handle(GetDeliveryEventsQuery query);
}