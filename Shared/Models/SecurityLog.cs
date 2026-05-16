namespace MiddlewareProject.Models
{
    public class SecurityLog : BaseLog
    {
        public string EventDetail { get; set; } 
        public string WarningLevel { get; set; } = "WARNING";

        public override string GetLogType() => "SECURITY";
    }
}