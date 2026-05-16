using System;

namespace Consumer.Pipeline
{
    public class PerformanceHandler : LogHandler
    {
        public override void Handle(string jsonLog)
        {
            // Simülasyon: Sistem stres altındaysa %10 ihtimalle veriyi sil (Drop)
            Random rnd = new Random();
            int stressTestValue = rnd.Next(1, 101);

            if (stressTestValue <= 10) // %10 ihtimale denk gelir
            {
                Console.WriteLine("[4. ADIM - PERFORMANS] Sistem aşırı yük altında! Stres testi gereği bu veri SİLİNDİ (Dropped).");
                // Return diyerek zinciri burada kırıyoruz, işlemi bitiriyoruz.
                return; 
            }

            Console.WriteLine("[4. ADIM - PERFORMANS] Veri başarıyla işlendi ve performans metrikleri kaydedildi.");

            // Varsa sıradaki halkaya aktar (Şu an zincirin sonu burası)
            if (_nextHandler != null)
            {
                _nextHandler.Handle(jsonLog);
            }
        }
    }
}