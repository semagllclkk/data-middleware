namespace MiddlewareProject.Models
{
    public class TransactionLog : BaseLog
    {
        public string Iban { get; set; }
        public string Swift { get; set; }
        public string SourceAccount { get; set; } 
        public string DestinationAccount { get; set; } 

        public override string GetLogType() => "TRANSACTION";
    }
}