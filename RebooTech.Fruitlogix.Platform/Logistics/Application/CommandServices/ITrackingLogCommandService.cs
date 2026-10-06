using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;

namespace RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;

public interface ITrackingLogCommandService
{
    Task<TrackingLog> Handle(CreateTrackingLogCommand command);
}