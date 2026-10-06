using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.Messaging.Application.CommandServices;

public interface IConversationCommandService
{
    Task<Conversation> Handle(CreateConversationCommand command);
}