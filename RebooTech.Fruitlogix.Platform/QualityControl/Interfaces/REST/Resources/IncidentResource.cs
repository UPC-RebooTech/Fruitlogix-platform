using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;
namespace RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Resources;

public record IncidentResource(
    int Id,
    int BatchId,
    string Description,
    List<string> EvidenceUrls,
    [property: SwaggerEnumValues(typeof(IncidentStatus))]
    string Status,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? ResolvedAt
);