namespace RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.ValueObjects;

/// <summary>Disponibilidad del chofer.</summary>
public enum DriverStatus
{
    Available,
    OnRoute,
    OffDuty
}

/// <summary>Disponibilidad del vehículo.</summary>
public enum VehicleStatus
{
    Available,
    OnRoute,
    Maintenance
}

/// <summary>Tipo de unidad refrigerada.</summary>
public enum VehicleType
{
    HeavyReefer,
    ReeferTruck,
    MediumReeferVan,
    LightReefer
}

/// <summary>
///     Categorías de licencia profesional (Perú). No es un enum porque los valores llevan guiones;
///     se guardan como texto con esta forma exacta.
/// </summary>
public static class DriverLicenseClasses
{
    public static readonly IReadOnlyList<string> All = ["A-IIb", "A-IIIa", "A-IIIb", "A-IIIc"];

    /// <summary>Devuelve el valor canónico (sin distinguir mayúsculas) o lanza ArgumentException.</summary>
    public static string Normalize(string? value)
    {
        var match = All.FirstOrDefault(c => string.Equals(c, value?.Trim(), StringComparison.OrdinalIgnoreCase));
        return match ?? throw new ArgumentException(
            $"licenseClass must be one of: {string.Join(", ", All)}.", nameof(value));
    }
}

internal static class FleetEnumParser
{
    /// <summary>Parsea un enum por nombre (sin distinguir mayúsculas); los números no se aceptan.</summary>
    public static TEnum Parse<TEnum>(string? value, string field) where TEnum : struct, Enum
    {
        if (!string.IsNullOrWhiteSpace(value)
            && !int.TryParse(value, out _)
            && Enum.TryParse<TEnum>(value.Trim(), ignoreCase: true, out var parsed)
            && Enum.IsDefined(parsed))
            return parsed;
        throw new ArgumentException($"{field} must be one of: {string.Join(", ", Enum.GetNames<TEnum>())}.", field);
    }
}
