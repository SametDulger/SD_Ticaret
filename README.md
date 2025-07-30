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

### Gereksinimler
- .NET 9 SDK
- SQL Server

### Adımlar

1. **Repository'yi klonlayın**
```bash
git clone https://github.com/SametDulger/SD_Ticaret.git
cd SD_Ticaret
```

2. **Veritabanı bağlantısını yapılandırın**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=SDTicaretDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

3. **Migration'ları çalıştırın**
```bash
dotnet ef database update -p SDTicaret.Infrastructure -s SDTicaret.API
```

4. **Projeyi çalıştırın**
```bash
# API (https://localhost:7244)
dotnet run --project SDTicaret.API

# Web (https://localhost:7068)
dotnet run --project SDTicaret.Web
```

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
