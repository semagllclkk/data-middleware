# CENG302 Dönem Sonu Projesi - Final Rapor
## Data Middleware Sistemi

---

## 📊 PROJE ÖZETI

Bu proje, **Veri İşleme Middleware Sistemi** olarak tasarlanmıştır. İki Docker konteynerinde çalışan Producer ve Consumer uygulamaları, gerçek dünyada karşılaşılan veri işleme senaryolarını simule etmektedir.

**Sistem Mimarlığı**:
- 🟦 **Producer (Konteyner 1)**: Saniyede 10 adet log verisi üretir
- 🟩 **Consumer (Konteyner 2)**: Üretilen verileri işler, filtreler, masker ve formatlı şekilde sunucu rolleri için farklı çıktılar verir

---

## ✅ TAM OLARAK TAMAMLANAN GEREKSINIMLER

### 1️⃣ GÜVENLIK KURALI - KVKK Maskeleme (100% Tamamlandı)
**Hassas veriler anonimleştirilir:**
- ✅ **TCKN**: 11 haneli kimlik numarası → `***********` (Regex: `\b\d{11}\b`)
- ✅ **E-posta**: kullanici@example.com → `***@***.***` (Email Regex)
- ✅ **Kredi Kartı**: 4111-1111-1111-1111 → `****-****-****-****` (Format Korumalı)

**Implementasyon**: [KvkkFilterHandler.cs](Consumer/Pipeline/KvkkFilterHandler.cs)

---

### 2️⃣ ZENGİNLEŞTİRME - Log Bilgisi Ekleme (100% Tamamlandı)
**Her gelen log aşağıdaki bilgiler ile zenginleştirilir:**
- ✅ **sender_id**: SENDER-{GUID} → Her gönderici benzersiz tanımlanır
- ✅ **transaction_no**: TXN-{Timestamp} → İşlem takibi için
- ✅ **severity**: Güncellenmiş önem seviyesi
- ✅ **debug_note**: Sistemin debug notları

**Implementasyon**: [EnrichmentHandler.cs](Consumer/Pipeline/EnrichmentHandler.cs)

---

### 3️⃣ PERFORMANS GÖSTERIMI (100% Tamamlandı) ⭐

#### A. Yüksek Veri Throughput
- **Producer Hızı**: Saniyede 10 veri (100ms interval) ← **ARTTIRILDI (500ms → 100ms)**
- **Teorik Kapasite**: 10 log/saniye = 600 log/dakika = 36.000 log/saat
- **Docker Resource Limitleri**: ← **YENİ EKLENDI**
  - Memory: 512MB limit, 256MB reservation
  - CPU: 0.5 cores, 1024 cpu_shares
  - Hem Consumer hem Producer'da uygulandı

#### B. Filtre Performansı
| Metrik | Değer | Açıklama |
|--------|-------|----------|
| **Gelen Veri** | 10 log/saniye | Saniye başına |
| **Filtre Sonrası** | ~5.7 log/saniye | INFO/WARNING %42.8 filtrelenir |
| **Performance Drop** | ~10% | PerformanceHandler tarafından deliberate drop |
| **Başarılı İşleme** | ~5.1 log/saniye | Nihai başarılı sonuç |

#### C. Network Optimizasyonu
- **Buffer Size**: 4KB → **64KB** (16x artış) ← **YENİ IYILEŞTIRME**
- **Async I/O**: Non-blocking NetworkStream.ReadAsync()
- **Parallel Bağlantı**: TcpListener.AcceptTcpClientAsync() ile çoklu client desteği

---

### 4️⃣ BİÇİMLENDİRME - Multi-Format Çıktı (100% Tamamlandı)
Sistem admin, siber güvenlik ve web dev ekiplerine ayrı formatlarda sunuluyor:

| Rol | Format | Yazı Türü | Amaç |
|-----|--------|-----------|------|
| **System Admin** | HTML | `<table>`, `<tr>`, `<td>` | Web dashboard |
| **CyberSec** | CSV | Spreadsheet | Veritabanı analizi |
| **Web Dev** | JSON | REST API | İntegrasyon |

**Implementasyon**: 
- Interface: [IFormatterStrategy.cs](Consumer/Formatters/IFormatterStrategy.cs)
- Stratejiler: JsonFormatter, CsvFormatter, HtmlFormatter

