# 📹 PROJE SUNUMU SENARYOSU
## Data Middleware Sistemi - 8 Dakikalık Video

---

## 🎬 VIDEO PLANI

| Bölüm | Süre | İçerik |
|--------|------|--------|
| **1. Giriş** | 1 min | Proje tanıtımı, hedef |
| **2. Mimari Tasarım** | 1.5 min | Producer-Consumer modeli, Docker |
| **3. Tasarım Desenleri** | 2 min | 5+ desen gösterimi |
| **4. Live Demo** | 2.5 min | Docker up, veri akışı, filterler |
| **5. Performans Testi** | 0.5 min | Throughput ve resource kullanımı |
| **6. Özet** | 0.5 min | Sonuç ve öğrenimler |
| **TOPLAM** | **8 dakika** | ✅ |

---

## 📝 KONUŞMA METNİ (8 Dakika)

### **BÖLÜM 1 - GIRIŞ (1 dakika)**

*[Kamera başlıyor, başlık gösteriliyor: "Data Middleware Sistemi"]*

"Merhaba. Bugün size CENG302 Dönem Sonu Projesi olarak geliştirdiğimiz **Data Middleware Sistemi**'ni göstereceğim.

Bu sistem, gerçek dünyada sıkça karşılaşılan bir sorunu çözmektedir: **Bir kaynaktan gelen verileri, farklı rollere ve formatlara uygun şekilde işlemek**. 

Düşünün ki: Bir bankanın log sistemi var. Bu loglar hem sistem yöneticileri, hem siber güvenlik ekibi, hem de web developers tarafından görülmesi gerekiyor. Ama her biri farklı bir format ister. Üstelik hassas veriler maskelenmelidir.

İşte bunu yapan system buradaki projemiz.

Görmek ister misiniz?"

---

### **BÖLÜM 2 - MİMARİ TASARIM (1.5 dakika)**

*[Mimarı diagram göster: Producer → Network → Consumer]*

"Sistem iki ana bileşenden oluşuyor:

**Producer Konteyner**: Saniyede 10 adet log oluşturuyor. Her log, üç tür olabilir: TRANSACTION, ERROR, veya SECURITY. Bu logları JSON formatında Producer'dan Consumer'a gönderiyor.

**Consumer Konteyner**: Bu logları alıyor ve bir işlem hattından geçiriyor.

*[Pipeline şeması göster]*

Bu işlem hattında 6 adım var:

**Adım 1 - Severity Filter**: INFO ve WARNING seviyesi loglar % 42 oranında filtreleniyor. Çünkü bu loglar önemsiz kabul ediliyor. Sadece ERROR ve CRITICAL geçiyor.

**Adım 2 - KVKK Maskeleme**: Hassas veriler maskeleniyor. Mesela 11 haneli kimlik numarası görüyorsanız, tamamı yıldızla değiştiriliyor. E-posta adresleri maskeleniyor. Kredi kartı numaraları maskeleniyor.

**Adım 3 - Zenginleştirme**: Her log'a yeni bilgiler ekleniyor. Kime ait olduğunu gösteren bir sender ID, işlem takibi için bir transaction number.

**Adım 4 - Paralel Biçimlendirme**: Artık verinin 3 ayrı kopyası yapılıyor:
- Sistem yöneticileri için HTML tablo formatı
- Siber güvenlik ekibi için CSV (spreadsheet) formatı
- Web geliştiriciler için JSON formatı

**Adım 5 - Performance Test**: Sistemin yük altındaki davranışını test etmek için %10 oranında deliberate drop yapılıyor.

Tüm bu adımlar **asynchronously** çalışıyor, yani bloke etmiyor."

---

### **BÖLÜM 3 - TASARIM DESENLERİ (2 dakika)**

*[Kod editor açılıyor, Chain of Responsibility kodu gösteriliyor]*

"Bu sistemde **5 farklı tasarım deseni** kullandık. Buna bakın:

**#1 - Chain of Responsibility Pattern**:
*[SetNext() metodu göster]*

Her handler bir zincire bağlanıyor. Biri işi bitirince diğerine devrediliyor. Böylece esnek bir işlem hattı oluşuyor. Yeni bir handler eklemek çok kolay.

**#2 - Strategy Pattern**:
*[IFormatterStrategy interface göster]*

Formatter'ın nasıl çalışacağını runtime'da seçebiliyoruz. JSON olabilir, CSV olabilir, HTML olabilir. Aynı FormatterHandler, ama farklı stratejiler.

**#3 - Factory Pattern**:
*[LogFactory.CreateLog() kodu göster]*

Log nesnelerini merkezileştirilmiş bir yerden oluşturuyoruz. 'TRANSACTION' dersen TransactionLog, 'ERROR' dersen ErrorLog geri geliyor.

**#4 - Polymorphism**:
*[BaseLog ve türetilmiş sınıfları göster]*

BaseLog soyut sınıfından ErrorLog, TransactionLog, SecurityLog türetilmiş. Her biri kendi GetLogType() metodunu override ediyor.

**#5 - Fan-Out Pattern** (bonus):
*[FanOutHandler kodu göster]*

Tek bir log alınıyor, 3 farklı formatter'a paralel gönderiliyor. Performansı ciddi şekilde artırıyor."

---

### **BÖLÜM 4 - LIVE DEMO (2.5 dakika)**

*[Terminal açılıyor]*

"Şimdi sistemi canlı çalıştıralım. 

