using System.Collections.Generic;

namespace Patient_Enquiry_System_version_2.Models
{
    // This ViewModel is used ONLY for Staff Details page.
    // It combines Ticket + List of Messages into one object.
    // This avoids using ViewBag (which is weakly typed and unsafe).
    public class StaffTicketDetailsViewModel
    {
        // The ticket itself (contains PatientId, Status, etc.)
        public Ticket Ticket { get; set; }

        // The full conversation messages for that ticket
        public List<TicketMessage> Messages { get; set; }
    }
}