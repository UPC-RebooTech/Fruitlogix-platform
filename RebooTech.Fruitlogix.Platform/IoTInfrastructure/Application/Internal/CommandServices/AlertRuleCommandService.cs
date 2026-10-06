using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.Internal.CommandServices;

public class AlertRuleCommandService(
    IAlertRuleRepository repository,
    IUnitOfWork unitOfWork) : IAlertRuleCommandService
{
    public async Task<AlertRule> Handle(CreateAlertRuleCommand command)
    {
        var rule = new AlertRule(command);
        await repository.AddAsync(rule);
        await unitOfWork.CompleteAsync();
        return rule;
    }
}