using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Application.Internal.QueryServices;

public class MembershipQueryService(IMembershipRepository membershipRepository) : IMembershipQueryService
{
    public async Task<Membership?> Handle(GetMembershipByUserIdQuery query) =>
        await membershipRepository.FindByUserIdAsync(query.UserId);
}
