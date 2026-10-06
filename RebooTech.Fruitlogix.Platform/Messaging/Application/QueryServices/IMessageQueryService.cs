using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.Messaging.Application.QueryServices;

public interface IMessageQueryService
{
    Task<IEnumerable<Message>> Handle(GetMessagesByConversationIdQuery query);
}