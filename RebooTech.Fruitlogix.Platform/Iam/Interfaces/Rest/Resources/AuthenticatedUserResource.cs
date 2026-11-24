using RebooTech.Fruitlogix.Platform.Iam.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;
namespace RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest.Resources;

public record AuthenticatedUserResource(int Id, string Username, string Token,
    [property: SwaggerEnumValues(typeof(UserRole))] string Role);