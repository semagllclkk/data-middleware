using System;
using System.Collections.Generic;

namespace Consumer.Pipeline
{
    /// <summary>
    /// Tek bir log verisini birden fazla FormatterHandler'a aynı anda iletir.
    /// Ödev gereksinimi: System Admin (HTML), CyberSec (CSV), Web Dev (JSON)
    /// farklı formatlarda çıktı almalıdır.
    /// </summary>
    public class FanOutHandler : LogHandler
    {
        private readonly IEnumerable<FormatterHandler> _formatters;
        private readonly LogHandler _sharedNext;

        public FanOutHandler(IEnumerable<FormatterHandler> formatters, LogHandler sharedNext)
        {
            _formatters  = formatters;
            _sharedNext  = sharedNext;
        }

        public override void Handle(string jsonLog)
        {
            string[] roleNames = { "System Admin (HTML)", "CyberSec (CSV)", "Web Dev (JSON)" };
            int i = 0;

            foreach (var formatter in _formatters)
            {
                Console.WriteLine($"\n  >> ROL: {roleNames[i++]}");
                // Her formatter'ın çıktısı performance handler'a gider
                formatter.SetNext(_sharedNext);
                formatter.Handle(jsonLog);
            }
        }
    }
}
