using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;

public interface IAlertCommandService
{
    Task<Alert> Handle(CreateAlertCommand command);
    Task<Alert> Handle(ResolveAlertCommand command);
}