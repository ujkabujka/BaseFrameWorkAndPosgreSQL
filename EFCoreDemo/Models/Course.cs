namespace EFCoreDemo.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsPublished { get; set; }
    public CourseDifficulty Difficulty { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public CourseDetails Details { get; set; } = new();
    public List<Enrollment> Enrollments { get; set; } = new();
}
