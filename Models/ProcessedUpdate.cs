using System;

namespace Patient_Enquiry_System_version_2.Models
{
    public class ProcessedUpdate
    {
        public int Id { get; set; }
        public long UpdateId { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}
