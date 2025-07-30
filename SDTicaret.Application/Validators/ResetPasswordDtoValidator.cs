using FluentValidation;
using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Validators;

public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token boş olamaz");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni şifre boş olamaz")
            .MinimumLength(6).WithMessage("Yeni şifre en az 6 karakter olmalıdır");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Şifre tekrarı boş olamaz")
            .Equal(x => x.NewPassword).WithMessage("Şifreler eşleşmiyor");
    }
} 