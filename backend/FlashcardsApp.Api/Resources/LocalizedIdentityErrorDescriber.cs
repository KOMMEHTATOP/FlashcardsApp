using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace FlashcardsApp.Api.Resources;

public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
{
    private readonly IStringLocalizer<IdentityErrorMessages> _localizer;

    public LocalizedIdentityErrorDescriber(IStringLocalizer<IdentityErrorMessages> localizer)
    {
        _localizer = localizer;
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
        => new()
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = _localizer[nameof(PasswordRequiresNonAlphanumeric)]
        };

    public override IdentityError PasswordRequiresLower()
        => new()
        {
            Code = nameof(PasswordRequiresLower),
            Description = _localizer[nameof(PasswordRequiresLower)]
        };

    public override IdentityError PasswordRequiresUpper()
        => new()
        {
            Code = nameof(PasswordRequiresUpper),
            Description = _localizer[nameof(PasswordRequiresUpper)]
        };

    public override IdentityError PasswordRequiresDigit()
        => new()
        {
            Code = nameof(PasswordRequiresDigit),
            Description = _localizer[nameof(PasswordRequiresDigit)]
        };

    public override IdentityError PasswordTooShort(int length)
        => new()
        {
            Code = nameof(PasswordTooShort),
            Description = _localizer[nameof(PasswordTooShort), length]
        };

    public override IdentityError DuplicateUserName(string userName)
        => new()
        {
            Code = nameof(DuplicateUserName),
            Description = _localizer[nameof(DuplicateUserName), userName]
        };

    public override IdentityError DuplicateEmail(string email)
        => new()
        {
            Code = nameof(DuplicateEmail),
            Description = _localizer[nameof(DuplicateEmail), email]
        };

    public override IdentityError InvalidEmail(string email)
        => new()
        {
            Code = nameof(InvalidEmail),
            Description = _localizer[nameof(InvalidEmail), email]
        };

    public override IdentityError PasswordMismatch()
        => new()
        {
            Code = nameof(PasswordMismatch),
            Description = _localizer[nameof(PasswordMismatch)]
        };

    public override IdentityError InvalidToken()
        => new()
        {
            Code = nameof(InvalidToken),
            Description = _localizer[nameof(InvalidToken)]
        };

    public override IdentityError DefaultError()
        => new()
        {
            Code = nameof(DefaultError),
            Description = _localizer[nameof(DefaultError)]
        };

    public override IdentityError InvalidUserName(string userName)
        => new()
        {
            Code = nameof(InvalidUserName),
            Description = _localizer[nameof(InvalidUserName), userName]
        };

    public override IdentityError DuplicateRoleName(string role)
        => new()
        {
            Code = nameof(DuplicateRoleName),
            Description = _localizer[nameof(DuplicateRoleName), role]
        };
}