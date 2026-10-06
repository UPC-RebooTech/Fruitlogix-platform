using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.QueryServices;

public interface IAlertRuleQueryService
{
    Task<IEnumerable<AlertRule>> Handle(GetAllAlertRulesQuery query);
}