using RebooTech.Fruitlogix.Platform.Messaging.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Messaging.Application.Internal.QueryServices;

public class ConversationQueryService(
    IConversationRepository conversationRepository
) : IConversationQueryService
{
    public async Task<IEnumerable<Conversation>> Handle(GetAllConversationsQuery query) =>
        await conversationRepository.ListAsync();

    public async Task<IEnumerable<Conversation>> Handle(GetConversationsByUserIdQuery query) =>
        await conversationRepository.FindByUserIdAsync(query.UserId);
}