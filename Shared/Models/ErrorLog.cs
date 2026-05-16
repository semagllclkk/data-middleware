namespace MiddlewareProject.Models
{
    public class ErrorLog : BaseLog
    {
        public string ErrorMessage { get; set; } 
        
        public override string GetLogType() => "ERROR";
    }
}