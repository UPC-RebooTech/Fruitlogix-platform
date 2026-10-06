using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Interfaces.REST.Resources;

namespace RebooTech.Fruitlogix.Platform.IoTInfrastructure.Interfaces.REST.Transform;

public static class UpdateIoTDeviceCommandFromResourceAssembler
{
    public static UpdateIoTDeviceCommand ToCommandFromResource(int id, UpdateIoTDeviceResource resource) =>
        new(id, resource.TargetStatus);
}