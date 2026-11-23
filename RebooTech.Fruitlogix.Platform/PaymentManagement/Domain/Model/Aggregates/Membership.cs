using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Model;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;

/// <summary>
///     Membresía SaaS de un usuario. Solo cambia a PRO cuando el backend recibe
///     el evento <c>payment_intent.succeeded</c> con firma válida de Stripe.
/// </summary>
public class Membership : IAuditableEntity
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public MembershipPlan Plan { get; private set; }
    public string? GatewayRef { get; private set; }
    public DateTimeOffset? ActivatedAt { get; private set; }

    protected Membership() { }

    public Membership(int userId)
    {
        if (userId <= 0) throw new ArgumentException("UserId must be positive.");
        UserId = userId;
        Plan = MembershipPlan.FREE;
    }

    /// <summary>¿Este pago de Stripe ya fue aplicado? (Stripe puede reenviar eventos.)</summary>
    public bool AlreadyApplied(string gatewayRef) =>
        Plan == MembershipPlan.PRO && GatewayRef == gatewayRef;

    public void ActivatePro(string gatewayRef)
    {
        if (string.IsNullOrWhiteSpace(gatewayRef))
            throw new ArgumentException("GatewayRef is required.");
        Plan = MembershipPlan.PRO;
        GatewayRef = gatewayRef;
        ActivatedAt = DateTimeOffset.UtcNow;
    }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
