# Search Engine Service - Backend (.NET)

Bu servis farklı provider'lardan (JSON/XML) gelen içerikleri tek formatta toplar, puanlar, veritabanına kaydeder ve arama API'si sunar.

## 📋 İçindekiler

- [Özellikler](#-özellikler)
- [Teknoloji Yığını](#-teknoloji-yığını)
- [Mimari](#-mimari)
- [Provider Entegrasyonu](#-provider-entegrasyonu)
- [Skorlama Formülü](#-skorlama-formülü)
- [Kurulum](#-kurulum)
- [Docker](#-docker)
- [API Dokümantasyonu](#-api-dokümantasyonu)
- [Yeni Provider Ekleme](#-yeni-provider-ekleme)
- [Test](#-test)
- [Performans](#-performans)

## ✨ Özellikler

- 🔌 **Çoklu Provider Desteği**: JSON, XML ve diğer formatlardan veri çekme
- 📊 **Akıllı Skorlama**: İçerik kalitesini otomatik puanlama
- 🗄️ **Veritabanı Yönetimi**: PostgreSQL ile veri saklama ve indeksleme
- 🚀 **Rate Limiting**: Provider isteklerini sınırlandırma
- 💾 **Cache**: Hızlı sorgular için bellek içi önbellek
- 🔍 **Gelişmiş Arama**: Filtreleme, sıralama ve sayfalama
- 📈 **Repository Pattern**: Temiz veri erişim katmanı

## 🛠️ Teknoloji Yığını

- **Backend**: ASP.NET Core Web API (.NET 9)
- **ORM**: Entity Framework Core
- **Veritabanı**: PostgreSQL (Npgsql)
- **Dokümantasyon**: OpenAPI (`/openapi/v1.json`)
- **Cache**: In-memory cache (IMemoryCache)
- **DI**: Built-in Dependency Injection
- **Logging**: Serilog (opsiyonel)

## 🏗️ Mimari

```
├── Controllers/           # API endpoint'leri
├── Services/            # İş mantığı
│   ├── ContentIngestionService
│   ├── ContentSearchService
│   └── ScoreCalculator
├── Providers/           # Provider entegrasyonu
│   ├── Behavior/        # Rate limiting handler'lar
│   └── Dto/           # Provider DTO'ları
├── Repositories/        # Veri erişim katmanı
├── Models/             # Domain modelleri
├── Data/              # DbContext ve konfigürasyon
├── Constants/         # Magic string'ler
├── Options/           # Konfigürasyon sınıfları
└── Middleware/        # Custom middleware'ler
```

### Provider Entegrasyonu

**Mevcut Provider'lar:**
- **JsonProviderClient**: JSON formatında veri çeker
- **XmlProviderClient**: XML formatında veri çeker

**Rate Limiting:**
- Her provider için ayrı istek limiti
- `ProviderRateLimitDelegatingHandler` ile implementasyon

**Veri Akışı:**
1. Provider'dan veri çekme
2. `ProviderContentDto`'ya dönüştürme
3. Skor hesaplama
4. Veritabanına kaydetme (Repository Pattern)
5. Cache'e alma

## 📊 Skorlama Formülü

`Final Skor = (Temel Puan * İçerik Türü Katsayısı) + Güncellik Puanı + Etkileşim Puanı`

### Temel Puan Hesaplama
- **Video**: `views/1000 + likes/100`
- **Metin**: `reading_time + reactions/50`

### Tür Katsayıları
- **Video**: `1.5`
- **Metin**: `1.0`

### Güncellik Puanı
- **1 hafta**: `+5`
- **1 ay**: `+3`
- **3 ay**: `+1`
- **Daha eski**: `+0`

### Etkileşim Puanı
- **Video**: `(likes/views) * 10`
- **Metin**: `(reactions/reading_time) * 5`

## 🚀 Kurulum

### Gereksinimler
- .NET 9 SDK
- PostgreSQL 13+
- (Opsiyonel) Docker & Docker Compose

### Adım 1: Veritabanı
PostgreSQL'i başlatın ve veritabanı oluşturun:
```sql
CREATE DATABASE searchdb;
```

### Adım 2: Konfigürasyon
`appsettings.json` dosyasını düzenleyin:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=searchdb;Username=postgres;Password=postgres"
  },
  "Providers": {
    "JsonUrl": "https://raw.githubusercontent.com/WEG-Technology/mock/refs/heads/main/v2/provider1",
    "XmlUrl": "https://raw.githubusercontent.com/WEG-Technology/mock/refs/heads/main/v2/provider2"
  },
  "Scoring": {
    "VideoTypeFactor": 1.5,
    "TextTypeFactor": 1.0
  }
}
```

### Adım 3: Çalıştırma
```bash
dotnet restore
dotnet run
```

Uygulama açılışında `EnsureCreated()` ile tablolar otomatik oluşturulur.

## 🐳 Docker

### Geliştirme Ortamı
Proje kökünden:
```bash
docker compose up -d --build
```

Servisler:
- **Frontend**: `http://localhost:3000`
- **Backend**: `http://localhost:5018`
- **PostgreSQL**: `localhost:5432`

### Sadece Backend
```bash
docker build -t search-engine-backend .
docker run -p 5018:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=searchdb;Username=postgres;Password=postgres" \
  search-engine-backend
```

## 📚 API Dokümantasyonu

### OpenAPI
- Swagger UI: `http://localhost:5018/swagger`
- OpenAPI JSON: `http://localhost:5018/openapi/v1.json`

### Endpoint'ler

#### Provider Sync
```http
POST /api/contents/sync
```
Provider verilerini çeker ve veritabanına yazar/günceller.

**Response:**
```json
{
  "message": "Sync tamamlandi",
  "imported": 42
}
```

#### Arama
```http
GET /api/contents?keyword=search&type=Video&sortBy=popularity&page=1&pageSize=10
```

**Parametreler:**
- `keyword` (optional): Başlıkta anahtar kelime arama
- `type` (optional): `Video` veya `Text`
- `sortBy` (optional): `final`, `popularity`, `relevance`
- `page` (optional): Sayfa numarası (default: 1)
- `pageSize` (optional): Sayfa başına sonuç (default: 10, max: 100)

**Response:**
```json
{
  "items": [...],
  "page": 1,
  "pageSize": 10,
  "total": 42,
  "totalPages": 5
}
```

## ➕ Yeni Provider Ekleme

### 1. Provider Client Oluştur
```csharp
public class NewProviderClient(HttpClient httpClient) : IProviderClient
{
    public string Name => ProviderConstants.NewProvider;
    
    public async Task<IReadOnlyList<ProviderContentDto>> FetchAsync(CancellationToken cancellationToken)
    {
        // Provider'a özel veri çekme mantığı
    }
}
```

### 2. DTO'ları Oluştur (Gerekirse)
`Providers/Dto/` altında özel DTO'lar oluşturun.

### 3. Constants'a Ekle
```csharp
// ProviderConstants.cs
public const string NewProvider = "provider-new";
```

### 4. Program.cs'de Kaydet
```csharp
builder.Services.AddHttpClient<NewProviderClient>(client =>
{
    client.BaseAddress = new Uri(newProviderUrl);
}).AddHttpMessageHandler<NewProviderRateLimitHandler>();

builder.Services.AddScoped<IProviderClient>(sp => 
    sp.GetRequiredService<NewProviderClient>());
```

### 5. Konfigürasyon
`appsettings.json` ve `ProviderOptions.cs`'ye gerekli ayarları ekleyin.

## 🧪 Test

### Unit Test
```bash
dotnet test
```

### Test Stratejisi
- **Unit Test**: `ScoreCalculator` formüllerinin doğrulanması
- **Integration Test**: `/sync` + `/contents` akışının test edilmesi
- **Contract Test**: Provider JSON/XML parsing doğrulamaları

### Örnek Test Senaryoları
- Video içerik skorlama formülü testi
- Provider veri parsing testi
- Cache mekanizması testi
- Rate limiting testi

## ⚡ Performans

### Veritabanı Optimizasyonları
- **Index'ler**: Provider+ExternalId (unique), Title, Type, PublishedAtUtc, FinalScore
- **Partitioning**: Tarihe göre (opsiyonel)
- **Connection Pooling**: Default EF Core ayarları

### Cache Stratejisi
- **Query Cache**: Arama sonuçları için (5 dakika)
- **Provider Cache**: Provider verileri için (15 dakika)
- **Distributed Cache**: Redis ile ölçeklenebilirlik

### Rate Limiting
- Her provider için ayrı limit
- Konfigürasyon ile ayarlanabilir
- `SemaphoreSlim` ile implementasyon

## 🔗 İlgili Projeler

- **Frontend**: [../frontend/README.md](../frontend/README.md) - Next.js arama dashboard'u
- **Provider Mock**: [WEG-Technology/mock](https://github.com/WEG-Technology/mock) - Test verileri

## 📝 Lisans

Bu proje MIT lisansı altında dağıtılmaktadır.
