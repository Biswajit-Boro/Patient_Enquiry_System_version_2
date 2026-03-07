using System;

namespace Patient_Enquiry_System_version_2.Models
{
    public class TicketMessage
    {
        public int TicketMessageId { get; set; }   // 🔹 Primary Key (EF convention)

        public int TicketId { get; set; }

        public string SenderType { get; set; }   // "Patient" or "Staff"

        public string MessageText { get; set; }

        public DateTime SentAt { get; set; }
        // ── FILE SUPPORT ──
        public string FileUrl { get; set; }
        public string FileType { get; set; }
    }
}