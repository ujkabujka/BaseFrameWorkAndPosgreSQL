namespace EFCoreDemo.Models;

public class Enrollment
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int ProgressPercent { get; set; }
    public DateTime EnrolledAtUtc { get; set; } = DateTime.UtcNow;
    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
