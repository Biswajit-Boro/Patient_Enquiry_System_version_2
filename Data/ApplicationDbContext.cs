using System.Data.Entity;
using Patient_Enquiry_System_version_2.Models;

namespace Patient_Enquiry_System_version_2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<ProcessedUpdate> ProcessedUpdates { get; set; }
        public DbSet<WebhookLog> WebhookLogs { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Ticket> Tickets { get; set; }   // 🔹 NEW
        public DbSet<TicketMessage> TicketMessages { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

    }
}