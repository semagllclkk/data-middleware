using System;

namespace Consumer.Pipeline
{
    /// <summary>
    /// Gereksinim 3: INFO ve WARNING seviyesindeki logları düşürür (filter-out).
    /// Yalnızca ERROR ve CRITICAL loglar zincirin devamına geçer.
    /// </summary>
    public class SeverityFilterHandler : LogHandler
    {
        public override void Handle(string jsonLog)
        {
            // LogLevel değerini JSON string içinden okuyoruz
            bool isLowPriority = 
                (jsonLog.Contains("\"LogLevel\":\"INFO\"")    || jsonLog.Contains("LogLevel:INFO")) ||
                (jsonLog.Contains("\"LogLevel\":\"WARNING\"") || jsonLog.Contains("LogLevel:WARNING"));

            if (isLowPriority)
            {
                Console.WriteLine("[0. ADIM - FİLTRELEME] INFO/WARNING log düşürüldü, işleme devam edilmiyor.");
                return; // Zinciri burada kes
            }

            Console.WriteLine("[0. ADIM - FİLTRELEME] ERROR/CRITICAL log — işleme devam ediliyor.");
            _nextHandler?.Handle(jsonLog);
        }
    }
}
