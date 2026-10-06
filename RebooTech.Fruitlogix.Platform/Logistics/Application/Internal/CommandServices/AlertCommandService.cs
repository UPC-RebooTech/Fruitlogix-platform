using RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Logistics.Application.Internal.CommandServices;

public class AlertCommandService : IAlertCommandService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IUnitOfWork      _unitOfWork;

    public AlertCommandService(IAlertRepository alertRepository, IUnitOfWork unitOfWork)
    {
        _alertRepository = alertRepository;
        _unitOfWork      = unitOfWork;
    }

    public async Task<Alert> Handle(CreateAlertCommand command)
    {
        var alert = new Alert(command);
        await _alertRepository.AddAsync(alert);
        await _unitOfWork.CompleteAsync();
        return alert;
    }

    public async Task<Alert> Handle(ResolveAlertCommand command)
    {
        var alert = await _alertRepository.FindByIdAsync(command.AlertId)
                    ?? throw new KeyNotFoundException($"Alert {command.AlertId} not found.");
        alert.Resolve();
        _alertRepository.Update(alert);
        await _unitOfWork.CompleteAsync();
        return alert;
    }
}