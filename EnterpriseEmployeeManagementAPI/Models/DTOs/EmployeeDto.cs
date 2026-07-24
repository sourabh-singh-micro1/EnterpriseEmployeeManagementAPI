namespace EnterpriseEmployeeManagementAPI.Models.DTOs;

public sealed record EmployeeDto(
    Guid Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Email,
    DateOnly DateOfBirth,
    DateOnly HireDate,
    decimal Salary,
    bool IsActive,
    int DepartmentId,
    string DepartmentName,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);
