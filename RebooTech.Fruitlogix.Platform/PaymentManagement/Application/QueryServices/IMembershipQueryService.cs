using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Application.QueryServices;

public interface IMembershipQueryService
{
    Task<Membership?> Handle(GetMembershipByUserIdQuery query);
}