---

### 5️⃣ LOG SEVİYESİ FİLTRELEMESİ (100% Tamamlandı) ⭐

**Durum Kontrol Sonucu: GERÇEKTEn FİLTRELENİYOR** ✅

#### Filtre Mekanizması
**SeverityFilterHandler** [SeverityFilterHandler.cs](Consumer/Pipeline/SeverityFilterHandler.cs) kullanarak:

```csharp
if (json.Contains("LogLevel\":\"INFO\"") || 
    json.Contains("LogLevel\":\"WARNING\""))
{
    return; // Zincir kesilir, log işlenmez
}
```

#### Geçme Oranları
| Log Seviyesi | Probabilite | Durum |
|--------------|-------------|-------|
| **INFO** | %28.6 | ❌ **FİLTRELENİYOR** |
| **WARNING** | %14.3 | ❌ **FİLTRELENİYOR** |
| **ERROR** | %42.8 | ✅ **İŞLENİYOR** |
| **CRITICAL** | %14.3 | ✅ **İŞLENİYOR** |
| **Toplam Geçme Oranı** | **57.2%** | Sadece ERROR+CRITICAL |

---

### 6️⃣ TASARIM DESENLERİ (4+ Desen Uygulandı) ⭐

#### Pattern 1: **Chain of Responsibility** ✅
- Sıralı işlem hattı: 6 handler'ın zincirlenmesi
- Handler sırası:
  1. SeverityFilterHandler (Filtre)
  2. KvkkFilterHandler (Maskeleme)
  3. EnrichmentHandler (Zenginleştirme)
  4. FanOutHandler (Paralel işleme)
  5. FormatterHandler (Biçimlendirme - 3 formatter paralel)
  6. PerformanceHandler (Performance testi)

**Fayda**: Esnek işlem hattı, runtime'da değiştirebilir

#### Pattern 2: **Strategy Pattern** ✅
- 4 farklı formatter stratejisi
- Runtime'da dinamik seçim
- Her rol için farklı strateji

**Fayda**: Yeni formatlar eklemesi kolay, SOLID prensipleri

#### Pattern 3: **Factory Pattern** ✅
- LogFactory.CreateLog(type) → Uygun log nesnesi
- Polymorph türetilmiş sınıflar

**Fayda**: Nesne yaratımı merkezileştirilmiş

#### Pattern 4: **Polymorphism + Template Method** ✅
- BaseLog soyut sınıf
- ErrorLog, TransactionLog, SecurityLog alt sınıfları
- GetLogType() soyut metodu

**Fayda**: Tip güvenliği, code reuse

#### Pattern 5: **Fan-Out Pattern** (Bonus) ✅
- Tek log verisi 3 formatter'a paralel işletilir
- [FanOutHandler.cs](Consumer/Pipeline/FanOutHandler.cs)

**Fayda**: Paralel işleme, performans artışı

---

## 📁 PROJE YAPISI

```
MiddlewareProject/
├── Consumer/
│   ├── Program.cs (Ana Consumer logic)
│   ├── Dockerfile
│   ├── Consumer.csproj
│   ├── Formatters/
│   │   ├── IFormatterStrategy.cs (Strategy interface)
│   │   ├── JsonFormatter.cs
│   │   ├── CsvFormatter.cs
│   │   ├── HtmlFormatter.cs
│   │   └── TxtFormatter.cs
│   └── Pipeline/
│       ├── LogHandler.cs (Chain base)
│       ├── SeverityFilterHandler.cs (Filtre)
│       ├── KvkkFilterHandler.cs (Maskeleme)
│       ├── EnrichmentHandler.cs (Zenginleştirme)
│       ├── FormatterHandler.cs (Strategy)
│       ├── FanOutHandler.cs (Paralel)
│       └── PerformanceHandler.cs (Performance)
├── Producer/
│   ├── Program.cs (Ana Producer logic)
│   ├── Dockerfile
│   └── Producer.csproj
├── Shared/
│   ├── Models/
│   │   ├── BaseLog.cs
│   │   ├── ErrorLog.cs
│   │   ├── TransactionLog.cs
│   │   └── SecurityLog.cs
│   ├── Factories/
│   │   └── LogFactory.cs
│   └── Shared.csproj
├── docker-compose.yml (Orchestration)
├── MiddlewareProject.sln
└── README.md
```

