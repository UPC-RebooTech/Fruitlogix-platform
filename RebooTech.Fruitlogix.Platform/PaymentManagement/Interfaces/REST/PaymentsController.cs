using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
public class PaymentsController : ControllerBase
{
    /// <summary>
    /// Endpoint para pagar facturas/órdenes únicas de fruta
    /// </summary>
    [HttpPost("create-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentResource resource)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                // ⚠️ CRUCIAL: Stripe recibe los montos en CENTAVOS (Ej: S/. 10.50 deben ser 1050)
                Amount = (long)(resource.Amount * 100), 
                Currency = resource.Currency.ToLower(),
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true, // Habilita tarjetas, Apple Pay, Google Pay automáticamente
                },
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", resource.OrderId.ToString() },
                    { "PaymentType", "OneTime_Order" }
                }
            };

            var service = new PaymentIntentService();
            PaymentIntent intent = await service.CreateAsync(options);

            // Devolvemos el clientSecret que Vue necesita para renderizar la tarjeta
            return Ok(new { clientSecret = intent.ClientSecret });
        }
        catch (StripeException e)
        {
            return BadRequest(new { message = e.StripeError.Message });
        }
    }

    /// <summary>
    /// Endpoint para suscribir a Productores o Distribuidores al SaaS mensual
    /// </summary>
    [HttpPost("subscribe")]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionResource resource)
    {
        try
        {
            // 1. Creamos el cliente en los servidores de Stripe
            var customerOptions = new CustomerCreateOptions
            {
                Email = resource.Email,
                Name = resource.Name
            };
            var customerService = new CustomerService();
            Customer customer = await customerService.CreateAsync(customerOptions);

            // 2. Creamos un intento de pago recurrente/inicial para el plan SaaS
            // Para simplificar la demo y evitar configurar catálogos de productos en el dashboard de Stripe,
            // cobramos una tarifa fija simulando la suscripción mensual ($29.00 USD).
            var options = new PaymentIntentCreateOptions
            {
                Amount = 2900, // $29.00 USD fijos en centavos
                Currency = "usd",
                Customer = customer.Id,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
                Metadata = new Dictionary<string, string>
                {
                    { "PlanType", resource.PlanType },
                    { "PaymentType", "SaaS_Subscription" },
                    { "UserEmail", resource.Email }
                }
            };

            var service = new PaymentIntentService();
            PaymentIntent intent = await service.CreateAsync(options);

            return Ok(new 
            { 
                customerId = customer.Id,
                clientSecret = intent.ClientSecret 
            });
        }
        catch (StripeException e)
        {
            return BadRequest(new { message = e.StripeError.Message });
        }
    }
}

// ── DTOs / RESOURCES PARA LAS PETICIONES ──────────────────────────────────────
public record CreatePaymentIntentResource(decimal Amount, string Currency, int OrderId);
public record CreateSubscriptionResource(string Email, string Name, string PlanType);