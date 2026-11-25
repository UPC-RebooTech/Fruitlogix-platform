namespace RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Resources;

/// <summary>Cosecha programada.</summary>
public record HarvestPlanResource(
    int Id,
    int ProducerId,
    string? ProducerName,
    string FruitType,
    DateTimeOffset ScheduledFor,
    double EstimatedYieldKg,
    string Zone,
    string CrewName,
    int Pickers,
    string? Notes,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);

/// <summary>
///     POST /harvest-plans y PUT /harvest-plans/{id}. PUT reemplaza todos los campos.
///     producerId: un Producer no lo envía (se usa su perfil); Admin y Distributor deben enviarlo.
/// </summary>
/// <param name="ScheduledFor">Fecha y hora ISO 8601, por ejemplo 2026-12-02T06:00:00-05:00.</param>
/// <param name="EstimatedYieldKg">Mayor que 0.</param>
/// <param name="Pickers">Entero mayor o igual que 1.</param>
public record SaveHarvestPlanResource(
    string FruitType,
    DateTimeOffset ScheduledFor,
    double EstimatedYieldKg,
    string Zone,
    string CrewName,
    int Pickers,
    string? Notes = null,
    int? ProducerId = null
);
