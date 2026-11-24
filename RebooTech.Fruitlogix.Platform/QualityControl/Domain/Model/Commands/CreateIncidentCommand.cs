namespace RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;

public record CreateIncidentCommand(
    int BatchId,
    string Description,
    List<string> EvidenceUrls,
    int? ReportedByUserId = null,
    int? OrderId = null
);