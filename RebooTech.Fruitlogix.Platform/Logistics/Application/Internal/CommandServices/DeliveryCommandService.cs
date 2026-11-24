using RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Logistics.Application.Internal.CommandServices;

public class DeliveryCommandService : IDeliveryCommandService
{
    private readonly IDeliveryRepository      _deliveryRepository;
    private readonly IDeliveryEventRepository _deliveryEventRepository;
    private readonly IUnitOfWork              _unitOfWork;

    public DeliveryCommandService(
        IDeliveryRepository deliveryRepository,
        IDeliveryEventRepository deliveryEventRepository,
        IUnitOfWork unitOfWork)
    {
        _deliveryRepository      = deliveryRepository;
        _deliveryEventRepository = deliveryEventRepository;
        _unitOfWork              = unitOfWork;
    }

    public async Task<Delivery> Handle(CreateDeliveryCommand command)
    {
        var delivery = new Delivery(command);
        await _deliveryRepository.AddAsync(delivery);
        await _unitOfWork.CompleteAsync(); // genera delivery.Id para el evento
        await _deliveryEventRepository.AddAsync(
            new DeliveryEvent(delivery.Id, DeliveryEventType.Created, command.ActorUserId));
        await _unitOfWork.CompleteAsync();
        return delivery;
    }

    public async Task<Delivery> Handle(StartDispatchCommand command)
    {
        var delivery = await FindOrThrowAsync(command.DeliveryId);
        delivery.StartDispatch();
        _deliveryRepository.Update(delivery);
        await _deliveryEventRepository.AddAsync(
            new DeliveryEvent(delivery.Id, DeliveryEventType.Dispatched, command.ActorUserId));
        await _unitOfWork.CompleteAsync();
        return delivery;
    }

    public async Task<Delivery> Handle(ReportDelayCommand command)
    {
        var delivery = await FindOrThrowAsync(command.DeliveryId);
        delivery.ReportDelay(command.Reason);
        _deliveryRepository.Update(delivery);
        await _deliveryEventRepository.AddAsync(
            new DeliveryEvent(delivery.Id, DeliveryEventType.Delayed, command.ActorUserId, note: delivery.DelayReason));
        await _unitOfWork.CompleteAsync();
        return delivery;
    }

    public async Task<int> Handle(MarkDeliveriesDeliveredByOrderCommand command)
    {
        var deliveries = await _deliveryRepository.FindByOrderIdAsync(command.OrderId);
        var changed = 0;
        foreach (var delivery in deliveries.Where(d => d.CurrentStatus != DeliveryStatus.Delivered))
        {
            delivery.MarkDelivered();
            _deliveryRepository.Update(delivery);
            await _deliveryEventRepository.AddAsync(
                new DeliveryEvent(delivery.Id, DeliveryEventType.Delivered, command.ActorUserId));
            changed++;
        }
        if (changed > 0) await _unitOfWork.CompleteAsync();
        return changed;
    }

    public async Task<Delivery> Handle(ArriveDeliveryCommand command)
    {
        var delivery = await FindOrThrowAsync(command.DeliveryId);
        delivery.Arrive();
        _deliveryRepository.Update(delivery);
        await _deliveryEventRepository.AddAsync(new DeliveryEvent(delivery.Id, DeliveryEventType.Arrived,
            command.ActorUserId, note: command.Note, latitude: command.Latitude, longitude: command.Longitude));
        await _unitOfWork.CompleteAsync();
        return delivery;
    }

    public async Task<Delivery> Handle(UpdateDeliveryAssignmentCommand command)
    {
        var delivery = await FindOrThrowAsync(command.DeliveryId);
        delivery.UpdateAssignment(command.DriverName, command.DriverPhone, command.VehiclePlate, command.VehicleType);
        _deliveryRepository.Update(delivery);
        await _unitOfWork.CompleteAsync();
        return delivery;
    }

    public async Task<DeliveryEvent> Handle(AddDeliveryCheckpointCommand command)
    {
        var delivery = await FindOrThrowAsync(command.DeliveryId);
        delivery.EnsureCanRecordCheckpoint();
        var checkpoint = new DeliveryEvent(delivery.Id, DeliveryEventType.Checkpoint, command.ActorUserId,
            command.Name, command.Note, command.Latitude, command.Longitude, command.OccurredAt);
        await _deliveryEventRepository.AddAsync(checkpoint);
        await _unitOfWork.CompleteAsync();
        return checkpoint;
    }

    private async Task<Delivery> FindOrThrowAsync(int deliveryId) =>
        await _deliveryRepository.FindByIdAsync(deliveryId)
        ?? throw new KeyNotFoundException($"Delivery {deliveryId} not found.");
}
