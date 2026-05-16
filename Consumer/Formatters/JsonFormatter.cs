namespace Consumer.Formatters
{
    public class JsonFormatter : IFormatterStrategy
    {
        public string Format(string data)
        {
            // Veri zaten JSON formatında geliyor, o yüzden doğrudan döndürüyoruz
            return $"[JSON FORMATI] {data}";
        }
    }
}