using EnterpriseEmployeeManagementAPI.Models.DTOs;
using FluentValidation;

namespace EnterpriseEmployeeManagementAPI.Validators;

public sealed class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator(TimeProvider timeProvider)
    {
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
