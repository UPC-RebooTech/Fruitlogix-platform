// Messaging/Interfaces/REST/Transform/MessageResourceAssembler.cs

using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Messaging.Interfaces.REST.Resources;

namespace RebooTech.Fruitlogix.Platform.Messaging.Interfaces.REST.Transform;

public static class MessageResourceAssembler
{
    public static AddMessageToConversationCommand ToCommandFromResource(
        int conversationId, SendMessageResource resource) =>
        new(conversationId, resource.SenderId, resource.Content);

    public static MessageResource ToResourceFromEntity(Message message) =>
        new(
            message.Id,
            message.SenderId,
            message.Content.Value,
            message.SentAt
        );
}