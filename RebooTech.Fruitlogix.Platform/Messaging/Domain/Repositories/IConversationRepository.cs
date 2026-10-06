using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Messaging.Domain.Repositories;

public interface IConversationRepository : IBaseRepository<Conversation>
{
    Task<bool> ExistsByOrderIdAsync(int orderId);
    Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(int conversationId);
    Task<IEnumerable<Conversation>> FindByUserIdAsync(int userId);
}