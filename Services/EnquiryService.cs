using System;
using System.Linq;
using System.Threading.Tasks;
using Patient_Enquiry_System_version_2.Data;
using Patient_Enquiry_System_version_2.Models;

namespace Patient_Enquiry_System_version_2.Services
{
    public class EnquiryService
    {
        private readonly ApplicationDbContext _context;

        public EnquiryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ProcessEnquiryAsync(TelegramUpdate update)
        {
            var messageText = update.message.text;

            if (string.IsNullOrWhiteSpace(messageText))
                return false;

            var lines = messageText.Split('\n');

            string name = null;
            string phone = null;
            string issue = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("NAME:", StringComparison.OrdinalIgnoreCase))
                    name = line.Replace("NAME:", "").Trim();

                if (line.StartsWith("PHONE:", StringComparison.OrdinalIgnoreCase))
                    phone = line.Replace("PHONE:", "").Trim();

                if (line.StartsWith("ISSUE:", StringComparison.OrdinalIgnoreCase))
                    issue = line.Replace("ISSUE:", "").Trim();
            }

            // Validation
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(issue) ||
                phone.Length != 10 ||
                !phone.All(char.IsDigit))
            {
                return false;
            }

            var enquiry = new Enquiry
            {
                TelegramUserId = update.message.from.id,
                Username = update.message.from.username,
                Name = name,
                Phone = phone,
                Issue = issue,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Enquiries.Add(enquiry);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
