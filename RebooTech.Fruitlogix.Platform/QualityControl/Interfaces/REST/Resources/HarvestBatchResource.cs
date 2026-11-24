using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;
namespace RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Resources;

public record HarvestBatchResource(
    int Id,
    int ProducerId,
    string FruitType,
    double QuantityKg,
    DateOnly HarvestDate,
    [property: SwaggerEnumValues(typeof(BatchStatus))]
    string Status
);