using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.CommandServices;

public interface IIoTDeviceCommandService
{
    Task<IoTDevice?> Handle(UpdateIoTDeviceCommand command);
}