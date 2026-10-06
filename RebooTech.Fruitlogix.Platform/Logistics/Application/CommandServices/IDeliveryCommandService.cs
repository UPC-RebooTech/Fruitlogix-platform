using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;

public interface IDeliveryCommandService
{
    Task<Delivery> Handle(CreateDeliveryCommand command);
    Task<Delivery> Handle(StartDispatchCommand command);
    Task<Delivery> Handle(ReportDelayCommand command);
}