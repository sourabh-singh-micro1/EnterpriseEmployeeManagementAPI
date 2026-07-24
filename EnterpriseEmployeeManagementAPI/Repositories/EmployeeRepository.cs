using EnterpriseEmployeeManagementAPI.Data;
using EnterpriseEmployeeManagementAPI.Interfaces;
using EnterpriseEmployeeManagementAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseEmployeeManagementAPI.Repositories;

public sealed class EmployeeRepository(ApplicationDbContext dbContext) : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Employees
            .AsNoTracking()
            .Include(employee => employee.Department)
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .ToListAsync(cancellationToken);

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Employees
            .Include(employee => employee.Department)
            .SingleOrDefaultAsync(employee => employee.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Employee>> SearchAsync(
        string? searchTerm,
        int? departmentId,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Employee> query = dbContext.Employees
            .AsNoTracking()
            .Include(employee => employee.Department);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm.Trim()}%";
            query = query.Where(employee =>
                EF.Functions.Like(employee.FirstName, pattern) ||
                EF.Functions.Like(employee.LastName, pattern) ||
                EF.Functions.Like(employee.Email, pattern) ||
                EF.Functions.Like(employee.EmployeeNumber, pattern));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(employee => employee.DepartmentId == departmentId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(employee => employee.IsActive == isActive.Value);
        }

        return await query
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> EmailExistsAsync(
        string email,
        Guid? excludingEmployeeId = null,
        CancellationToken cancellationToken = default) =>
        dbContext.Employees.AnyAsync(
            employee => employee.Email == email &&
                        (!excludingEmployeeId.HasValue || employee.Id != excludingEmployeeId.Value),
            cancellationToken);

    public Task<bool> EmployeeNumberExistsAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default) =>
        dbContext.Employees.AnyAsync(
            employee => employee.EmployeeNumber == employeeNumber,
            cancellationToken);

    public async Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await dbContext.Employees.AddAsync(employee, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.Entry(employee).Reference(item => item.Department).LoadAsync(cancellationToken);
        return employee;
    }

    public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.Entry(employee).Reference(item => item.Department).LoadAsync(cancellationToken);
    }

    public async Task DeleteAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
