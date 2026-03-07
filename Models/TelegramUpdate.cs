using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Patient_Enquiry_System_version_2.Models
{
    public class TelegramUpdate
    {
        public long update_id { get; set; }
        public TelegramMessage message { get; set; }
    }

    public class TelegramMessage
    {
        public long message_id { get; set; }
        public TelegramUser from { get; set; }
        public string text { get; set; }

        // ── FILE SUPPORT ──
        public TelegramPhotoSize[] photo { get; set; }
        public TelegramDocument document { get; set; }
    }

    public class TelegramUser
    {
        public long id { get; set; }
        public string username { get; set; }
    }

    public class TelegramPhotoSize
    {
        public string file_id { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class TelegramDocument
    {
        public string file_id { get; set; }
        public string file_name { get; set; }
        public string mime_type { get; set; }
    }
}