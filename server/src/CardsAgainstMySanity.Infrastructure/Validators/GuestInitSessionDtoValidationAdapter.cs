namespace CardsAgainstMySanity.Infrastructure.Validators;

using System.Net;
using CardsAgainstMySanity.Domain.Auth.Dtos;
using CardsAgainstMySanity.SharedKernel;
using CardsAgainstMySanity.SharedKernel.Validation;
using FluentValidation;

public class GuestInitSessionDtoValidationAdapter : AbstractValidator<GuestInitSessionDto>, IModelValidator<GuestInitSessionDto>
{
    public GuestInitSessionDtoValidationAdapter() => this.AddRules();

    private void AddRules()
    {
        this.RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MinimumLength(1)
            .WithMessage("Username must be at least 1 characters long")
            .MaximumLength(24)
            .WithMessage("Username must be at most 24 characters long");

        this.RuleFor(x => x.IpAddress)
            .NotEmpty()
            .WithMessage("IpAddress is required")
            .Must(this.BeAValidAddress)
            .WithMessage("IpAddress must be a valid IP address");
    }

    private bool BeAValidAddress(string ipAddress) =>
        IPAddress.TryParse(ipAddress, out _);

    public new Result<GuestInitSessionDto, ValidationErrorList> Validate(GuestInitSessionDto model)
    {
        var result = base.Validate(model);

        return result.IsValid
            ? Result<GuestInitSessionDto, ValidationErrorList>.Ok(model)
            : Result<GuestInitSessionDto, ValidationErrorList>.Fail(result.Errors.ToValidationErrorList());
    }
}
