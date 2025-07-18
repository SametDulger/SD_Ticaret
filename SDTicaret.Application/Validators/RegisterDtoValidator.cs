using FluentValidation;
using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Kullanıcı adı boş olamaz")
            .Length(3, 50).WithMessage("Kullanıcı adı 3-50 karakter arasında olmalıdır")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boş olamaz")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz")
            .MaximumLength(100).WithMessage("E-posta en fazla 100 karakter olabilir");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$")
            .WithMessage("Şifre en az bir küçük harf, bir büyük harf ve bir rakam içermelidir");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Şifreler eşleşmiyor");

        RuleFor(x => x.FirstName)
            .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").When(x => !string.IsNullOrEmpty(x.FirstName))
            .WithMessage("Ad sadece harf içerebilir");

        RuleFor(x => x.LastName)
            .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").When(x => !string.IsNullOrEmpty(x.LastName))
            .WithMessage("Soyad sadece harf içerebilir");
    }
} 