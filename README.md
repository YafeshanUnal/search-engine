# Search Engine Platform

Modern bir arama motoru platformu içerik sağlayıcıları entegrasyonu ve gelişmiş arama özellikleri ile.

## 🏗️ Mimari

### Genel Bakış
Platform, mikro servis mimarisi yaklaşımı ile tasarlanmıştır:
- **Backend**: ASP.NET Core Web API
- **Frontend**: Next.js Dashboard
- **Database**: PostgreSQL
- **Caching**: In-Memory Cache (IMemoryCache)
- **Containerization**: Docker & Docker Compose

### Katmanlar

```
┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │   Backend API   │
│   (Next.js)     │◄──►│  (ASP.NET Core) │
└─────────────────┘    └─────────────────┘
                              │
                       ┌─────────────────┐
                       │   PostgreSQL    │
                       │   Database      │
                       └─────────────────┘
```

### Backend Mimarisi

#### Controllers
- **ContentsController**: Arama ve senkronizasyon endpoint'leri
- **CacheController**: Cache yönetimi ve debugging

#### Services
- **ContentSearchService**: Arama işlemleri ve cache stratejisi
- **ContentIngestionService**: İçerik senkronizasyonu
- **ScoreCalculator**: İçerik skorlama algoritması
- **StartupService**: Başlangıç görevleri

#### Providers
- **JsonProviderClient**: JSON formatında içerik sağlayıcısı
- **XmlProviderClient**: XML formatında içerik sağlayıcısı

#### Repository Pattern
- **ContentRepository**: Veritabanı operasyonları
- **Generic Repository**: Tekrar kullanılabilir veri erişimi

### Caching Stratejisi

#### Cache Katmanları
1. **Provider Data Cache**: Sağlayıcı verileri (10 dakika)
2. **All Contents Cache**: Tüm içerikler (15 dakika)  
3. **Search Results Cache**: Arama sonuçları (5 dakika)

#### Cache Key Formatları
- `contents:all` - Tüm içerikler
- `provider:data:{provider}` - Sağlayıcı verileri
- `search:{keyword}:{type}:{sort}:{page}:{pageSize}` - Arama sonuçları

### Skorlama Algoritması

#### Final Score Hesaplama
```
Final Score = (Base Score × Type Factor) + Recency Score + Interaction Score
```

#### Bileşenler
- **Base Score**: Görüntüleme ve beğeni temel skor
- **Type Factor**: Video/Text tipine göre ağırlık
- **Recency Score**: Yayın tarihine göre tazelik skoru
- **Interaction Score**: Etkileşim oranı (likes/views)

## 🚀 Kurulum

### Gereksinimler
- .NET 9.0 SDK
- Node.js 18+ 
- PostgreSQL 14+
- Docker (isteğe bağlı)

### Backend Kurulumu

1. **Repository klonlayın**
```bash
git clone https://github.com/YafeshanUnal/search-engine.git
cd search-engine
```

2. **Backend proje dizinine gidin**
```bash
cd backend/SearchEngineService
```

3. **NuGet paketlerini yükleyin**
```bash
dotnet restore
```

4. **Veritabanı bağlantısını yapılandırın**
`appsettings.json` dosyasında connection string'i güncelleyin:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=SearchEngine;Username=postgres;Password=your_password"
  }
}
```

5. **Veritabanını oluşturun**
```bash
dotnet run --project SearchEngineService
```
(Uygulama ilk çalıştığında veritabanı otomatik oluşturulur)

### Frontend Kurulumu

1. **Frontend proje dizinine gidin**
```bash
cd frontend/search-dashboard
```

2. **Node paketlerini yükleyin**
```bash
npm install
```

3. **Geliştirme sunucusunu başlatın**
```bash
npm run dev
```

### Docker ile Kurulum

1. **Docker Compose ile tüm servisleri başlatın**
```bash
docker-compose up -d
```

2. **Servislerin durumunu kontrol edin**
```bash
docker-compose ps
```

## 📝 Yapılandırma

### Backend Ayarları

#### Provider URLs
`appsettings.json` dosyasında sağlayıcı URL'lerini yapılandırın:
```json
{
  "JsonUrl": "https://your-json-provider.com/api/content",
  "XmlUrl": "https://your-xml-provider.com/api/content"
}
```

#### Skorlama Parametreleri
```json
{
  "Scoring": {
    "VideoTypeFactor": 1.2,
    "TextTypeFactor": 1.0,
    "VideoBaseViewsDivisor": 1000,
    "VideoBaseLikesDivisor": 100,
    "TextBaseReactionsDivisor": 10,
    "TextPopularityReadingTimeWeight": 0.5
  }
}
```

### Frontend Ayarları

#### API URL
`next.config.ts` dosyasında backend URL'ini yapılandırın:
```typescript
const nextConfig = {
  env: {
    NEXT_PUBLIC_API_URL: 'http://localhost:5000'
  }
}
```

## 🔧 Kullanım

### API Endpoint'leri

#### Arama
```
GET /api/contents?keyword=aranacak_kelime&type=Video&sortBy=final&page=1&pageSize=10
```

#### Senkronizasyon
```
POST /api/contents/sync
```

#### Cache Bilgileri
```
GET /api/cache
DELETE /api/cache
```

### Swagger Documentation
Development ortamında Swagger UI erişilebilir:
```
http://localhost:5000/swagger
```

## 🧪 Test

### Backend Testleri
```bash
cd backend/SearchEngineService.Tests
dotnet test
```

### Frontend Testleri
```bash
cd frontend/search-dashboard
npm test
```

## 📊 Performans Optimizasyonları

### Caching
- Multi-level caching stratejisi
- Otomatik cache invalidation
- Memory-efficient cache keys

### Database
- Optimized index'ler
- Connection pooling
- Query optimization

### API
- Response compression
- Rate limiting
- Async/await pattern'ler

## 🔒 Güvenlik

### Authentication (Planlanan)
- JWT token authentication
- API key management
- Rate limiting per user

### Data Validation
- Input validation
- SQL injection prevention
- XSS protection

## 🚀 Deployment

### Production Deployment

#### Backend
```bash
dotnet publish -c Release -o ./publish
```

#### Frontend
```bash
npm run build
```

#### Docker Production
```bash
docker-compose -f docker-compose.prod.yml up -d
```

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. [LICENSE](LICENSE) dosyasına bakın.

## 📞 İletişim

- **Repository**: https://github.com/YafeshanUnal/search-engine
- **Issues**: https://github.com/YafeshanUnal/search-engine/issues

## 🔮 Gelecek Planları

### V1.1
- [ ] Full-text search integration
- [ ] Advanced filtering options
- [ ] Real-time updates

### V1.2
- [ ] Multi-language support
- [ ] Analytics dashboard
- [ ] Export functionality

### V2.0
- [ ] Machine learning recommendations
- [ ] Distributed caching
- [ ] Microservices architecture

---

**Not**: Bu proje modern web geliştirme best practice'lerini takip ederek geliştirilmiştir. Herhangi bir soru veya öneri için issues bölümünü kullanabilirsiniz.
