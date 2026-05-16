namespace Consumer.Formatters
{
    public class TxtFormatter : IFormatterStrategy
    {
        public string Format(string data)
        {
            // Düz metin (TXT) dosyaları için okunaklı bir ayrıştırıcı çizgi ekliyoruz
            return $"--- YENİ LOG KAYDI ---\n{data}\n----------------------";
        }
    }
}