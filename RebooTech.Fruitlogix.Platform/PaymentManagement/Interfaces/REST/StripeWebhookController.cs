using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Commands;
using Stripe;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST;

/// <summary>
///     TS17 — Recibe los eventos de Stripe. Es anónimo (Stripe no envía JWT), por eso
///     la autenticidad se valida con la firma <c>Stripe-Signature</c> y el secreto
///     <c>Stripe:WebhookSecret</c> (whsec_...).
/// </summary>
[ApiController]
[Route("api/v1/payments/webhook")]
public class StripeWebhookController(
    IMembershipCommandService membershipCommandService,
    IConfiguration configuration,
    ILogger<StripeWebhookController> logger) : ControllerBase
{
    public const string SubscriptionPaymentType = "SaaS_Subscription";

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Receive()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();
        var secret = configuration["Stripe:WebhookSecret"];

        if (string.IsNullOrWhiteSpace(secret))
        {
            logger.LogError("Stripe webhook received but Stripe:WebhookSecret is not configured.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json, signature ?? string.Empty, secret, throwOnApiVersionMismatch: false);
        }
        catch (StripeException e)
        {
            // Escenario 2: firma inválida -> se descarta y se registra como intento no verificado.
            logger.LogWarning(
                "Unverified Stripe webhook attempt discarded. Remote IP: {RemoteIp}. Reason: {Reason}",
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                e.Message);
            return BadRequest(new { message = "Invalid Stripe signature." });
        }

        switch (stripeEvent.Type)
        {
            case EventTypes.PaymentIntentSucceeded:
                await HandlePaymentSucceeded(stripeEvent);
                break;
            case EventTypes.PaymentIntentPaymentFailed:
                if (stripeEvent.Data.Object is PaymentIntent failed)
                    logger.LogInformation("Stripe payment failed: {PaymentIntentId}", failed.Id);
                break;
        }

        // 200 para que Stripe no reintente eventos que ya procesamos (o que ignoramos).
        return Ok();
    }

    private async Task HandlePaymentSucceeded(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not PaymentIntent intent) return;

        var metadata = intent.Metadata ?? new Dictionary<string, string>();
        if (!metadata.TryGetValue("PaymentType", out var paymentType) ||
            paymentType != SubscriptionPaymentType)
            return; // Pagos de pedidos únicos: no cambian el plan.

        if (!metadata.TryGetValue("UserId", out var rawUserId) ||
            !int.TryParse(rawUserId, out var userId) || userId <= 0)
        {
            logger.LogWarning("Subscription payment {PaymentIntentId} has no valid UserId metadata.", intent.Id);
            return;
        }

        // Escenario 1: payment_intent.succeeded -> plan "Miembro Pro".
        await membershipCommandService.Handle(new ActivateProMembershipCommand(userId, intent.Id));
        logger.LogInformation("User {UserId} upgraded to PRO by {PaymentIntentId}.", userId, intent.Id);
    }
}
