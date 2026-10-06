using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.Logistics.Application.QueryServices;

public interface IAlertQueryService
{
    Task<IEnumerable<Alert>> Handle(GetAllActiveAlertsQuery query);
}