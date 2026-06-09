using System.ComponentModel.DataAnnotations.Schema;

namespace Aris.Domain.Commons;

public abstract class BaseEntity
{
    public int Id { get; protected set; }

    [NotMapped]
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    [NotMapped]
    public bool Active { get; protected set; } = true;

    protected void Deactivate() => Active = false;
}
