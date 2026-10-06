using RebooTech.Fruitlogix.Platform.Messaging.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Messaging.Application.Internal.QueryServices;

public class MessageQueryService(
    IConversationRepository conversationRepository
) : IMessageQueryService
{
    public async Task<IEnumerable<Message>> Handle(GetMessagesByConversationIdQuery query)
    {
        var conversationExists = await conversationRepository
            .FindByIdAsync(query.ConversationId);

        if (conversationExists is null)
            throw new KeyNotFoundException(
                $"Conversation {query.ConversationId} not found.");

        return await conversationRepository
            .GetMessagesByConversationIdAsync(query.ConversationId);
    }
}