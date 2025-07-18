# SD Ticaret - Yönetim Sistemi

Modern .NET 9 teknolojileri ile geliştirilmiş, Clean Architecture prensiplerine uygun ticaret yönetim sistemi.

> ⚠️ **Geliştirme Aşamasında** ⚠️
> 
> Bu proje aktif geliştirme aşamasındadır. Hatalar, eksik özellikler ve değişiklikler olabilir. Production ortamında kullanmadan önce kapsamlı test yapılması önerilir.

## 🏗️ Proje Yapısı

Proje Clean Architecture pattern'i kullanılarak 5 katmana ayrılmıştır:

```
SD_Ticaret/
├── SDTicaret.Core/           # Domain Entities & Interfaces
├── SDTicaret.Application/    # Business Logic & Services
├── SDTicaret.Infrastructure/ # Data Access & External Services
├── SDTicaret.API/           # REST API
└── SDTicaret.Web/           # MVC Web Application
```

### Katmanlar

- **Core**: Entity'ler, interface'ler ve domain logic
- **Application**: Business logic, DTO'lar, validation ve mapping
- **Infrastructure**: Database context, repository'ler ve external service'ler
- **API**: REST API endpoints ve authentication
- **Web**: MVC web uygulaması

## 🚀 Teknolojiler

### Backend
- **.NET 9** - Framework
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **AutoMapper** - Object mapping
- **FluentValidation** - Validation
- **JWT Bearer** - Authentication
- **Serilog** - Logging
- **Swagger/OpenAPI** - API documentation
- **Rate Limiting** - API throttling

### Frontend
- **ASP.NET Core MVC** - Web framework
- **Bootstrap** - CSS framework
- **jQuery** - JavaScript library
- **jQuery Validation** - Client-side validation

## 📊 Veritabanı Modelleri

### Ana Entity'ler
- **User** - Kullanıcı yönetimi ve authentication
- **Product** - Ürün katalog yönetimi
- **Category** - Kategori yönetimi
- **Customer** - Müşteri bilgileri
- **Order** - Sipariş yönetimi
- **OrderItem** - Sipariş detayları
- **Payment** - Ödeme işlemleri
- **Stock** - Stok yönetimi

### İş Süreçleri
- **Supplier** - Tedarikçi yönetimi
- **Employee** - Çalışan yönetimi
- **Branch** - Şube yönetimi
- **Contract** - Sözleşme yönetimi
- **Complaint** - Şikayet yönetimi
- **Survey** - Anket yönetimi
- **Campaign** - Kampanya yönetimi

## 🔧 Kurulum

### Gereksinimler
- .NET 9 SDK
- SQL Server (Express/Developer/Enterprise)
- Visual Studio 2022 veya VS Code

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

3. **Migration'ları oluşturun ve çalıştırın**
```bash
cd SDTicaret.API

# Migration oluştur (eğer model değişiklikleri varsa)
dotnet ef migrations add InitialCreate

# Migration'ları veritabanına uygula
dotnet ef database update
```

4. **Projeyi çalıştırın**
```bash
# API (https://localhost:7244)
dotnet run --project SDTicaret.API

# Web (https://localhost:7068)
dotnet run --project SDTicaret.Web
```

## 🔐 Authentication

Sistem JWT Bearer token authentication kullanır:

- **Secret Key**: 32+ karakter
- **Issuer**: SDTicaretAPI
- **Audience**: SDTicaretClient
- **Expiry**: 1 saat

### API Endpoints

#### Authentication
- `POST /api/auth/login` - Giriş
- `POST /api/auth/register` - Kayıt
- `POST /api/auth/refresh` - Token yenileme

#### CRUD Operations
Her entity için standart CRUD endpoint'leri:
- `GET /api/{entity}` - Listeleme
- `GET /api/{entity}/{id}` - Detay
- `POST /api/{entity}` - Ekleme
- `PUT /api/{entity}/{id}` - Güncelleme
- `DELETE /api/{entity}/{id}` - Silme

**Mevcut Controller'lar**: Auth, Users, Products, Categories, Customers, Suppliers, Branches, Employees, Orders, OrderItems, Payments, Contracts, Complaints, Surveys, Campaigns, Stocks, Health

## 📝 API Dokümantasyonu

Swagger UI üzerinden API dokümantasyonuna erişim:
- **Development**: `https://localhost:7244/swagger`
- **Production**: `https://your-domain.com/swagger`

## 🛡️ Güvenlik

- **JWT Authentication** - Token tabanlı kimlik doğrulama
- **Rate Limiting** - API throttling (100 request/dakika)
- **Input Validation** - FluentValidation ile giriş doğrulama
- **Exception Handling** - Global exception middleware
- **HTTPS** - Güvenli bağlantı

## 📊 Logging

Serilog ile yapılandırılmış logging sistemi:
- **Console** - Development ortamında
- **File** - Production ortamında (günlük rotasyon)
- **Retention** - 30 gün

## 🧪 Test

```bash
# Unit testler
dotnet test

# Integration testler
dotnet test --filter Category=Integration
```

> **Not**: Test coverage henüz tamamlanmamıştır. Bazı özellikler için test yazılması gerekmektedir.

## 📦 Deployment

> **⚠️ Uyarı**: Bu proje henüz production ortamı için hazır değildir. Deployment öncesi güvenlik, performans ve stabilite testleri yapılmalıdır.

### Docker (Önerilen)
```bash
docker build -t sdticaret .
docker run -p 8080:80 sdticaret
```

### IIS
1. Publish işlemi yapın
2. IIS'de application pool oluşturun
3. Site'ı yapılandırın

### Azure
1. Azure App Service oluşturun
2. GitHub Actions ile CI/CD kurun
3. Environment variables'ları yapılandırın

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit yapın (`git commit -m 'Add amazing feature'`)
4. Push yapın (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 🐛 Bilinen Sorunlar ve Eksikler

### Mevcut Eksiklikler
- [ ] Unit test coverage tamamlanmamış
- [ ] Integration testler eksik
- [ ] Performance optimization gerekli
- [ ] Security audit tamamlanmamış
- [ ] Error handling geliştirilmeli
- [ ] Logging seviyeleri optimize edilmeli

### Planlanan Özellikler
- [ ] Real-time notifications
- [ ] Advanced reporting
- [ ] Multi-language support
- [ ] Advanced search functionality
- [ ] Payment gateway integration
- [ ] Email notification system
