using EnterpriseEmployeeManagementAPI.Configuration;
using EnterpriseEmployeeManagementAPI.Data;
using EnterpriseEmployeeManagementAPI.HealthChecks;
using EnterpriseEmployeeManagementAPI.Interfaces;
using EnterpriseEmployeeManagementAPI.Models.DTOs;
using EnterpriseEmployeeManagementAPI.Repositories;
using EnterpriseEmployeeManagementAPI.Services;
using EnterpriseEmployeeManagementAPI.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseEmployeeManagementAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmployeeManagement(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IValidator<CreateEmployeeRequest>, CreateEmployeeRequestValidator>();
        services.AddScoped<IValidator<UpdateEmployeeRequest>, UpdateEmployeeRequestValidator>();
        services
            .AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);

        return services;
    }

    public static async Task InitializeEmployeeDatabaseAsync(
        this WebApplication app,
        CancellationToken cancellationToken = default)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedData.InitializeAsync(dbContext, cancellationToken);
    }
}
