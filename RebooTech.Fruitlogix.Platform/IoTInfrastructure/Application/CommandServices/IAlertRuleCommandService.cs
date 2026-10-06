using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.CommandServices;

public interface IAlertRuleCommandService
{
    Task<AlertRule> Handle(CreateAlertRuleCommand command);
}