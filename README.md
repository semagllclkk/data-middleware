# CENG302 — Data Middleware Ödevi

> **Borsa kuruluşu için Log Ara Katmanı (Middleware)**  
> İki Docker konteynerinden oluşan bir sistem: biri log üretir, diğeri işler.

---

## Projeyi Docker ile Çalıştırma

```bash
# 1. Proje kök dizinine gir
cd MiddlewareProject

# 2. İki konteyneri birlikte ayağa kaldır
docker-compose up --build

# Durdurmak için
docker-compose down
```

> **Not:** `docker-compose up` komutu önce `consumer-app`'i başlatır, Producer bağlanmaya hazır olana kadar 2 saniye bekler. Çalışmaya başlayınca konsol çıktısında log akışını görebilirsiniz.

---

## Proje Mimarisi

```
MiddlewareProject/
├── Producer/          → Konteyner 1: Sahte borsa logları üretir ve TCP ile gönderir
├── Consumer/          → Konteyner 2: Middleware — logları işler
│   ├── Pipeline/      → İşlem zinciri (Chain of Responsibility)
│   │   ├── LogHandler.cs           # Zincirin soyut taban sınıfı
│   │   ├── KvkkFilterHandler.cs    # Adım 1: Hassas veri maskeleme
│   │   ├── EnrichmentHandler.cs    # Adım 2: Log zenginleştirme
│   │   ├── FanOutHandler.cs        # Adım 3: Üç role dağıtım
│   │   ├── FormatterHandler.cs     # Adım 4: Biçimlendirme (Strategy)
│   │   └── PerformanceHandler.cs   # Adım 5: Performans ölçümü
│   └── Formatters/    → Biçimlendirme stratejileri (Strategy Pattern)
│       ├── IFormatterStrategy.cs   # Ortak arayüz
│       ├── HtmlFormatter.cs        # System Admin → HTML
│       ├── CsvFormatter.cs         # CyberSec    → CSV
│       └── JsonFormatter.cs        # Web Dev     → JSON
└── Shared/            → İki konteyner arasında paylaşılan modeller
    ├── Models/        → BaseLog, TransactionLog, ErrorLog, SecurityLog
    └── Factories/     → LogFactory (Factory Pattern)
```

---

## Her Bileşen Ne İş Yapar?

### 1. Producer (Veri Üretici)
- Her 500ms'de bir rastgele log üretir: `TRANSACTION`, `ERROR`, `SECURITY`
- `LogFactory` ile log nesnesi oluşturur, TCKN / e-posta / kredi kartı gibi hassas alanları doldurur
- Nesneyi JSON'a çevirip **TCP soketi** üzerinden Consumer'a gönderir

### 2. Consumer — İşlem Zinciri (Middleware)

Her gelen log şu adımlardan sırayla geçer:

| Adım | Sınıf | Görev |
|------|-------|-------|
| 1 | `KvkkFilterHandler` | TCKN → `***********`, e-posta → `***@***.***`, kredi kartı → `****-****-****-****` |
| 2 | `EnrichmentHandler` | `sender_id`, `transaction_no`, `severity`, `message`, `[DEBUG]` etiketleri ekler |
| 3 | `FanOutHandler` | Zenginleştirilmiş logu 3 role paralel dağıtır |
| 4 | `FormatterHandler` | Role göre HTML / CSV / JSON formatına dönüştürür |
| 5 | `PerformanceHandler` | İşlem süresini ölçer; %10 rastgele drop ile stres testi simüle eder |

---

## Kullanılan Tasarım Kalıpları

### 1. Chain of Responsibility (Sorumluluk Zinciri)

`LogHandler` soyut sınıfı, `SetNext()` metodu ile halkalar birbirine bağlanır.  
Her halka kendi işini yapar, sonra veriyi bir sonrakine iletir — veya zinciri kırar (drop).

```
filter → enrichment → fanOut → [htmlFormatter | csvFormatter | jsonFormatter] → performance
```

**Neden?** Middleware adımları birbirinden bağımsız olmalı. Yeni bir adım (örn. şifreleme) eklemek için sadece yeni bir `LogHandler` yazıp zincire takman yeterli.

### 2. Strategy (Strateji)

`IFormatterStrategy` arayüzü üç somut strateji ile uygulanmıştır:

| Strateji | Hedef Rol | Format |
|----------|-----------|--------|
| `HtmlFormatter` | System Admin | `<div class='log-item'>...</div>` |
| `CsvFormatter` | CyberSec | `[CSV FORMATI] key:value, ...` |
| `JsonFormatter` | Web Dev | `[JSON FORMATI] {...}` |

`FormatterHandler.SetStrategy()` ile çalışma zamanında format değiştirilebilir.

**Neden?** Her rol farklı format ister; `if/else` yerine strateji ile her format ayrı sınıfa taşındı, açık/kapalı prensibine uygun.

### 3. Factory (Fabrika) — Bonus

`LogFactory.CreateLog("TRANSACTION" | "ERROR" | "SECURITY")` ile Producer, nesne türünü bilmeden log yaratır.

---

## Performans Ölçümü

`PerformanceHandler` her log için işlem süresini ölçer ve konsola yazar.  
Stres testi için Producer'daki `Task.Delay(500)` kaldırılarak maksimum hızda veri gönderilebilir:

```csharp
// Producer/Program.cs — stres testi için bu satırı yorum satırı yap:
// await Task.Delay(500);
```

`docker-compose up` çıktısında `[4. ADIM - PERFORMANS]` satırlarında gecikme değerleri görünür.

---

## Ödev Gereksinimleri Karşılama Tablosu

| Gereksinim | Durum | Nerede |
|------------|-------|--------|
| İki Docker konteyneri | ✅ | `docker-compose.yml` |
| Güvenlik — hassas veri maskeleme | ✅ | `KvkkFilterHandler.cs` |
| Zenginleştirme — etiket ekleme | ✅ | `EnrichmentHandler.cs` |
| Filtreleme (info/warning drop) | ✅ | `SeverityFilterHandler.cs` |
| Biçim özelleştirme (3 rol) | ✅ | `FanOutHandler` + Formatters |
| En az 1 tasarım kalıbı | ✅ | Chain of Responsibility + Strategy + Factory |
| Performans ölçümü | ✅ | `PerformanceHandler.cs` |
| Tüm senaryolar üretiliyor | ✅ | `Producer/Program.cs` — TRANSACTION/ERROR/SECURITY |

---

## Video Anlatım İçin Öneri Akışı (15 dk)

1. **(2 dk)** Proje yapısını IDE'de göster, hangi klasör ne işe yarıyor
2. **(2 dk)** `docker-compose up --build` çalıştır, konsol çıktısını göster
3. **(4 dk)** Canlı çıktıda şu adımları göster:
   - KVKK maskeleme (TCKN/email/cc yıldızlandı)
   - Enrichment etiketleri (sender_id, transaction_no, severity)
   - HTML / CSV / JSON çıktılarını yan yana göster
4. **(4 dk)** Tasarım kalıplarını kodda göster:
   - `LogHandler.cs` → `SetNext()` zinciri
   - `IFormatterStrategy.cs` → `SetStrategy()` kullanımı
5. **(3 dk)** Stres testi: `Task.Delay` kaldır, yeniden build et, throughput/drop metriklerini göster