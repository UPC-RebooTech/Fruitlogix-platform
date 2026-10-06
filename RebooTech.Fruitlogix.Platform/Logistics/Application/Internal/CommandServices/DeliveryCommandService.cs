using RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Logistics.Application.Internal.CommandServices;

public class DeliveryCommandService : IDeliveryCommandService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork         _unitOfWork;

    public DeliveryCommandService(
        IDeliveryRepository deliveryRepository,
        IUnitOfWork unitOfWork)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork         = unitOfWork;
    }

    public async Task<Delivery> Handle(CreateDeliveryCommand command)
    {
        var delivery = new Delivery(command);
        await _deliveryRepository.AddAsync(delivery);
        await _unitOfWork.CompleteAsync();
        return delivery;
    }

    public async Task<Delivery> Handle(StartDispatchCommand command)
    {
        var delivery = await _deliveryRepository.FindByIdAsync(command.DeliveryId)
            ?? throw new KeyNotFoundException($"Delivery {command.DeliveryId} not found.");
        delivery.StartDispatch();
        _deliveryRepository.Update(delivery);
        await _unitOfWork.CompleteAsync();
        return delivery;
    }

    public async Task<Delivery> Handle(ReportDelayCommand command)
    {
        var delivery = await _deliveryRepository.FindByIdAsync(command.DeliveryId)
            ?? throw new KeyNotFoundException($"Delivery {command.DeliveryId} not found.");
        delivery.ReportDelay(command.Reason);
        _deliveryRepository.Update(delivery);
        await _unitOfWork.CompleteAsync();
        return delivery;
    }
}