using EnterpriseEmployeeManagementAPI.Models.Entities;

namespace EnterpriseEmployeeManagementAPI.Interfaces;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Employee>> SearchAsync(
        string? searchTerm,
        int? departmentId,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(
        string email,
        Guid? excludingEmployeeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> EmployeeNumberExistsAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default);

    Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default);

    Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);

    Task DeleteAsync(Employee employee, CancellationToken cancellationToken = default);
}
