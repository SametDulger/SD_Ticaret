# SD Ticaret Projesi Kurulum Rehberi

## Gereksinimler
- .NET 9.0 SDK
- SQL Server (Express, LocalDB veya Full)
- Visual Studio 2022 veya VS Code

## Kurulum Adımları

### 1. Projeyi Klonlayın
```bash
git clone <repository-url>
cd SD_Ticaret
```

### 2. Veritabanı Bağlantısını Yapılandırın

#### API Projesi için:
1. `SDTicaret.API/appsettings.example.json` dosyasını `appsettings.json` olarak kopyalayın
2. Connection string'i kendi veritabanı ayarlarınıza göre güncelleyin:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
   }
   ```

3. Development ortamı için `appsettings.Development.example.json` dosyasını `appsettings.Development.json` olarak kopyalayın

#### Web Projesi için:
1. `SDTicaret.Web/appsettings.example.json` dosyasını `appsettings.json` olarak kopyalayın
2. API URL'ini güncelleyin (gerekirse):
   ```json
   "ApiSettings": {
     "BaseUrl": "http://localhost:5080/api/"
   }
   ```

### 3. JWT Secret Key'ini Güncelleyin
`SDTicaret.API/appsettings.json` dosyasında JWT secret key'ini güvenli bir değerle değiştirin:
```json
"Jwt": {
  "Secret": "YOUR_SUPER_SECRET_KEY_WITH_AT_LEAST_32_CHARACTERS_FOR_JWT_SIGNING"
}
```

### 4. Veritabanını Oluşturun
```bash
# API projesi dizininde
cd SDTicaret.API
dotnet ef database update
```

### 5. Projeyi Çalıştırın
```bash
# API projesini çalıştırın
cd SDTicaret.API
dotnet run

# Yeni terminal açın ve Web projesini çalıştırın
cd SDTicaret.Web
dotnet run
```

## Önemli Notlar

- **Migration dosyaları** otomatik olarak oluşturulacaktır
- **Log dosyaları** `Logs/` klasöründe tutulacaktır
- **Hassas bilgiler** (connection string, JWT secret) asla GitHub'a yüklenmemelidir
- Development ortamında debug logları aktif olacaktır

## Sorun Giderme

### Veritabanı Bağlantı Hatası
- SQL Server'ın çalıştığından emin olun
- Connection string'deki server adını kontrol edin
- Windows Authentication kullanıyorsanız gerekli izinlerin olduğundan emin olun

### Migration Hatası
- Veritabanının mevcut olduğundan emin olun
- `dotnet ef migrations add InitialCreate` komutunu çalıştırın
- Sonra `dotnet ef database update` komutunu çalıştırın 