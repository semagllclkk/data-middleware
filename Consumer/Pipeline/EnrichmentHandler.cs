using System;

namespace Consumer.Pipeline
{
    public class EnrichmentHandler : LogHandler
    {
        public override void Handle(string jsonLog)
        {
            // Özel etiketler oluştur
            string senderId   = $"SENDER-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            string txNo       = $"TXN-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            bool   isCritical = jsonLog.Contains("ERROR") || jsonLog.Contains("Hata");
            string severity   = isCritical ? "CRITICAL" : "INFO";
            string debugNote  = isCritical
                ? "[DEBUG] Kritik hata tespit edildi, risk seviyesi yükseltildi."
                : "[DEBUG] Standart log, ek islem gerekmez.";

            // Log string'ine etiketleri GERCEKTEN ekle
            string enrichedLog =
                $"{jsonLog.TrimEnd()}" +
                $" | sender_id:{senderId}" +
                $" | transaction_no:{txNo}" +
                $" | severity:{severity}" +
                $" | message:{severity} log alindi." +
                $" | {debugNote}";

            Console.WriteLine($"[2. ADIM - ZENGINLESTIRME] sender_id:{senderId} | tx:{txNo} | severity:{severity}");

            if (_nextHandler != null)
                _nextHandler.Handle(enrichedLog);
        }
    }
}