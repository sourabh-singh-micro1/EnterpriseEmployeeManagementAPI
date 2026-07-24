using EnterpriseEmployeeManagementAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseEmployeeManagementAPI.Data;

public static class SeedData
{
    public static async Task InitializeAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.Departments.AnyAsync(cancellationToken))
        {
            return;
        }

        var engineering = new Department { Id = 1, Code = "ENG", Name = "Engineering" };
        var humanResources = new Department { Id = 2, Code = "HR", Name = "Human Resources" };
        var finance = new Department { Id = 3, Code = "FIN", Name = "Finance" };

        await dbContext.Departments.AddRangeAsync(
            [engineering, humanResources, finance],
            cancellationToken);

        var seededAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        await dbContext.Employees.AddRangeAsync(
            [
                new Employee
                {
                    Id = Guid.Parse("bf1d8da7-b7a7-4fd2-9f50-9a73afc7ee70"),
                    EmployeeNumber = "EMP-1001",
                    FirstName = "Aarav",
                    LastName = "Sharma",
                    Email = "aarav.sharma@example.com",
                    DateOfBirth = new DateOnly(1990, 4, 12),
                    HireDate = new DateOnly(2021, 2, 15),
                    Salary = 95000m,
                    IsActive = true,
                    DepartmentId = engineering.Id,
                    CreatedAtUtc = seededAt,
                    UpdatedAtUtc = seededAt
                },
                new Employee
                {
                    Id = Guid.Parse("7390df5d-ac04-4576-80e3-d091f66b03a5"),
                    EmployeeNumber = "EMP-1002",
                    FirstName = "Maya",
                    LastName = "Patel",
                    Email = "maya.patel@example.com",
                    DateOfBirth = new DateOnly(1988, 9, 3),
                    HireDate = new DateOnly(2019, 7, 1),
                    Salary = 88000m,
                    IsActive = true,
                    DepartmentId = humanResources.Id,
                    CreatedAtUtc = seededAt,
                    UpdatedAtUtc = seededAt
                },
                new Employee
                {
                    Id = Guid.Parse("6886eeb1-f7a8-4e0d-a565-adc566ea34ef"),
                    EmployeeNumber = "EMP-1003",
                    FirstName = "Noah",
                    LastName = "Williams",
                    Email = "noah.williams@example.com",
                    DateOfBirth = new DateOnly(1992, 1, 28),
                    HireDate = new DateOnly(2022, 10, 10),
                    Salary = 91000m,
                    IsActive = true,
                    DepartmentId = finance.Id,
                    CreatedAtUtc = seededAt,
                    UpdatedAtUtc = seededAt
                }
            ],
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
