using System.Text.RegularExpressions;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Model;

namespace RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Aggregates;

/// <summary>Unidad refrigerada de la flota.</summary>
public partial class Vehicle : IAuditableEntity
{
    public const int MaxUnitCodeLength = 30;
    public const int PlateLength = 7;
    public const double MinCapacityKg = 500;
    public const double MaxCapacityKg = 40000;
    public const double MinAllowedCelsius = -30;
    public const double MaxAllowedCelsius = 30;

    public int Id { get; private set; }
    /// <summary>Código interno de la unidad (por ejemplo, "U-07").</summary>
    public string UnitCode { get; private set; } = null!;
    /// <summary>Placa en formato ABC-123, en mayúsculas y única.</summary>
    public string Plate { get; private set; } = null!;
    public VehicleType Type { get; private set; }
    public double CapacityKg { get; private set; }
    public double MinCelsius { get; private set; }
    public double MaxCelsius { get; private set; }
    public VehicleStatus Status { get; private set; }
    public int? CreatedByUserId { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Vehicle() { }

    public Vehicle(CreateVehicleCommand command)
    {
        Apply(command.UnitCode, command.Plate, command.Type, command.CapacityKg, command.MinCelsius, command.MaxCelsius);
        Status = command.Status is null
            ? VehicleStatus.Available
            : FleetEnumParser.Parse<VehicleStatus>(command.Status, "status");
        CreatedByUserId = command.CreatedByUserId;
    }

    public void Update(UpdateVehicleCommand command)
    {
        Apply(command.UnitCode, command.Plate, command.Type, command.CapacityKg, command.MinCelsius, command.MaxCelsius);
        if (command.Status is not null)
            Status = FleetEnumParser.Parse<VehicleStatus>(command.Status, "status");
    }

    /// <summary>Normaliza la placa (trim y mayúsculas) y valida el formato ABC-123.</summary>
    public static string NormalizePlate(string? plate)
    {
        var normalized = plate?.Trim().ToUpperInvariant() ?? string.Empty;
        if (!PlateRegex().IsMatch(normalized))
            throw new ArgumentException("plate must have the format ABC-123.", nameof(plate));
        return normalized;
    }

    private void Apply(string unitCode, string plate, string type, double capacityKg, double minCelsius, double maxCelsius)
    {
        if (string.IsNullOrWhiteSpace(unitCode))
            throw new ArgumentException("unitCode is required.", nameof(unitCode));
        if (unitCode.Trim().Length > MaxUnitCodeLength)
            throw new ArgumentException($"unitCode cannot exceed {MaxUnitCodeLength} characters.", nameof(unitCode));
        if (capacityKg is < MinCapacityKg or > MaxCapacityKg)
            throw new ArgumentException($"capacityKg must be between {MinCapacityKg} and {MaxCapacityKg}.", nameof(capacityKg));
        if (minCelsius is < MinAllowedCelsius or > MaxAllowedCelsius)
            throw new ArgumentException($"minCelsius must be between {MinAllowedCelsius} and {MaxAllowedCelsius}.", nameof(minCelsius));
        if (maxCelsius is < MinAllowedCelsius or > MaxAllowedCelsius)
            throw new ArgumentException($"maxCelsius must be between {MinAllowedCelsius} and {MaxAllowedCelsius}.", nameof(maxCelsius));
        if (minCelsius > maxCelsius)
            throw new ArgumentException("minCelsius cannot be greater than maxCelsius.", nameof(minCelsius));

        UnitCode = unitCode.Trim();
        Plate = NormalizePlate(plate);
        Type = FleetEnumParser.Parse<VehicleType>(type, "type");
        CapacityKg = capacityKg;
        MinCelsius = minCelsius;
        MaxCelsius = maxCelsius;
    }

    [GeneratedRegex("^[A-Z]{3}-[0-9]{3}$")]
    private static partial Regex PlateRegex();
}
