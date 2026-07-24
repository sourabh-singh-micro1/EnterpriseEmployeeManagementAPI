using EnterpriseEmployeeManagementAPI.Data;
using EnterpriseEmployeeManagementAPI.Models.Entities;
using EnterpriseEmployeeManagementAPI.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EnterpriseEmployeeManagementAPI.Tests;

public sealed class EmployeeRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnEmployeesWithDepartments()
    {
        await using var dbContext = CreateDbContext();
        await SeedAsync(dbContext);
        var repository = new EmployeeRepository(dbContext);

        var employees = await repository.GetAllAsync();

        employees.Should().ContainSingle();
        employees[0].Department.Should().NotBeNull();
        employees[0].Department!.Name.Should().Be("Engineering");
    }

    [Fact]
    public async Task SearchAsync_ShouldApplyDepartmentAndStatusFilters()
    {
        await using var dbContext = CreateDbContext();
        await SeedAsync(dbContext);
        var repository = new EmployeeRepository(dbContext);

        var employees = await repository.SearchAsync(null, 1, true);
        var missing = await repository.SearchAsync(null, 2, true);

        employees.Should().ContainSingle();
        missing.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEmployee()
    {
        await using var dbContext = CreateDbContext();
        await SeedAsync(dbContext);
        var repository = new EmployeeRepository(dbContext);
        var employee = await repository.GetByIdAsync(EmployeeId);

        await repository.DeleteAsync(employee!);

        (await repository.GetByIdAsync(EmployeeId)).Should().BeNull();
    }

    private static readonly Guid EmployeeId = Guid.Parse("8a71e7e1-ce4e-47e6-9bf3-0b560b4ae7d8");

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static async Task SeedAsync(ApplicationDbContext dbContext)
    {
        var department = new Department { Id = 1, Code = "ENG", Name = "Engineering" };
        var employee = new Employee
        {
            Id = EmployeeId,
            EmployeeNumber = "EMP-3001",
            FirstName = "Jordan",
            LastName = "Lee",
            Email = "jordan.lee@example.com",
            DateOfBirth = new DateOnly(1990, 1, 1),
            HireDate = new DateOnly(2020, 1, 1),
            Salary = 70000m,
            IsActive = true,
            DepartmentId = department.Id,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        await dbContext.Departments.AddAsync(department);
        await dbContext.Employees.AddAsync(employee);
        await dbContext.SaveChangesAsync();
    }
}
