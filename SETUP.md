# SD Ticaret Projesi Kurulum Rehberi

## Gereksinimler
- .NET 9.0 SDK
- SQL Server (Express, LocalDB veya Full)
- Visual Studio 2022 veya VS Code
- Docker (opsiyonel)

## Kurulum Adımları

### 1. Projeyi Klonlayın
```bash
git clone <repository-url>
cd SD_Ticaret
```

### 2. Veritabanı Bağlantısını Yapılandırın

#### API Projesi için:
1. `SDTicaret.API/appsettings.example.json` dosyasını `appsettings.json` olarak kopyalayın
2. Connection string'i güncelleyin:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
   }
   ```

#### Web Projesi için:
1. `SDTicaret.Web/appsettings.example.json` dosyasını `appsettings.json` olarak kopyalayın
2. API URL'ini güncelleyin:
   ```json
   "ApiSettings": {
     "BaseUrl": "http://localhost:7244/api/"
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
cd SDTicaret.API
dotnet ef database update
```

### 5. Projeyi Çalıştırın

#### Manuel Çalıştırma:
```bash
# API projesini çalıştırın
cd SDTicaret.API
dotnet run

# Yeni terminal açın ve Web projesini çalıştırın
cd ../SDTicaret.Web
dotnet run
```

#### Docker ile Çalıştırma:
```bash
docker-compose up -d
```

## Erişim URL'leri

- **Web Uygulaması**: `https://localhost:5244`
- **API**: `https://localhost:7244`
- **Swagger UI**: `https://localhost:7244/swagger`

## Önemli Notlar

- **Migration dosyaları** Git tarafından ignore edilir
- **Log dosyaları** `Logs/` klasöründe tutulacaktır
- **Hassas bilgiler** (connection string, JWT secret) asla GitHub'a yüklenmemelidir

## Sorun Giderme

### Veritabanı Bağlantı Hatası
- SQL Server'ın çalıştığından emin olun
- Connection string'deki server adını kontrol edin

### Migration Hatası
- Veritabanının mevcut olduğundan emin olun
- `dotnet ef migrations add InitialCreate` komutunu çalıştırın
- Sonra `dotnet ef database update` komutunu çalıştırın

### Docker Sorunları
- Docker Desktop'ın çalıştığından emin olun
- Port çakışması varsa `docker-compose.yml` dosyasındaki portları değiştirin 