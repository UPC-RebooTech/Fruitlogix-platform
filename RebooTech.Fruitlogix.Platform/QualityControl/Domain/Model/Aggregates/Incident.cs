using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Model;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;

public class Incident : IAuditableEntity
{
    public int Id { get; private set; }
    public int BatchId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string EvidenceUrls { get; private set; } = string.Empty; // JSON serializado
    public IncidentStatus Status { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }
    /// <summary>Usuario IAM que reportó la incidencia (null en datos antiguos).</summary>
    public int? ReportedByUserId { get; private set; }
    /// <summary>Pedido relacionado (null en datos antiguos).</summary>
    public int? OrderId { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    /// <summary>Máximo de imágenes de evidencia por incidencia.</summary>
    public const int MaxEvidenceFiles = 5;

    public Incident() { }

    public Incident(int batchId, string description, List<string>? evidenceUrls,
        int? reportedByUserId = null, int? orderId = null)
    {
        ReportedByUserId = reportedByUserId;
        OrderId      = orderId;
        BatchId      = batchId;
        Description  = description;
        evidenceUrls ??= [];
        if (evidenceUrls.Count > MaxEvidenceFiles)
            throw new ArgumentException($"An incident can have at most {MaxEvidenceFiles} evidence files.");
        EvidenceUrls = System.Text.Json.JsonSerializer.Serialize(evidenceUrls);
        Status       = IncidentStatus.Open;
        ResolvedAt   = null;
    }

    // Helpers para que el Controller/Service no trabajen con JSON crudo
    public List<string> GetEvidenceUrls() =>
        System.Text.Json.JsonSerializer.Deserialize<List<string>>(EvidenceUrls) ?? [];

    public int RemainingEvidenceSlots => Math.Max(0, MaxEvidenceFiles - GetEvidenceUrls().Count);

    public void AddEvidenceUrls(IEnumerable<string> urls)
    {
        var current = GetEvidenceUrls();
        var newUrls = urls.Where(u => !string.IsNullOrWhiteSpace(u)).ToList();
        if (current.Count + newUrls.Count > MaxEvidenceFiles)
            throw new InvalidOperationException(
                $"An incident can have at most {MaxEvidenceFiles} evidence files ({current.Count} already attached).");
        current.AddRange(newUrls);
        EvidenceUrls = System.Text.Json.JsonSerializer.Serialize(current);
    }

    public void UpdateStatus(IncidentStatus newStatus)
    {
        Status     = newStatus;
        ResolvedAt = newStatus == IncidentStatus.Resolved
            ? DateTimeOffset.UtcNow
            : ResolvedAt;
    }
}