using EnterpriseEmployeeManagementAPI.Interfaces;
using EnterpriseEmployeeManagementAPI.Models.DTOs;
using EnterpriseEmployeeManagementAPI.Models.Entities;

namespace EnterpriseEmployeeManagementAPI.Services;

public sealed class EmployeeService(
    IEmployeeRepository employeeRepository,
    TimeProvider timeProvider,
    ILogger<EmployeeService> logger) : IEmployeeService
{
    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.GetAllAsync(cancellationToken);
        return employees.Select(MapToDto).ToList();
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken);
        return employee is null ? null : MapToDto(employee);
    }

    public async Task<IReadOnlyList<EmployeeDto>> SearchAsync(
        string? searchTerm,
        int? departmentId,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.SearchAsync(
            searchTerm,
            departmentId,
            isActive,
            cancellationToken);
        return employees.Select(MapToDto).ToList();
    }

    public async Task<EmployeeDto> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedEmployeeNumber = request.EmployeeNumber.Trim().ToUpperInvariant();

        if (await employeeRepository.EmailExistsAsync(normalizedEmail, null, cancellationToken))
        {
            throw new InvalidOperationException("An employee with this email address already exists.");
        }

        if (await employeeRepository.EmployeeNumberExistsAsync(normalizedEmployeeNumber, cancellationToken))
        {
            throw new InvalidOperationException("An employee with this employee number already exists.");
        }

        var now = timeProvider.GetUtcNow();
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            EmployeeNumber = normalizedEmployeeNumber,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            DateOfBirth = request.DateOfBirth,
            HireDate = request.HireDate,
            Salary = request.Salary,
            IsActive = true,
            DepartmentId = request.DepartmentId,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        var createdEmployee = await employeeRepository.AddAsync(employee, cancellationToken);
        logger.LogInformation(
            "Created employee {EmployeeId} with employee number {EmployeeNumber}",
            createdEmployee.Id,
            createdEmployee.EmployeeNumber);
        return MapToDto(createdEmployee);
    }

    public async Task<EmployeeDto?> UpdateAsync(
        Guid id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return null;
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await employeeRepository.EmailExistsAsync(normalizedEmail, id, cancellationToken))
        {
            throw new InvalidOperationException("An employee with this email address already exists.");
        }

        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = normalizedEmail;
        employee.DateOfBirth = request.DateOfBirth;
        employee.HireDate = request.HireDate;
        employee.Salary = request.Salary;
        employee.DepartmentId = request.DepartmentId;
        employee.IsActive = request.IsActive;
        employee.UpdatedAtUtc = timeProvider.GetUtcNow();

        await employeeRepository.UpdateAsync(employee, cancellationToken);
        logger.LogInformation("Updated employee {EmployeeId}", employee.Id);
        return MapToDto(employee);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return false;
        }

        await employeeRepository.DeleteAsync(employee, cancellationToken);
        logger.LogInformation("Deleted employee {EmployeeId}", employee.Id);
        return true;
    }

    private static EmployeeDto MapToDto(Employee employee) =>
        new(
            employee.Id,
            employee.EmployeeNumber,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.DateOfBirth,
            employee.HireDate,
            employee.Salary,
            employee.IsActive,
            employee.DepartmentId,
            employee.Department?.Name ?? string.Empty,
            employee.CreatedAtUtc,
            employee.UpdatedAtUtc);
}
