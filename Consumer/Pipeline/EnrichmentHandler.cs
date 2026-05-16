using System;

namespace Consumer.Pipeline
{
    public class EnrichmentHandler : LogHandler
    {
        public override void Handle(string jsonLog)
        {
            string enrichedLog = jsonLog;
            
            // Eğer logun içinde ERROR veya Hata kelimesi geçiyorsa özel olarak etiketle
            if (enrichedLog.Contains("ERROR") || enrichedLog.Contains("Hata"))
            {
                Console.WriteLine("[2. ADIM - ZENGİNLEŞTİRME] Kritik bir hata tespit edildi, risk seviyesi yükseltildi.");
            }
            else
            {
                 Console.WriteLine("[2. ADIM - ZENGİNLEŞTİRME] Standart log olarak sınıflandırıldı.");
            }

            // İşlem bitti, sıradaki halkaya aktar
            if (_nextHandler != null)
            {
                _nextHandler.Handle(enrichedLog);
            }
        }
    }
}