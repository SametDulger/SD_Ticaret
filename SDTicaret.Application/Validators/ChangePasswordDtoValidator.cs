using FluentValidation;
using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Validators;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Mevcut şifre boş olamaz");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni şifre boş olamaz")
            .MinimumLength(6).WithMessage("Yeni şifre en az 6 karakter olmalıdır")
            .NotEqual(x => x.CurrentPassword).WithMessage("Yeni şifre mevcut şifre ile aynı olamaz");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Şifre tekrarı boş olamaz")
            .Equal(x => x.NewPassword).WithMessage("Şifreler eşleşmiyor");
    }
} 