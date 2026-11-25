using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Model;

namespace RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Aggregates;

/// <summary>Chofer de la flota de la distribuidora.</summary>
public class Driver : IAuditableEntity
{
    public const int MaxNameLength = 100;
    public const int MaxPhoneLength = 30;
    public const int MaxLicenseClassLength = 10;

    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Phone { get; private set; }
    /// <summary>A-IIb, A-IIIa, A-IIIb o A-IIIc.</summary>
    public string LicenseClass { get; private set; } = null!;
    public DateOnly LicenseExpiry { get; private set; }
    public bool ColdChainCertified { get; private set; }
    public DriverStatus Status { get; private set; }
    /// <summary>Descanso obligatorio hasta esta fecha y hora (opcional).</summary>
    public DateTimeOffset? RestUntil { get; private set; }
    public int? CreatedByUserId { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Driver() { }

    public Driver(CreateDriverCommand command)
    {
        Apply(command.Name, command.Phone, command.LicenseClass, command.LicenseExpiry,
            command.ColdChainCertified, command.RestUntil);
        Status = command.Status is null
            ? DriverStatus.Available
            : FleetEnumParser.Parse<DriverStatus>(command.Status, "status");
        CreatedByUserId = command.CreatedByUserId;
    }

    public void Update(UpdateDriverCommand command)
    {
        Apply(command.Name, command.Phone, command.LicenseClass, command.LicenseExpiry,
            command.ColdChainCertified, command.RestUntil);
        if (command.Status is not null)
            Status = FleetEnumParser.Parse<DriverStatus>(command.Status, "status");
    }

    private void Apply(string name, string? phone, string licenseClass, DateOnly licenseExpiry,
        bool coldChainCertified, DateTimeOffset? restUntil)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("name is required.", nameof(name));
        if (name.Trim().Length > MaxNameLength)
            throw new ArgumentException($"name cannot exceed {MaxNameLength} characters.", nameof(name));
        var trimmedPhone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        if (trimmedPhone is { Length: > MaxPhoneLength })
            throw new ArgumentException($"phone cannot exceed {MaxPhoneLength} characters.", nameof(phone));
        if (licenseExpiry == default)
            throw new ArgumentException("licenseExpiry is required (yyyy-MM-dd).", nameof(licenseExpiry));

        Name = name.Trim();
        Phone = trimmedPhone;
        LicenseClass = DriverLicenseClasses.Normalize(licenseClass);
        LicenseExpiry = licenseExpiry;
        ColdChainCertified = coldChainCertified;
        RestUntil = restUntil;
    }
}
