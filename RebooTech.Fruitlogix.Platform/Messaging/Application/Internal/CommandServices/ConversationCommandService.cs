using RebooTech.Fruitlogix.Platform.Messaging.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Messaging.Application.Internal.CommandServices;

public class ConversationCommandService(
    IConversationRepository conversationRepository,
    IUnitOfWork unitOfWork
) : IConversationCommandService
{
    public async Task<Conversation> Handle(CreateConversationCommand command)
    {
        if (await conversationRepository.ExistsByOrderIdAsync(command.OrderId))
            throw new InvalidOperationException(
                $"A conversation for order {command.OrderId} already exists.");

        var conversation = new Conversation(command);
        await conversationRepository.AddAsync(conversation);
        await unitOfWork.CompleteAsync();
        return conversation;
    }
}