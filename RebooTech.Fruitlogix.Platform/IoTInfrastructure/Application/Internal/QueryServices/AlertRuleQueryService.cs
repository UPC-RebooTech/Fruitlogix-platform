using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.Internal.QueryServices;

public class AlertRuleQueryService(IAlertRuleRepository repository)
    : IAlertRuleQueryService
{
    public async Task<IEnumerable<AlertRule>> Handle(GetAllAlertRulesQuery query)
        => await repository.ListAsync();
}