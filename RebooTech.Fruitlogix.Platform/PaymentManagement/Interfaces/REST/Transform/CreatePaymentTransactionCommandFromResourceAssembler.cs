using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST.Resources;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST.Transform;

public static class CreatePaymentTransactionCommandFromResourceAssembler
{
    public static CreatePaymentTransactionCommand ToCommandFromResource(
        CreatePaymentTransactionResource resource) =>
        new(
            resource.InvoiceId,
            resource.Amount,
            resource.Currency,
            resource.Method,
            resource.Gateway,
            resource.GatewayRef,
            resource.CardEnding,
            resource.CardBrand
        );
}