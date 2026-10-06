using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Resources;

namespace RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Transform;

public static class AssignProducerCommandFromResourceAssembler
{
    public static AssignProducerCommand ToCommandFromResource(int orderId, AssignProducerResource resource) =>
        new(orderId, resource.ProducerId!.Value);
}