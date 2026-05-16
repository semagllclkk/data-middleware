using System;

namespace MiddlewareProject.Models
{
    public abstract class BaseLog
    {
        public string LogId { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; }
        public string TCKN { get; set; }
        public string Email { get; set; }
        public string CreditCard { get; set; }
        public string City { get; set; }

        public abstract string GetLogType();
    }
}