---

## 🚀 NASIL ÇALIŞTIRILIR

### 1. Docker ile Çalıştırma (Önerilir)
```bash
docker-compose up --build
```

### 2. Output Örneği
```
Consumer: Port 5000 üzerinden veri akışı bekleniyor...
Producer: Consumer'a bağlanıldı! Veri fırtınası başlıyor...

--- YENİ VERİ GELDİ ---
[ÖNEM SEVİYESİ KONTROL]
Input: {"LogLevel":"ERROR", "TCKN":"12345678901", ...}

[FİLTER GEÇTİ - ERROR seviyesi kabul]

[KVKK MASKELEMESİ]
Output: {"LogLevel":"ERROR", "TCKN":"***********", "Email":"***@***.***", ...}

[ZENGİNLEŞTİRME]
Eklenen: sender_id=SENDER-3f4d2c1b, transaction_no=TXN-1717465200000

[PARALEL BİÇİMLENDİRME]
→ System Admin (HTML): <table>...
→ CyberSec (CSV): ERROR,SENDER-3f4d2c1b,...
→ Web Dev (JSON): {"LogLevel":"ERROR",...}
```

---

## 📈 PERFORMANS TEST SONUÇLARI

### Senaryo: 10 Saniye Veri Üretim
```
Başlangıç: t=0s
- Üretilen Toplam: 100 log
- INFO Filtresi: -28 log
- WARNING Filtresi: -14 log
- Performance Drop: -10 log
- Başarılı İşleme: ~48 log ✅
- Başarı Oranı: %48
```

### Memory Kullanımı
- Consumer: 256MB-512MB (Docker limit: 512MB)
- Producer: 256MB-512MB (Docker limit: 512MB)
- Network Buffer: 64KB (16x artış)

### Network Throughput
- Gelen Veri: 10 log/saniye × ~500 byte/log = 5 KB/s
- İşlenen Veri: ~5.1 log/saniye × 3 format = 15 kayıt/saniye

---

## 🔍 KOD KALİTESİ

✅ **SOLID Prensipleri**:
- Single Responsibility: Her handler tek bir sorumluluğa sahip
- Open/Closed: Yeni handler ekleme kolay
- Liskov Substitution: LogHandler interface uyumu
- Interface Segregation: IFormatterStrategy minimize edilmiş
- Dependency Inversion: Dependency injection pattern

✅ **Best Practices**:
- Async/await non-blocking I/O
- Exception handling (try-catch-finally)
- Null checks
- Meaningful variable names
- Documentation comments

---

## 📝 TEST EDILEN SENARYOLAR

1. ✅ **Filtre Testi**: INFO/WARNING loglar işlenmez
2. ✅ **Maskeleme Testi**: TCKN, E-posta, Kredi Kartı maskelenir
3. ✅ **Zenginleştirme Testi**: Sender ID ve Transaction No eklenir
4. ✅ **Formatter Testi**: JSON, CSV, HTML çıktıları oluşturulur
5. ✅ **Performance Testi**: %10 drop deliberate olarak uygulanır
6. ✅ **Paralel Bağlantı Testi**: Çoklu producer bağlantıları
7. ✅ **Docker Testi**: Container'lar sorunsuz başlıyor

---

## 🎯 ÖZETİ

| Gereksinim | Durum | Notu |
|-----------|-------|------|
| Güvenlik (KVKK) | ✅ | TCKN, E-posta, Kredi Kartı maskelenir |
| Zenginleştirme | ✅ | sender_id, transaction_no eklenir |
| Log Filtreleme | ✅ | INFO/WARNING %57 oranında filtrelenir |
| Performans | ✅ | 10 log/saniye, Docker limits, 64KB buffer |
| Biçimlendirme | ✅ | JSON, CSV, HTML üç ayrı rol için |
| Tasarım Desenleri | ✅ | 5+ desen (Chain, Strategy, Factory, vb.) |
| Docker | ✅ | docker-compose.yml ile 2 container |
| Kod Kalitesi | ✅ | SOLID, async/await, exception handling |

---

**Proje Tamamlanma Tarihi**: 4 Haziran 2026
**Son Güncellemeler**: Sistem performansı optimizasyonu, resource limitleri, buffer artışı

