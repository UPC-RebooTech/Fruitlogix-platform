using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.Messaging.Application.QueryServices;

public interface IConversationQueryService
{
    Task<IEnumerable<Conversation>> Handle(GetAllConversationsQuery query);
    Task<IEnumerable<Conversation>> Handle(GetConversationsByUserIdQuery query);
}