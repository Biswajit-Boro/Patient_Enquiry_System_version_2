using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Patient_Enquiry_System_version_2.Models
{
    public class StaffReplyRequest
    {
        public int TicketId { get; set; }

        public string MessageText { get; set; }

        public string NewStatus { get; set; } // Optional: Open, InProgress, Closed
    }
}