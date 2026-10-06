using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.Internal.CommandServices;

public class IoTDeviceCommandService(
    IIoTDeviceRepository repository,
    IUnitOfWork unitOfWork) : IIoTDeviceCommandService
{
    public async Task<IoTDevice?> Handle(UpdateIoTDeviceCommand command)
    {
        var device = await repository.FindByIdAsync(command.Id);
        if (device is null) return null;

        switch (command.TargetStatus.ToUpperInvariant())
        {
            case "MAINTENANCE":
                device.Calibrate();
                break;
            case "ACTIVE":
                device.Activate();
                break;
            default:
                throw new ArgumentException($"Status '{command.TargetStatus}' is not a valid target status.");
        }

        repository.Update(device);
        await unitOfWork.CompleteAsync();
        return device;
    }
}