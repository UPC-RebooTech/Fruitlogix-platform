namespace RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Resources;

public record CreateIncidentResource(
    int BatchId,
    string Description,
    List<string> EvidenceUrls
);