using RebooTech.Fruitlogix.Platform.Shared.Domain.Model.Entities;

namespace RebooTech.Fruitlogix.Platform.Iam.Domain.Model.Aggregates;

public partial class User : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}