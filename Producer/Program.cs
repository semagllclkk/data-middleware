using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MiddlewareProject.Factories;
using MiddlewareProject.Models;

namespace Producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== PRODUCER (Konteyner 1) ===");
            
            // Consumer'ın ayağa kalkması için 2 saniye avans veriyoruz
            await Task.Delay(2000); 

            // Docker ortamında "consumer-app" kullan, local'de "127.0.0.1"
            string host = Environment.GetEnvironmentVariable("CONSUMER_HOST") ?? "127.0.0.1";
            Console.WriteLine($"[DEBUG] CONSUMER_HOST env var: {Environment.GetEnvironmentVariable("CONSUMER_HOST")}");
            Console.WriteLine($"[DEBUG] Attempting to connect to: {host}:5000");
            int port = 5000;

            try
            {
                // Consumer'a boruyu (bağlantıyı) bağlıyoruz
                using TcpClient client = new TcpClient(host, port);
                using NetworkStream stream = client.GetStream();
                Console.WriteLine("Consumer'a bağlanıldı! Veri fırtınası başlıyor...\n");

                string[] logTypes = { "TRANSACTION", "ERROR", "SECURITY" };
                Random rnd = new Random();

                // Sonsuz döngü ile sürekli veri üretiyoruz
                while (true)
                {
                    // 1. Factory Pattern ile rastgele bir log nesnesi üret
                    string randomType = logTypes[rnd.Next(logTypes.Length)];
                    BaseLog newLog = LogFactory.CreateLog(randomType);
                    
                    // 2. Mock (Sahte) Verilerle içini doldur
                    newLog.UserId = $"USER_{rnd.Next(100, 999)}";
                    newLog.TCKN = "12345678901";           // KVKK: TCKN maskelenecek
                    newLog.Email = "kullanici@example.com"; // KVKK: e-posta maskelenecek
                    newLog.CreditCard = "4111-1111-1111-1111"; // KVKK: kredi kartı maskelenecek
                    
                    // Polimorfizm: Eğer bu bir hata loguysa, hata mesajını da ekle
                    if (newLog is ErrorLog errorLog) 
                        errorLog.ErrorMessage = "Geçersiz ID Hatası Alındı!";

                    // 3. Nesneyi JSON Metnine Çevir (Serialization)
                    string jsonLog = JsonSerializer.Serialize((object)newLog);
                    byte[] data = Encoding.UTF8.GetBytes(jsonLog);

                    // 4. TCP Üzerinden Fırlat
                    await stream.WriteAsync(data, 0, data.Length);
                    Console.WriteLine($"[GÖNDERİLDİ]: {newLog.GetLogType()} - {newLog.LogId}");

                    // Şimdilik saniyede 2 veri göndersin (Stres testinde bu gecikmeyi sileceğiz)
                    await Task.Delay(500); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bağlantı Hatası: Önce Consumer'ın çalıştığından emin olun. Hata: {ex.Message}");
            }
        }
    }
}