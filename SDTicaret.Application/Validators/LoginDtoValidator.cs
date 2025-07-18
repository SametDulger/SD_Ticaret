using FluentValidation;
using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Kullanıcı adı boş olamaz")
            .Length(3, 50).WithMessage("Kullanıcı adı 3-50 karakter arasında olmalıdır")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır");
    }
} 