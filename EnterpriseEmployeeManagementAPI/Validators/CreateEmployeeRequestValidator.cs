using EnterpriseEmployeeManagementAPI.Models.DTOs;
using FluentValidation;

namespace EnterpriseEmployeeManagementAPI.Validators;

public sealed class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(request => request.EmployeeNumber)
            .NotEmpty()
            .MaximumLength(20)
            .Matches("^[A-Za-z0-9-]+$");
        RuleFor(request => request.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(request => request.LastName).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(request => request.DateOfBirth)
            .LessThan(DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime));
        RuleFor(request => request.HireDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime));
        RuleFor(request => request.Salary).GreaterThanOrEqualTo(0);
        RuleFor(request => request.DepartmentId).GreaterThan(0);
    }
}
