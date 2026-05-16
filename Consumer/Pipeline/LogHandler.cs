namespace Consumer.Pipeline
{
    public abstract class LogHandler
    {
        protected LogHandler? _nextHandler;

        // Zincirin bir sonraki halkasını belirleyen metot
        public LogHandler SetNext(LogHandler nextHandler)
        {
            _nextHandler = nextHandler;
            return nextHandler;
        }

        // Her halkanın kendi işini yapacağı ana metot
        public abstract void Handle(string jsonLog);
    }
}