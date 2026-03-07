using System;

namespace Patient_Enquiry_System_version_2.Models
{
    public class Announcement
    {
        public int AnnouncementId { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public int TotalSent { get; set; }
        public int TotalFailed { get; set; }
    }
}