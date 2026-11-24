using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;
namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST.Resources;

public record InvoiceResource(
    int Id,
    int ClientId,
    int? OrderId,
    decimal TotalAmount,
    string Currency,
    DateOnly DueDate,
    [property: SwaggerEnumValues(typeof(InvoiceStatus))]
    string Status,
    string InvoiceType,
    DateTimeOffset? IssuedAt,
    DateTimeOffset? PaidAt,
    int? IssuedByUserId
);