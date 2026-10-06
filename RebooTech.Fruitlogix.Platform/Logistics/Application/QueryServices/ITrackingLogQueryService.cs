using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.Logistics.Application.QueryServices;

public interface ITrackingLogQueryService
{
    Task<IEnumerable<TrackingLog>> Handle(GetTrackingLogsByDeliveryIdQuery query);
}