using EnterpriseEmployeeManagementAPI.Interfaces;
using EnterpriseEmployeeManagementAPI.Models.DTOs;
using EnterpriseEmployeeManagementAPI.Models.Entities;
using EnterpriseEmployeeManagementAPI.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace EnterpriseEmployeeManagementAPI.Tests;

public sealed class EmployeeServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 7, 24, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task GetAllAsync_ShouldMapEmployeesToDtos()
    {
        var repository = new Mock<IEmployeeRepository>();
        repository
            .Setup(item => item.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateEmployee()]);
        var service = CreateService(repository);

        var result = await service.GetAllAsync();

        result.Should().ContainSingle();
        result[0].EmployeeNumber.Should().Be("EMP-2001");
        result[0].DepartmentName.Should().Be("Engineering");
    }

    [Fact]
    public async Task CreateAsync_ShouldNormalizeAndPersistEmployee()
    {
        var repository = new Mock<IEmployeeRepository>();
        repository
            .Setup(item => item.EmailExistsAsync("alex.taylor@example.com", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository
            .Setup(item => item.EmployeeNumberExistsAsync("EMP-2002", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository
            .Setup(item => item.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee employee, CancellationToken _) =>
            {
                employee.Department = new Department { Id = 1, Code = "ENG", Name = "Engineering" };
                return employee;
            });
        var service = CreateService(repository);
        var request = new CreateEmployeeRequest(
            " emp-2002 ",
            " Alex ",
            " Taylor ",
            " Alex.Taylor@Example.com ",
            new DateOnly(1993, 5, 5),
            new DateOnly(2025, 1, 2),
            75000m,
            1);

        var result = await service.CreateAsync(request);

        result.EmployeeNumber.Should().Be("EMP-2002");
        result.Email.Should().Be("alex.taylor@example.com");
        result.CreatedAtUtc.Should().Be(Now);
        repository.Verify(
            item => item.AddAsync(
                It.Is<Employee>(employee => employee.FirstName == "Alex" && employee.LastName == "Taylor"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailExists_ShouldThrowConflictException()
    {
        var repository = new Mock<IEmployeeRepository>();
        repository
            .Setup(item => item.EmailExistsAsync("alex@example.com", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var service = CreateService(repository);
        var request = new CreateEmployeeRequest(
            "EMP-2002",
            "Alex",
            "Taylor",
            "alex@example.com",
            new DateOnly(1993, 5, 5),
            new DateOnly(2025, 1, 2),
            75000m,
            1);

        var action = () => service.CreateAsync(request);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*email address already exists*");
    }

    private static EmployeeService CreateService(Mock<IEmployeeRepository> repository) =>
        new(repository.Object, new FixedTimeProvider(Now), NullLogger<EmployeeService>.Instance);

    private static Employee CreateEmployee() => new()
    {
        Id = Guid.NewGuid(),
        EmployeeNumber = "EMP-2001",
        FirstName = "Jamie",
        LastName = "Morgan",
        Email = "jamie.morgan@example.com",
        DateOfBirth = new DateOnly(1991, 2, 3),
        HireDate = new DateOnly(2020, 4, 5),
        Salary = 80000m,
        DepartmentId = 1,
        Department = new Department { Id = 1, Code = "ENG", Name = "Engineering" },
        CreatedAtUtc = Now,
        UpdatedAtUtc = Now
    };

    private sealed class FixedTimeProvider(DateTimeOffset value) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => value;
    }
}
