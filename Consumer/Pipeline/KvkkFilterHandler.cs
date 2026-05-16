using System;
using System.Text.RegularExpressions;

namespace Consumer.Pipeline
{
    public class KvkkFilterHandler : LogHandler
    {
        public override void Handle(string jsonLog)
        {
            // TCKN için 11 haneli sayıları bulup yıldızlarla (maskeleme) değiştiriyoruz
            // "12345678901" -> "***********"
            string maskedLog = Regex.Replace(jsonLog, @"\b\d{11}\b", "***********");
            
            // İleride buraya E-posta ve Kredi Kartı regex'leri de eklenebilir.
            
            Console.WriteLine("\n[1. ADIM - KVKK] Hassas veriler maskelendi.");

            // Zincirde başka halka varsa veriyi ona yolla
            if (_nextHandler != null)
            {
                _nextHandler.Handle(maskedLog); 
            }
        }
    }
}