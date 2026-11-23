using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Application.Internal.CommandServices;

public class MembershipCommandService(
    IMembershipRepository membershipRepository,
    IUnitOfWork unitOfWork) : IMembershipCommandService
{
    public async Task<Membership> Handle(ActivateProMembershipCommand command)
    {
        var membership = await membershipRepository.FindByUserIdAsync(command.UserId);

        // Idempotente: Stripe puede reenviar el mismo evento varias veces.
        if (membership is not null && membership.AlreadyApplied(command.GatewayRef))
            return membership;

        if (membership is null)
        {
            membership = new Membership(command.UserId);
            membership.ActivatePro(command.GatewayRef);
            await membershipRepository.AddAsync(membership);
        }
        else
        {
            membership.ActivatePro(command.GatewayRef);
            membershipRepository.Update(membership);
        }

        await unitOfWork.CompleteAsync();
        return membership;
    }
}
