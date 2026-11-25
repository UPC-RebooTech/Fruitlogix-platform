namespace RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;

/// <summary>Cosechas programadas, ordenadas por scheduledFor. ProducerId null = todas.</summary>
public record GetHarvestPlansQuery(int? ProducerId);

public record GetHarvestPlanByIdQuery(int HarvestPlanId);
