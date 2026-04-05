namespace EFCoreDemo.Models;

public class AuditLog
{
    public int Id { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string Payload { get; set; } = "{}";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
