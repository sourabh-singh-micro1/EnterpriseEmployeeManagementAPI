namespace EnterpriseEmployeeManagementAPI.Models.DTOs;

public sealed record CreateEmployeeRequest(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Email,
    DateOnly DateOfBirth,
    DateOnly HireDate,
    decimal Salary,
    int DepartmentId);
