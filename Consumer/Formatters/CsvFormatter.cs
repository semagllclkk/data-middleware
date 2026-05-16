namespace Consumer.Formatters
{
    public class CsvFormatter : IFormatterStrategy
    {
        public string Format(string data)
        {
            // Simülasyon: JSON formatındaki süslü parantezleri ve tırnakları temizleyip CSV benzeri virgüllü bir yapıya çeviriyoruz
            string csvLike = data.Replace("{", "").Replace("}", "").Replace("\"", "");
            return $"[CSV FORMATI] {csvLike}";
        }
    }
}