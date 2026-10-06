using RebooTech.Fruitlogix.Platform.Messaging.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Messaging.Application.Internal.CommandServices;

public class MessageCommandService(
    IConversationRepository conversationRepository,
    IUnitOfWork unitOfWork
) : IMessageCommandService
{
    public async Task<Message> Handle(AddMessageToConversationCommand command)
    {
        var conversation = await conversationRepository.FindByIdAsync(command.ConversationId)
                           ?? throw new KeyNotFoundException(
                               $"Conversation {command.ConversationId} not found.");

        conversation.AddMessage(command.SenderId, command.Content);

        await unitOfWork.CompleteAsync();

        return conversation.Messages.Last();
    }
}