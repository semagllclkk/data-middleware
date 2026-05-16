using System;
using Consumer.Formatters;

namespace Consumer.Pipeline
{
    public class FormatterHandler : LogHandler
    {
        private IFormatterStrategy _strategy;

        // Dışarıdan strateji belirlememizi sağlayan metot
        public void SetStrategy(IFormatterStrategy strategy)
        {
            _strategy = strategy;
        }

        public override void Handle(string jsonLog)
        {
            // Eğer özel bir strateji atanmamışsa, varsayılan olarak JSON kullan
            if (_strategy == null)
            {
                _strategy = new JsonFormatter();
            }

            // Seçili stratejiye göre veriyi dönüştür
            string formattedData = _strategy.Format(jsonLog);
            Console.WriteLine($"[3. ADIM - BİÇİMLENDİRME] {formattedData}");

            // Zincirin sıradaki halkasına (Performans) aktar
            if (_nextHandler != null)
            {
                _nextHandler.Handle(formattedData);
            }
        }
    }
}