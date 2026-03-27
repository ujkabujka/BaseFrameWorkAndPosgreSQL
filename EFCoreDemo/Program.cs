using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EFCoreDemo;

// This is the main entry point of our console application.
// Here we'll demonstrate basic database operations with Entity Framework Core.
Console.WriteLine("Entity Framework Core with PostgreSQL Demo");
Console.WriteLine("===========================================");

// Create a new instance of our DbContext.
// This establishes the connection to the database.
using (var context = new AppDbContext())
{
    Console.WriteLine("\n1. Ensuring database exists...");
    // EnsureCreated() creates the database if it doesn't exist.
    // In production, you'd typically use migrations instead (see below).
    context.Database.EnsureCreated();
    Console.WriteLine("Database ready!");

    Console.WriteLine("\n2. Checking if data already exists...");
    // Check if we already have data to avoid duplicates on multiple runs
    if (!context.Denemes.Any())
    {
        Console.WriteLine("No data found. Seeding with sample data...");

        // Add sample data to the Denemes table
        // This represents 5 people with names, heights, and ages
        context.Denemes.AddRange(
            new Deneme { Name = "Alice", Height = 165.5, Age = 25 },
            new Deneme { Name = "Bob", Height = 180.2, Age = 30 },
            new Deneme { Name = "Charlie", Height = 175.0, Age = 28 },
            new Deneme { Name = "Diana", Height = 160.3, Age = 22 },
            new Deneme { Name = "Eve", Height = 170.8, Age = 35 }
        );

        // SaveChanges() commits all changes to the database
        context.SaveChanges();
        Console.WriteLine("Sample data added!");
    }
    else
    {
        Console.WriteLine("Data already exists.");
    }

    Console.WriteLine("\n3. Retrieving and displaying all data...");
    // Query all records from the Denemes table
    // ToList() executes the query and returns results as a list
    var people = context.Denemes.ToList();

    Console.WriteLine("People in database:");
    foreach (var person in people)
    {
        Console.WriteLine($"Name: {person.Name}, Height: {person.Height}cm, Age: {person.Age} years");
    }

    Console.WriteLine("\n4. Example of filtering data...");
    // LINQ query to find people taller than 170cm
    var tallPeople = context.Denemes.Where(p => p.Height > 170).ToList();
    Console.WriteLine("People taller than 170cm:");
    foreach (var person in tallPeople)
    {
        Console.WriteLine($"Name: {person.Name}, Height: {person.Height}cm");
    }
}

Console.WriteLine("\nDemo completed!");

// Note about migrations:
// In a real application, instead of EnsureCreated(), you'd use:
// 1. dotnet ef migrations add InitialCreate  (creates migration files)
// 2. dotnet ef database update  (applies migrations to database)
// This allows versioning your database schema changes.
