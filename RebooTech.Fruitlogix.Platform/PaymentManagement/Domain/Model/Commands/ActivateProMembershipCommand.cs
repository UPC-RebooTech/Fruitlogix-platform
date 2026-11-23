namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Commands;

/// <summary>
///     Se emite cuando Stripe confirma (vía webhook firmado) el pago de la suscripción.
/// </summary>
/// <param name="UserId">Usuario IAM que pagó (viene en la metadata del PaymentIntent).</param>
/// <param name="GatewayRef">Id del PaymentIntent de Stripe (pi_...). Sirve para idempotencia.</param>
public record ActivateProMembershipCommand(int UserId, string GatewayRef);
