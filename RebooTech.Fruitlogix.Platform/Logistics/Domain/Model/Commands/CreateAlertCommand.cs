using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;

namespace RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;

public record CreateAlertCommand(
    int          DeliveryId,
    AlertType    Type,
    AlertSeverity Severity,
    string       Message
);