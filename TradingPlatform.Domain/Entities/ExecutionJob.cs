namespace TradingPlatform.Domain.Entities;

public sealed class ExecutionJob
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? LockedAtUtc { get; set; }

    public string Status { get; set; } = "Pending";
}