```bash
docker-compose up --build
```

*[Docker containers başlıyor, output gösteriliyor]*

Görüyorsunüz, iki container ayağa kalkıyor:
- Consumer: Port 5000 üzerinden dinlemeye başladı
- Producer: Bağlantı kurdu ve veri göndermeye başladı

*[Output örneği gösteriliyor]*

Bakın, Producer her 100 milisaniyede bir log gönderiyor. Yani saniyede 10 log.

Consumer bunu alıyor ve pipeline'den geçiriyor:

**İlk log INFO seviyesi** → FİLTRE ALDI, düşürüldü
**İkinci log ERROR seviyesi** → FİLTRE GEÇTİ ✓
**Maskeleme**: TCKN '12345678901' → '***********' oldu
**Zenginleştirme**: sender_id = SENDER-a3b2c1d4
**Formatlamalar**:
- HTML: `<table><tr><td>ERROR</td><td>sender_id</td>...`
- CSV: `ERROR,SENDER-a3b2c1d4,...`
- JSON: `{\"LogLevel\":\"ERROR\", ...}`

Gördüğünüz gibi 3 farklı rol için 3 farklı format aynı anda üretiliyor.

Bu işlem saniyede ~5 log'a tekabül ediyor. Yani throughput ciddi."

---

### **BÖLÜM 5 - PERFORMANS TESTİ (0.5 dakika)**

*[Grafikler gösteriliyor]*

"Performans açısından baktığımızda:

**Throughput**: 
- Gelen: 10 log/saniye
- Geçen: ~5.1 log/saniye (50% başarı oranı)

**Resource Limits** (Docker):
- Memory: 512MB limit, 256MB reservation per container
- CPU: 0.5 cores per container
- Bu limitler container'ların kontrolsüz kaynak tüketmesini engelliyor

**Network Buffer**: 64 KB (16x artış yapıldı)
Bu, daha fazla veriyi tek seferde almamızı sağlıyor.

Sistem yüksek yük altında stabil kalıyor."

---

### **BÖLÜM 6 - ÖZET (0.5 dakika)**

*[Slide: "Tamamlanan Gereksinimler"]*

"Bu projede şunları başardık:

✅ **Güvenlik**: KVKK uyumlu veri maskeleme
✅ **Esnek Mimarı**: 5+ tasarım deseni
✅ **Yüksek Performans**: Asynchronous işleme, 10 log/saniye
✅ **Multi-Format Çıktı**: 3 rol, 3 format
✅ **Production Ready**: Docker, resource limits, error handling

Bu sistem, gerçek bir veri middleware'inin temel unsurlarını içeriyor. Bankalar, halkalar, sağlık kurumları böyle sistemler kullanıyor.

Sorularınız var mı?

Teşekkür ederim."

*[Başlık: "CENG302 - Data Middleware"]*

---

## 🎥 TEKNIK DETAYLAR

### Video Yapımı İçin Gereken:
1. **Ekran Kaydı**: OBS Studio veya VS Code built-in recorder
2. **Terminal Output**: Docker log'ları açık tutma
3. **Kod Editor**: VS Code ile dosyalar gösterme
4. **Ses**: Telefon mikrofonundan veya headset

### Kameraya Gösterilecek Dosyalar:
- [Consumer/Program.cs](Consumer/Program.cs) → Paralel işleme
- [Consumer/Pipeline/](Consumer/Pipeline/) → 6 handler
- [Consumer/Formatters/IFormatterStrategy.cs](Consumer/Formatters/IFormatterStrategy.cs) → Strategy
- [Shared/Factories/LogFactory.cs](Shared/Factories/LogFactory.cs) → Factory
- [docker-compose.yml](docker-compose.yml) → Resource limits

### Terminal Komutları:
```bash
# Docker başlat
docker-compose up --build

# Logs takip et
docker-compose logs -f consumer-app
docker-compose logs -f producer-app

# Containers durumu
docker ps
```

### Output Örnekleri:
```
--- YENİ VERİ GELDİ ---
Input: {"LogLevel":"INFO", "TCKN":"12345678901"}
[FİLTRE]: INFO silinmiş
---

--- YENİ VERİ GELDİ ---
Input: {"LogLevel":"ERROR", "TCKN":"12345678901"}
[MASKELEME]: TCKN değeri maskelendi
[ZENGINLEŞTIRME]: sender_id eklendi
[FORMATTER - HTML]: <table>...</table>
[FORMATTER - CSV]: ERROR,SENDER-xyz,...
[FORMATTER - JSON]: {"LogLevel":"ERROR",...}
```

---

## ⏱️ ZAMAN YÖNETIMI

- **0:00-1:00** → Giriş ve sistem tanıtımı
- **1:00-2:30** → Mimarı ve işlem hattı
- **2:30-4:30** → Tasarım desenleri
- **4:30-7:00** → Live demo
- **7:00-7:30** → Performans metrikleri
- **7:30-8:00** → Özet ve sonuç

---

## 📋 ÖN HAZIRLIK KONTROL LİSTESİ

- [ ] Docker desktop açık ve çalışıyor
- [ ] docker-compose.yml güncel
- [ ] VS Code açık, Shared/Consumer/Producer klasörleri görülüyor
- [ ] Terminal pencereleri hazır
- [ ] Mikrofon testi yapıldı
- [ ] Ekran kaydı yazılımı (OBS, etc.) açık
- [ ] 10 dakikalık boş disk alanı var

---

