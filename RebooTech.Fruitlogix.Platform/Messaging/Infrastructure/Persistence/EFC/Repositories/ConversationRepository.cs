using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.Messaging.Infrastructure.Persistence.EFC.Repositories;

public class ConversationRepository(AppDbContext context)
    : BaseRepository<Conversation>(context), IConversationRepository
{
    public async Task<bool> ExistsByOrderIdAsync(int orderId) =>
        await Context.Set<Conversation>().AnyAsync(c => c.OrderId == orderId);
    public async Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(int conversationId)
    {
        var conversation = await Context.Set<Conversation>()
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        return conversation?.Messages ?? Enumerable.Empty<Message>();
    }
    public async Task<IEnumerable<Conversation>> FindByUserIdAsync(int userId) =>
        await Context.Set<Conversation>()
            .Where(c => c.ParticipantAId == userId || c.ParticipantBId == userId)
            .ToListAsync();
}