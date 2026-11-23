using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Application.CommandServices;

public interface IMembershipCommandService
{
    Task<Membership> Handle(ActivateProMembershipCommand command);
}
