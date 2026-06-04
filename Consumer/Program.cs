using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== CONSUMER (Konteyner 2) ===");
            int port = 5000;
            
            // IPAddress.Any ile tüm ağ arayüzlerinden gelen verileri dinliyoruz
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Port {port} üzerinden veri akışı bekleniyor...\n");

            // Sonsuz döngü ile sürekli yeni bağlantı bekliyoruz
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client); 
            }
        }

        static async Task HandleClientAsync(TcpClient client)
        {
            using NetworkStream stream = client.GetStream();
            
            // Buffer boyutu artırıldı: 4KB -> 64KB (Sistem performansı ve throughput için)
            byte[] buffer = new byte[65536];
            int bytesRead;

            // --- ZİNCİRİ İNŞA EDİYORUZ (Chain of Responsibility) ---
            var severityFilter = new Pipeline.SeverityFilterHandler(); // Adım 0: INFO/WARNING düşür
            var filter      = new Pipeline.KvkkFilterHandler();
            var enrichment  = new Pipeline.EnrichmentHandler();
            var performance = new Pipeline.PerformanceHandler();

            // Her rol için ayrı bir FormatterHandler oluşturuyoruz (Strategy Pattern)
            // System Admin  -> HTML
            // CyberSec      -> CSV
            // Web Dev       -> JSON
            var formatterHtml = new Pipeline.FormatterHandler();
            var formatterCsv  = new Pipeline.FormatterHandler();
            var formatterJson = new Pipeline.FormatterHandler();

            formatterHtml.SetStrategy(new Consumer.Formatters.HtmlFormatter());
            formatterCsv .SetStrategy(new Consumer.Formatters.CsvFormatter());
            formatterJson.SetStrategy(new Consumer.Formatters.JsonFormatter());

            // Zincir: Filtre -> KVKK -> Zenginlestirme -> [HTML | CSV | JSON] -> Performans
            var fanOut = new Pipeline.FanOutHandler(
                new[] { formatterHtml, formatterCsv, formatterJson },
                performance
            );

            severityFilter.SetNext(filter).SetNext(enrichment).SetNext(fanOut);

            try
            {
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("\n--- YENİ VERİ GELDİ ---");
                    
                    // Gelen veriyi zincirin ilk halkasına teslim ediyoruz
                    // İşleme asynchronously yapılıyor (non-blocking I/O)
                    severityFilter.Handle(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA] Veri işleme sırasında hata: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}