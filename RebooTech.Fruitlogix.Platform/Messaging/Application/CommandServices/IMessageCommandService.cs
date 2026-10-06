using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Entities;

namespace RebooTech.Fruitlogix.Platform.Messaging.Application.CommandServices;

public interface IMessageCommandService
{
    Task<Message> Handle(AddMessageToConversationCommand command);
}