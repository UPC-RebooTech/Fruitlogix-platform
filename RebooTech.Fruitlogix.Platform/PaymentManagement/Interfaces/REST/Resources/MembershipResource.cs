namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST.Resources;

public record MembershipResource(int UserId, string Plan, DateTimeOffset? ActivatedAt);
