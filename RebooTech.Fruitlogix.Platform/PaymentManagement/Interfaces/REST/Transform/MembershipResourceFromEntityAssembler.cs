using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST.Resources;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST.Transform;

public static class MembershipResourceFromEntityAssembler
{
    public static MembershipResource ToResourceFromEntity(Membership entity) =>
        new(entity.UserId, entity.Plan.ToString(), entity.ActivatedAt);

    /// <summary>Usuario sin registro de membresía: plan gratuito.</summary>
    public static MembershipResource Free(int userId) =>
        new(userId, MembershipPlan.FREE.ToString(), null);
}
