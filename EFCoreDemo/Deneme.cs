using System.ComponentModel.DataAnnotations;

namespace EFCoreDemo
{
    // This is our model class representing the "deneme" table.
    // Entity Framework Core will use this class to create the database table.
    // The [Key] attribute marks the Name property as the primary key.
    public class Deneme
    {
        [Key]  // Primary key for the table
        public string Name { get; set; } = string.Empty;  // String column

        public double Height { get; set; }  // Double column for height

        public int Age { get; set; }  // Int column for age
    }

    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; } = "";
        public string? Email { get; set; }
    }
}
