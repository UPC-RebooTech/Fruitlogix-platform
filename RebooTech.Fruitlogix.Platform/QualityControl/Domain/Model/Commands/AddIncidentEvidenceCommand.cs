namespace RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;

/// <summary>Agrega URLs de evidencia (ya subidas) a la incidencia.</summary>
public record AddIncidentEvidenceCommand(int IncidentId, List<string> EvidenceUrls);
