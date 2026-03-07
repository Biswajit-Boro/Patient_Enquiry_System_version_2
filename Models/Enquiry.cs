using System;

namespace Patient_Enquiry_System_version_2.Models
{
    public class Enquiry
    {
        public int Id { get; set; }

        public long TelegramUserId { get; set; }
        public string Username { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string Issue { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
