# Search Dashboard - Frontend (Next.js)

Modern arama dashboard'u ile backend API'sine bağlanarak içerik arama, filtreleme ve yönetim işlemleri sunar.

## 📋 İçindekiler

- [Özellikler](#-özellikler)
- [Teknoloji Yığını](#-teknoloji-yığını)
- [Mimari](#-mimari)
- [Kurulum](#-kurulum)
- [Docker](#-docker)
- [Kullanım](#-kullanım)
- [Bileşenler](#-bileşenler)
- [API Entegrasyonu](#api-entegrasyonu)
- [Styling](#-styling)
- [Performans](#-performans)

## ✨ Özellikler

- 🔍 **Gelişmiş Arama**: Anahtar kelime ile içerik arama
- 🎯 **Filtreleme**: İçerik türüne göre filtreleme (Video/Metin)
- 📊 **Sıralama**: Final skor, popülerlik ve alakalilik sıralaması
- 📄 **Sayfalama**: Verimli veri gösterimi
- 🔄 **Real-time Sync**: Provider veri senkronizasyonu
- 📱 **Responsive Design**: Mobil uyumlu arayüz
- ⚡ **Optimized Performance**: Client-side caching ve lazy loading

## 🛠️ Teknoloji Yığını

- **Framework**: Next.js 14 (App Router)
- **Dil**: TypeScript
- **UI**: React 18
- **Styling**: Tailwind CSS
- **HTTP Client**: Axios/Fetch API
- **State Management**: React Hooks (Context API)
- **Icons**: Lucide React
- **Forms**: React Hook Form (opsiyonel)

## 🏗️ Mimari

```
├── app/                  # Next.js App Router
│   ├── api/             # API route'leri (opsiyonel)
│   ├── components/       # Reusable bileşenler
│   ├── hooks/           # Custom hooks
│   ├── lib/             # Utility fonksiyonlar
│   └── types/           # TypeScript tipleri
├── components/          # UI bileşenleri
│   ├── ui/             # Temel UI bileşenleri
│   ├── forms/          # Form bileşenleri
│   └── layout/        # Layout bileşenleri
├── lib/               # API client ve yardımcılar
├── public/            # Static assets
└── styles/            # Global stiller
```

### Ana Bileşenler

- **SearchForm**: Arama formu ve filtreler
- **ContentList**: İçerik listesi
- **Pagination**: Sayfalama kontrolü
- **SyncButton**: Provider senkronizasyonu
- **ContentCard**: İçerik kartı
- **FilterPanel**: Gelişmiş filtreleme paneli

## 🚀 Kurulum

### Gereksinimler
- Node.js 18+
- npm veya yarn

### Adım 1: Bağımlılıkları Yükle
```bash
npm install
# veya
yarn install
```

### Adım 2: Konfigürasyon
`.env.local` dosyası oluşturun:
```env
NEXT_PUBLIC_API_BASE=http://localhost:5018
NEXT_PUBLIC_APP_NAME=Search Dashboard
NEXT_PUBLIC_APP_VERSION=1.0.0
```

### Adım 3: Geliştirme Sunucusu
```bash
npm run dev
# veya
yarn dev
```

Uygulama `http://localhost:3000` adresinde çalışacaktır.

### Adım 4: Production Build
```bash
npm run build
npm start
```

## 🐳 Docker

### Geliştirme Ortamı
Proje kökünden:
```bash
docker compose up -d --build
```

### Sadece Frontend
```bash
docker build -t search-dashboard .
docker run -p 3000:3000 \
  -e NEXT_PUBLIC_API_BASE=http://localhost:5018 \
  search-dashboard
```

## 📖 Kullanım

### Arama İşlemi
1. Anahtar kelime girin
2. İçerik türü seçin (Video/Metin/Tümü)
3. Sıralama kriteri belirleyin
4. Arama butonuna tıklayın

### Filtreleme
- **Anahtar Kelime**: Başlıkta arama
- **İçerik Türü**: Video, Metin veya Tümü
- **Sıralama**: Final skor, Popülerlik, Alakalilik
- **Sayfa**: Sayfalama ile gezinme

### Provider Sync
- "Sync Data" butonu ile provider verilerini güncelleyin
- İşlem durumu ve sonuç sayısı gösterilir

## 🧩 Bileşenler

### SearchForm
Arama formu ve filtreleme seçenekleri:
```tsx
<SearchForm
  onSearch={handleSearch}
  loading={loading}
  initialValues={initialFilters}
/>
```

### ContentList
İçerik listesi ve kartlar:
```tsx
<ContentList
  items={contents}
  loading={loading}
  onItemClick={handleItemClick}
/>
```

### Pagination
Sayfalama kontrolü:
```tsx
<Pagination
  currentPage={page}
  totalPages={totalPages}
  onPageChange={handlePageChange}
/>
```

## 🔌 API Entegrasyonu

### API Client
```typescript
// lib/api.ts
export const searchContents = async (params: SearchParams) => {
  const response = await fetch(`${API_BASE}/api/contents?${new URLSearchParams(params)}`);
  return response.json();
};

export const syncContents = async () => {
  const response = await fetch(`${API_BASE}/api/contents/sync`, { method: 'POST' });
  return response.json();
};
```

### TypeScript Tipleri
```typescript
// types/index.ts
export interface ContentItem {
  id: string;
  title: string;
  type: 'Video' | 'Text';
  views: number;
  likes: number;
  reactions: number;
  readingTime: number;
  finalScore: number;
  popularityScore: number;
  relevanceScore: number;
  publishedAtUtc: string;
  provider: string;
}

export interface SearchParams {
  keyword?: string;
  type?: 'Video' | 'Text';
  sortBy?: 'final' | 'popularity' | 'relevance';
  page?: number;
  pageSize?: number;
}

export interface SearchResult {
  items: ContentItem[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
}
```

### Custom Hooks
```typescript
// hooks/useSearch.ts
export const useSearch = () => {
  const [loading, setLoading] = useState(false);
  const [data, setData] = useState<SearchResult | null>(null);
  const [error, setError] = useState<string | null>(null);

  const search = async (params: SearchParams) => {
    setLoading(true);
    setError(null);
    try {
      const result = await searchContents(params);
      setData(result);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return { search, loading, data, error };
};
```

## 🎨 Styling

### Tailwind CSS Konfigürasyonu
```javascript
// tailwind.config.js
module.exports = {
  content: ['./src/**/*.{js,ts,jsx,tsx}'],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#eff6ff',
          500: '#3b82f6',
          600: '#2563eb',
        },
      },
    },
  },
  plugins: [],
};
```

### Temel Bileşen Stilleri
```tsx
// components/ui/Button.tsx
const Button = ({ children, variant = 'primary', ...props }) => {
  const baseClasses = 'px-4 py-2 rounded-md font-medium transition-colors';
  const variants = {
    primary: 'bg-blue-600 text-white hover:bg-blue-700',
    secondary: 'bg-gray-200 text-gray-900 hover:bg-gray-300',
  };

  return (
    <button className={`${baseClasses} ${variants[variant]}`} {...props}>
      {children}
    </button>
  );
};
```

## ⚡ Performans

### Optimizasyon Stratejileri
- **Code Splitting**: Next.js otomatik code splitting
- **Image Optimization**: Next.js Image component
- **Lazy Loading**: Büyük listeler için lazy loading
- **Debouncing**: Arama input'u için debouncing
- **Memoization**: React.memo ve useMemo kullanımı

### Client-side Caching
```typescript
// lib/cache.ts
const cache = new Map<string, { data: any; timestamp: number }>();

export const getCachedData = (key: string, ttl = 300000) => {
  const item = cache.get(key);
  if (item && Date.now() - item.timestamp < ttl) {
    return item.data;
  }
  return null;
};

export const setCachedData = (key: string, data: any) => {
  cache.set(key, { data, timestamp: Date.now() });
};
```

## 🔗 İlgili Projeler

- **Backend**: [../backend/README.md](../backend/README.md) - .NET Web API
- **Provider Mock**: [WEG-Technology/mock](https://github.com/WEG-Technology/mock) - Test verileri

## 🚀 Geliştirme İpuçları

### Hot Reload
- Dosya değişiklikleri otomatik yeniden yüklenir
- CSS değişiklikleri anında görünür

### Debugging
- React DevTools kullanımı
- Network tab ile API isteklerini izleme
- Console.log yerine debugger kullanımı

### Best Practices
- TypeScript strict mode kullanımı
- Component prop'ları için interface'ler
- Error boundary kullanımı
- Loading ve error state'leri

## 📝 Lisans

Bu proje MIT lisansı altında dağıtılmaktadır.
