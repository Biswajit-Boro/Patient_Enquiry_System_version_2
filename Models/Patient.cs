using System;

namespace Patient_Enquiry_System_version_2.Models
{
    public class Patient
    {
        public string PatientId { get; set; }   // Example: PID-1001

        public string Name { get; set; }

        public long? TelegramUserId { get; set; }   // Nullable until authenticated

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}