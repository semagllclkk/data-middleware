using System;
using MiddlewareProject.Models; // Modellerimizi kullanabilmek için ekliyoruz

namespace MiddlewareProject.Factories
{
    public class LogFactory
    {
        public static BaseLog CreateLog(string logType)
        {
            return logType.ToUpper() switch
            {
                "TRANSACTION" => new TransactionLog(),
                "ERROR" => new ErrorLog(),
                "SECURITY" => new SecurityLog(),
                _ => throw new ArgumentException($"Geçersiz log tipi: {logType}")
            };
        }
    }
}