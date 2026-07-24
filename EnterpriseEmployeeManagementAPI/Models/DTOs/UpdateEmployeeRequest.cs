namespace EnterpriseEmployeeManagementAPI.Models.DTOs;

public sealed record UpdateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    DateOnly DateOfBirth,
    DateOnly HireDate,
    decimal Salary,
    int DepartmentId,
    bool IsActive);
