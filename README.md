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
- **Bootstrap 5** - UI Framework

## 🔧 Kurulum

Detaylı kurulum rehberi için [SETUP.md](SETUP.md) dosyasına bakın.

### Hızlı Başlangıç

```bash
# Projeyi klonlayın
git clone <repository-url>
cd SD_Ticaret

# Veritabanını oluşturun
cd SDTicaret.API
dotnet ef database update

# API'yi çalıştırın
dotnet run

# Yeni terminal açın ve Web uygulamasını çalıştırın
cd ../SDTicaret.Web
dotnet run
```

## 📝 API Dokümantasyonu

Swagger UI: `https://localhost:7244/swagger`

## 🧪 Test

```bash
# Tüm testleri çalıştır
dotnet test
```

## 📦 Deployment

### Docker ile Deployment

```bash
# Docker Compose ile tüm servisleri başlatın
docker-compose up -d
```

### Manuel Deployment

```bash
# Production build
dotnet publish -c Release
```

## 🎨 Özellikler

- **Ürün Yönetimi**: Ürünler, kategoriler, tedarikçiler
- **Satış Yönetimi**: Siparişler, ödemeler
- **Kişi Yönetimi**: Müşteriler, çalışanlar
- **İş Yönetimi**: Şubeler, sözleşmeler, kampanyalar
- **Raporlama**: Dashboard, satış, stok, müşteri raporları
- **Güvenlik**: JWT authentication, rate limiting

## 🌐 Erişim URL'leri

- **Web Uygulaması**: `https://localhost:5244`
- **API**: `https://localhost:7244`
- **Swagger UI**: `https://localhost:7244/swagger`

## 📄 Lisans

MIT License - Detaylar için [LICENSE](LICENSE) dosyasına bakın.
