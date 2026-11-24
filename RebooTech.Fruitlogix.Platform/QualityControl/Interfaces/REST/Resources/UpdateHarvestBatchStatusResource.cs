using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;
namespace RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Resources;

public record UpdateHarvestBatchStatusResource(
    [property: SwaggerEnumValues(typeof(BatchStatus))] string Status);