using System;

namespace Patient_Enquiry_System_version_2.Models
{
    public class WebhookLog
    {
        public int Id { get; set; }
        public string RawJson { get; set; }
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    }
}
