using EnterpriseEmployeeManagementAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseEmployeeManagementAPI.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(department => department.Id);
            entity.Property(department => department.Name).HasMaxLength(100).IsRequired();
            entity.Property(department => department.Code).HasMaxLength(20).IsRequired();
            entity.HasIndex(department => department.Code).IsUnique();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(employee => employee.Id);
            entity.Property(employee => employee.EmployeeNumber).HasMaxLength(20).IsRequired();
            entity.Property(employee => employee.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(employee => employee.LastName).HasMaxLength(100).IsRequired();
            entity.Property(employee => employee.Email).HasMaxLength(254).IsRequired();
            entity.Property(employee => employee.Salary).HasPrecision(18, 2);
            entity.HasIndex(employee => employee.EmployeeNumber).IsUnique();
            entity.HasIndex(employee => employee.Email).IsUnique();
            entity.HasOne(employee => employee.Department)
                .WithMany(department => department.Employees)
                .HasForeignKey(employee => employee.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
