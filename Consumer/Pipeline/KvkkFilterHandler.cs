using System;
using System.Text.RegularExpressions;

namespace Consumer.Pipeline
{
    public class KvkkFilterHandler : LogHandler
    {
        public override void Handle(string jsonLog)
        {
            // 1) TCKN: 11 haneli sayı -> "***********"
            string maskedLog = Regex.Replace(jsonLog, @"\b\d{11}\b", "***********");

            // 2) E-posta adresleri -> "***@***.***"
            maskedLog = Regex.Replace(maskedLog,
                @"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}",
                "***@***.***");

            // 3) Kredi kartı numarası (XXXX-XXXX-XXXX-XXXX veya 16 hane) -> "****-****-****-****"
            maskedLog = Regex.Replace(maskedLog,
                @"\b(?:\d{4}[-\s]?){3}\d{4}\b",
                "****-****-****-****");

            Console.WriteLine("[1. ADIM - KVKK] Hassas veriler maskelendi: TCKN, E-posta, Kredi Kartı.");

            if (_nextHandler != null)
                _nextHandler.Handle(maskedLog);
        }
    }
}