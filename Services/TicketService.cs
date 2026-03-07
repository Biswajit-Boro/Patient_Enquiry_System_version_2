using System;
using System.Linq;
using System.Threading.Tasks;
using Patient_Enquiry_System_version_2.Data;
using Patient_Enquiry_System_version_2.Models;

namespace Patient_Enquiry_System_version_2.Services
{
    public class TicketService
    {
        private readonly ApplicationDbContext _context;

        // ==========================================
        // Constructor
        // This receives the database context from controller
        // so all operations use same DB connection safely
        // ==========================================
        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // Get Active Ticket
        // Returns ticket where Status is NOT Closed
        // Used when patient sends new message
        // Ensures only ONE active ticket exists
        // ==========================================
        public Ticket GetActiveTicket(string patientId)
        {
            return _context.Tickets
                .FirstOrDefault(t =>
                    t.PatientId == patientId &&
                    t.Status != "Closed");
        }

        // ==========================================
        // Create Ticket
        // Called when patient sends first issue message
        //
        // NEW CHANGE:
        // Priority automatically set to "Medium"
        // This ensures no NULL priority exists
        // ==========================================
        public async Task<Ticket> CreateTicketAsync(string patientId)
        {
            var ticket = new Ticket
            {
                PatientId = patientId,
                Status = "Open",
                CreatedAt = DateTime.UtcNow,
                Priority = "Medium"
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        // ==========================================
        // Save Message
        // Stores patient or staff message
        // SentAt stored in UTC (correct production practice)
        // Now supports optional FileUrl and FileType
        // ==========================================
        public async Task SaveMessageAsync(
            int ticketId,
            string senderType,
            string messageText,
            string fileUrl = null,
            string fileType = null)
        {
            var message = new TicketMessage
            {
                TicketId = ticketId,
                SenderType = senderType,
                MessageText = messageText,
                FileUrl = fileUrl,
                FileType = fileType,
                SentAt = DateTime.UtcNow
            };

            _context.TicketMessages.Add(message);
            await _context.SaveChangesAsync();
        }

        // ==========================================
        // Get Ticket By Id
        // Used by StaffController when opening ticket
        // ==========================================
        public Ticket GetTicketById(int ticketId)
        {
            return _context.Tickets
                .FirstOrDefault(t => t.TicketId == ticketId);
        }

        // ==========================================
        // Add Staff Reply
        // Stores staff message in conversation
        // ==========================================
        public async Task AddStaffReplyAsync(int ticketId, string messageText)
        {
            var message = new TicketMessage
            {
                TicketId = ticketId,
                SenderType = "Staff",
                MessageText = messageText,
                SentAt = DateTime.UtcNow
            };

            _context.TicketMessages.Add(message);
            await _context.SaveChangesAsync();
        }

        // ==========================================
        // Update Ticket Status
        // Used when staff selects InProgress or Closed
        // ClosedAt timestamp stored automatically
        // ==========================================
        public async Task UpdateTicketStatusAsync(int ticketId, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                return;

            var ticket = _context.Tickets
                .FirstOrDefault(t => t.TicketId == ticketId);

            if (ticket == null)
                return;

            ticket.Status = newStatus;

            if (newStatus == "Closed")
                ticket.ClosedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}