using EnterpriseEmployeeManagementAPI.Controllers;
using EnterpriseEmployeeManagementAPI.Interfaces;
using EnterpriseEmployeeManagementAPI.Models.DTOs;
using EnterpriseEmployeeManagementAPI.Validators;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace EnterpriseEmployeeManagementAPI.Tests;

public sealed class EmployeesControllerTests
{
    [Fact]
    public async Task GetById_WhenEmployeeDoesNotExist_ShouldReturnNotFound()
    {
        var service = new Mock<IEmployeeService>();
        service
            .Setup(item => item.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeDto?)null);
        var controller = CreateController(service);

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_WhenRequestIsValid_ShouldReturnCreatedAtAction()
    {
        var service = new Mock<IEmployeeService>();
        var request = CreateValidRequest();
        var employee = CreateEmployeeDto();
        service
            .Setup(item => item.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        var controller = CreateController(service);

        var result = await controller.Create(request, CancellationToken.None);

        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(EmployeesController.GetById));
        created.Value.Should().Be(employee);
    }

    [Fact]
    public async Task Create_WhenRequestIsInvalid_ShouldReturnValidationProblem()
    {
        var service = new Mock<IEmployeeService>();
        var request = CreateValidRequest() with { Email = "not-an-email" };
        var controller = CreateController(service);

        var result = await controller.Create(request, CancellationToken.None);

        var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().BeOfType<ValidationProblemDetails>();
        service.Verify(
            item => item.CreateAsync(It.IsAny<CreateEmployeeRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static EmployeesController CreateController(Mock<IEmployeeService> service) =>
        new(
            service.Object,
            new CreateEmployeeRequestValidator(TimeProvider.System),
            new UpdateEmployeeRequestValidator(TimeProvider.System),
            NullLogger<EmployeesController>.Instance);

    private static CreateEmployeeRequest CreateValidRequest() =>
        new(
            "EMP-4001",
            "Taylor",
            "Kim",
            "taylor.kim@example.com",
            new DateOnly(1994, 6, 10),
            new DateOnly(2024, 2, 1),
            72000m,
            1);

    private static EmployeeDto CreateEmployeeDto() =>
        new(
            Guid.NewGuid(),
            "EMP-4001",
            "Taylor",
            "Kim",
            "taylor.kim@example.com",
            new DateOnly(1994, 6, 10),
            new DateOnly(2024, 2, 1),
            72000m,
            true,
            1,
            "Engineering",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);
}
