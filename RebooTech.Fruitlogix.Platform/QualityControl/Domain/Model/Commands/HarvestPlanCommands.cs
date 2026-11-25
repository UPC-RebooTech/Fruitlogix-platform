namespace RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;

/// <summary>Datos de una cosecha programada (crear o reemplazar).</summary>
public record SaveHarvestPlanCommand(
    int ProducerId,
    string FruitType,
    DateTimeOffset ScheduledFor,
    double EstimatedYieldKg,
    string Zone,
    string CrewName,
    int Pickers,
    string? Notes
);

public record UpdateHarvestPlanCommand(int HarvestPlanId, SaveHarvestPlanCommand Data);

public record DeleteHarvestPlanCommand(int HarvestPlanId);
