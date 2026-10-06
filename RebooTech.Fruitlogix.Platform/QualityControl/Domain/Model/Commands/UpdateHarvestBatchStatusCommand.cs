using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.ValueObjects;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;

public record UpdateHarvestBatchStatusCommand(int BatchId, BatchStatus NewStatus);