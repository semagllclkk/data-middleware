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

            // Sonsuz döngü ile sürekli yeni bağlantı bekliyoruz (Kapanmasın diye)
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                
                // Bağlantı geldiğinde ana iş parçacığını (thread) kilitlememek için 
                // işlemi asenkron olarak fırlatıp yeni veri beklemeye devam ediyoruz.
                _ = HandleClientAsync(client); 
            }
        }

       static async Task HandleClientAsync(TcpClient client)
{
    using NetworkStream stream = client.GetStream();
    byte[] buffer = new byte[2048];
    int bytesRead;

    // 1. ZİNCİRİ İNŞA EDİYORUZ (Bir kere oluşturmak yeterli)
    var filter = new Pipeline.KvkkFilterHandler();
    var enrichment = new Pipeline.EnrichmentHandler();
    var formatter = new Pipeline.FormatterHandler();
    var performance = new Pipeline.PerformanceHandler();

// SİHİRLİ DOKUNUŞ BURADA: Biçimlendirme stratejisini dinamik olarak CSV yapıyoruz
    formatter.SetStrategy(new Consumer.Formatters.CsvFormatter());
    formatter.SetStrategy(new Consumer.Formatters.HtmlFormatter());
    formatter.SetStrategy(new Consumer.Formatters.TxtFormatter());


    // Halkaları birbirine bağlıyoruz
    filter.SetNext(enrichment).SetNext(formatter).SetNext(performance);

    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
    {
        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"\n--- YENİ VERİ GELDİ ---");
        
        // 2. GELEN VERİYİ ZİNCİRİN İLK HALKASINA TESLİM EDİYORUZ
        filter.Handle(message); 
    }
    client.Close();
}
    }
}