# SD Ticaret - Yönetim Sistemi

Modern .NET 9 teknolojileri ile geliştirilmiş, Clean Architecture prensiplerine uygun ticaret yönetim sistemi.

## 🏗️ Proje Yapısı

```
SD_Ticaret/
├── SDTicaret.Core/           # Domain Entities & Interfaces
├── SDTicaret.Application/    # Business Logic & Services
├── SDTicaret.Infrastructure/ # Data Access & External Services
├── SDTicaret.API/           # REST API
├── SDTicaret.Web/           # MVC Web Application
└── SDTicaret.Tests/         # Unit & Integration Tests
```

## 🚀 Teknolojiler

- **.NET 9** - Framework
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **AutoMapper** - Object mapping
- **FluentValidation** - Validation
- **JWT Bearer** - Authentication
- **Serilog** - Logging
- **Swagger/OpenAPI** - API documentation
- **xUnit** - Testing framework

## 🔧 Kurulum

Detaylı kurulum rehberi için [SETUP.md](SETUP.md) dosyasına bakın.

## 📝 API Dokümantasyonu

Swagger UI: `https://localhost:7244/swagger`

## 🧪 Test

```bash
# Tüm testleri çalıştır
dotnet test

# Sadece unit testler
dotnet test --filter Category=Unit

# Sadece integration testler
dotnet test --filter Category=Integration
```

## 📦 Deployment

### Docker
```bash
docker build -t sdticaret .
docker run -p 8080:80 sdticaret
```

## 📄 Lisans

MIT License - Detaylar için [LICENSE](LICENSE) dosyasına bakın.
