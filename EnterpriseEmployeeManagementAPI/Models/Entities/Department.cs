namespace EnterpriseEmployeeManagementAPI.Models.Entities;

public sealed class Department
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Code { get; set; }

    public ICollection<Employee> Employees { get; set; } = [];
}
