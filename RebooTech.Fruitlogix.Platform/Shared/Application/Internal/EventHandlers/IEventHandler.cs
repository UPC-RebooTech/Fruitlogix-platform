using Cortex.Mediator.Notifications;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Model.Events;

namespace RebooTech.Fruitlogix.Platform.Shared.Application.Internal.EventHandlers;
public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
} 