using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Interfaces.REST.Resources;

namespace RebooTech.Fruitlogix.Platform.IoTInfrastructure.Interfaces.REST.Transform;

public static class CreateAlertRuleCommandFromResourceAssembler
{
    public static CreateAlertRuleCommand ToCommandFromResource(CreateAlertRuleResource resource) =>
        new(
            resource.DeviceType,
            resource.MinThreshold,
            resource.MaxThreshold,
            resource.AlertMessage,
            resource.Severity,
            resource.IsActive
        );
}