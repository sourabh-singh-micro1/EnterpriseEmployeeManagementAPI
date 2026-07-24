namespace EnterpriseEmployeeManagementAPI.Models.Entities;

public sealed class Employee
{
    public Guid Id { get; set; }

    public required string EmployeeNumber { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public DateOnly HireDate { get; set; }

    public decimal Salary { get; set; }

    public bool IsActive { get; set; } = true;

    public int DepartmentId { get; set; }

    public Department? Department { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }
}
