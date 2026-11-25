using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Model;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;

/// <summary>Cosecha programada de un productor (antes de que exista el lote).</summary>
public class HarvestPlan : IAuditableEntity
{
    public const int MaxFruitTypeLength = 100;
    public const int MaxZoneLength = 100;
    public const int MaxCrewNameLength = 100;
    public const int MaxNotesLength = 500;

    public int Id { get; private set; }
    /// <summary>Id del perfil en /producers.</summary>
    public int ProducerId { get; private set; }
    public string FruitType { get; private set; } = null!;
    public DateTimeOffset ScheduledFor { get; private set; }
    public double EstimatedYieldKg { get; private set; }
    public string Zone { get; private set; } = null!;
    public string CrewName { get; private set; } = null!;
    public int Pickers { get; private set; }
    public string? Notes { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected HarvestPlan() { }

    public HarvestPlan(SaveHarvestPlanCommand command)
    {
        Apply(command);
    }

    public void Update(SaveHarvestPlanCommand command) => Apply(command);

    private void Apply(SaveHarvestPlanCommand c)
    {
        if (c.ProducerId <= 0)
            throw new ArgumentException("producerId must be a positive integer.", nameof(c.ProducerId));
        if (c.ScheduledFor == default)
            throw new ArgumentException("scheduledFor is required (ISO 8601 date and time).", nameof(c.ScheduledFor));
        if (c.EstimatedYieldKg <= 0)
            throw new ArgumentException("estimatedYieldKg must be greater than 0.", nameof(c.EstimatedYieldKg));
        if (c.Pickers < 1)
            throw new ArgumentException("pickers must be at least 1.", nameof(c.Pickers));

        ProducerId = c.ProducerId;
        FruitType = Required(c.FruitType, "fruitType", MaxFruitTypeLength);
        ScheduledFor = c.ScheduledFor;
        EstimatedYieldKg = c.EstimatedYieldKg;
        Zone = Required(c.Zone, "zone", MaxZoneLength);
        CrewName = Required(c.CrewName, "crewName", MaxCrewNameLength);
        Pickers = c.Pickers;
        Notes = Optional(c.Notes, "notes", MaxNotesLength);
    }

    private static string Required(string? value, string field, int maxLength) =>
        Optional(value, field, maxLength) ?? throw new ArgumentException($"{field} is required.", field);

    private static string? Optional(string? value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new ArgumentException($"{field} cannot exceed {maxLength} characters.", field);
        return trimmed;
    }
}
