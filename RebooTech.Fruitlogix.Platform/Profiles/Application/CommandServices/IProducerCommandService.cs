using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.Profiles.Application.CommandServices;

public interface IProducerCommandService
{
    Task<Producer> Handle(CreateProducerCommand command);
    
    Task<Producer> Handle(UpdateProducerCommand command);
    
    Task Handle(DeleteProducerCommand command);
    
}