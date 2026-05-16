namespace Consumer.Formatters
{
    public class HtmlFormatter : IFormatterStrategy
    {
        public string Format(string data)
        {
            // Veriyi basit bir HTML yapısı içine sarıyoruz
            return $"<div class='log-item'>\n  <p><b>Veri:</b> {data}</p>\n</div>";
        }
    }
}