namespace RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Commands;

public record CreateConversationCommand(
    int OrderId,
    int ParticipantAId,
    int ParticipantBId
);