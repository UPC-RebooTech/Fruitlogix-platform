using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.Logistics.Application.QueryServices;

public interface IDeliveryQueryService
{
    Task<IEnumerable<Delivery>> Handle(GetAllDeliveriesQuery query);
}