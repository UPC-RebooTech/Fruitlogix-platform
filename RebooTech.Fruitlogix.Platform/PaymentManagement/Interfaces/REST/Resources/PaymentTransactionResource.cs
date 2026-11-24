using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;
namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST.Resources;

public record PaymentTransactionResource(
    int Id,
    int InvoiceId,
    decimal Amount,
    string Currency,
    string Method,
    string Gateway,
    string GatewayRef,
    [property: SwaggerEnumValues(typeof(TransactionStatus))]
    string Status,
    string? CardEnding,
    string? CardBrand,
    DateTimeOffset ProcessedAt
);