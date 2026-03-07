using System;

namespace Patient_Enquiry_System_version_2.Models
{
    public class Ticket
    {
        // Primary key for the ticket
        public int TicketId { get; set; }

        // Links ticket to patient (PID-1001, etc.)
        public string PatientId { get; set; }

        // Current status of ticket
        // Examples: Open, InProgress, Closed
        public string Status { get; set; }

        // When ticket was first created (stored in UTC)
        public DateTime CreatedAt { get; set; }

        // When ticket was closed (NULL if still open)
        public DateTime? ClosedAt { get; set; }


        // ================================
        // NEW FIELD: Priority
        // ================================
        // This defines urgency level.
        //
        // Production use example:
        // Low       → General question
        // Medium    → Normal issue
        // High      → Serious issue
        // Critical  → Emergency attention needed
        //
        // Default will be set in service layer later.
        public string Priority { get; set; }
    }
}