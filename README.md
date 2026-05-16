# 🛡️ Data Processing & Log Management Middleware (Dockerized)

Bu proje, yüksek performanslı ve asenkron bir mimari kullanılarak tasarlanmış, iki bağımsız mikroservis (Producer ve Consumer) arasında **ara katman (RabbitMQ, Kafka vb.) olmadan** doğrudan TCP Socket üzerinden veri iletişimini sağlayan bir *Middleware* (Ara Yazılım) simülasyonudur.

## 🎯 Projenin Amacı ve Özellikleri

Sistem, saniyede yüksek hacimli verinin (e-ticaret işlemleri, sistem hataları ve güvenlik uyarıları) kesintisiz bir şekilde üretilip, diğer bir servis tarafından karşılanmasını ve işlenmesini hedefler. İşleyici (Consumer) servis, gelen ham logları **4 aşamalı bir boru hattından (Pipeline)** geçirerek kurumsal ve yasal (KVKK) standartlara uygun hale getirir.

### ⚙️ Pipeline (Boru Hattı) Aşamaları
1. **Filtreleme (KVKK):** Gelen log içindeki hassas veriler (TCKN vb.) Regex kullanılarak maskelenir ve anonimleştirilir.
2. **Zenginleştirme (Enrichment):** Verinin içeriği analiz edilir. Kritik hatalar veya güvenlik tehditleri algılandığında log seviyesi yükseltilir ve etiketlenir.
3. **Biçimlendirme (Formatting):** İşlenmiş veri, anlık sistem gereksinimlerine göre dinamik olarak belirlenen formatlara (JSON, TXT, CSV, HTML) dönüştürülür.
4. **Performans ve Stres Testi:** Sistemin darboğaza girmesini simüle etmek amacıyla yoğun yük altında rastgele veri silme (Drop) işlemleri yapılarak dayanıklılık test edilir.

---

## 🏗️ Kullanılan Tasarım Kalıpları (Design Patterns)

Proje katı Nesne Yönelimli Programlama (OOP) prensiplerine (SOLID) sadık kalınarak, 3 farklı kurumsal tasarım kalıbı ile inşa edilmiştir:

* **Factory Method Pattern:** Logların üretimi sırasında nesnelerin (`TransactionLog`, `ErrorLog`, `SecurityLog`) doğrudan somut sınıflar yerine tek bir merkezden (LogFactory) dinamik olarak üretilmesini sağlar.
* **Chain of Responsibility Pattern:** Veri işleme boru hattı (KVKK Filtresi -> Zenginleştirme -> Biçimlendirme -> Performans) bu kalıpla yönetilir. Gelen veri her bir halkadan geçer ve işlenerek bir sonraki aşamaya aktarılır. Karmaşık `if-else` bloklarını önler.
* **Strategy Pattern:** Biçimlendirme aşamasında, verinin hangi formatta dışa aktarılacağının çalışma zamanında (runtime) dinamik olarak değiştirilmesine olanak tanır (`JsonFormatter`, `TxtFormatter`, vb.).

---

## 🚀 Teknolojiler
* **Dil:** C# (.NET 8.0)
* **Haberleşme:** Asenkron TCP Socket Server/Client
* **Konteynerleştirme:** Docker & Docker Compose

## 🛠️ Nasıl Çalıştırılır?

Bu proje, dışarıdan hiçbir bağımlılık gerektirmeden sanal bir ağ üzerinde çalışacak şekilde Dockerize edilmiştir.

1.  Terminal veya Command Prompt'u açın ve projenin kök dizinine (bu dosyanın bulunduğu yere) gidin.
2.  Aşağıdaki komutu çalıştırarak konteynerleri ayağa kaldırın:
    ```bash
    docker-compose up --build
    ```
3.  Sistem önce `Consumer` servisini başlatacak ve 5000 portunu dinlemeye alacaktır. Ardından `Producer` servisi devreye girecek ve veri akışı asenkron olarak başlayacaktır.