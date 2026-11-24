using RebooTech.Fruitlogix.Platform.Iam.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;
namespace RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest.Resources;

/// <param name="Role">Opcional: CommercialClient (por defecto), Producer o Distributor.</param>
public record SignUpResource(string Username, string Password,
    [property: SwaggerEnumValues(typeof(UserRole), nameof(UserRole.Admin))] string? Role = null);