using System.Threading.Tasks;
using System.Data.Entity;
using Patient_Enquiry_System_version_2.Data;
using Patient_Enquiry_System_version_2.Models;

namespace Patient_Enquiry_System_version_2.Services
{
    public class IdempotencyService
    {
        private readonly ApplicationDbContext _context;

        public IdempotencyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsProcessedAsync(long updateId)
        {
            return await _context.ProcessedUpdates
                .AnyAsync(x => x.UpdateId == updateId);
        }

        public async Task MarkAsProcessedAsync(long updateId)
        {
            _context.ProcessedUpdates.Add(new ProcessedUpdate
            {
                UpdateId = updateId
            });

            await _context.SaveChangesAsync();
        }
    }
}
