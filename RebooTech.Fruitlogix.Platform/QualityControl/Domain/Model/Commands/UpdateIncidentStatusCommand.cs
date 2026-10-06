using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.ValueObjects;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;

public record UpdateIncidentStatusCommand(int IncidentId, IncidentStatus NewStatus);