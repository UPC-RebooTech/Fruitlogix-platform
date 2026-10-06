using RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Logistics.Application.Internal.CommandServices;

public class TrackingLogCommandService : ITrackingLogCommandService
{
    private readonly ITrackingLogRepository _trackingLogRepository;
    private readonly IUnitOfWork            _unitOfWork;

    public TrackingLogCommandService(
        ITrackingLogRepository trackingLogRepository,
        IUnitOfWork unitOfWork)
    {
        _trackingLogRepository = trackingLogRepository;
        _unitOfWork            = unitOfWork;
    }

    public async Task<TrackingLog> Handle(CreateTrackingLogCommand command)
    {
        var log = new TrackingLog(command);
        await _trackingLogRepository.AddAsync(log);
        await _unitOfWork.CompleteAsync();
        return log;
    }
}