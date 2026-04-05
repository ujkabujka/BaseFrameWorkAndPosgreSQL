namespace EFCoreDemo.Models;

public class CourseDetails
{
    public string Summary { get; set; } = string.Empty;
    public int EstimatedHours { get; set; }
    public List<CourseModule> Modules { get; set; } = new();
}

public class CourseModule
{
    public string Title { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
}
