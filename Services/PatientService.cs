using System.Linq;
using System.Threading.Tasks;
using Patient_Enquiry_System_version_2.Data;
using Patient_Enquiry_System_version_2.Models;

namespace Patient_Enquiry_System_version_2.Services
{
    public class PatientService
    {
        private readonly ApplicationDbContext _context;

        public PatientService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1️⃣ Check if Telegram account already bound
        public async Task<Patient> GetPatientByTelegramUserIdAsync(long telegramUserId)
        {
            return _context.Patients
                .FirstOrDefault(p => p.TelegramUserId == telegramUserId);
        }

        // 2️⃣ Check if PatientId exists and is active
        public async Task<Patient> GetPatientByPatientIdAsync(string patientId)
        {
            return _context.Patients
                .FirstOrDefault(p => p.PatientId == patientId && p.IsActive);
        }

        // 3️⃣ Bind TelegramUserId permanently
        public async Task BindTelegramUserAsync(Patient patient, long telegramUserId)
        {
            patient.TelegramUserId = telegramUserId;
            await _context.SaveChangesAsync();
        }

        // 4️⃣ Synchronous version for MVC UI
        public Patient GetByPatientId(string patientId)
        {
            return _context.Patients
                .FirstOrDefault(p => p.PatientId == patientId);
        }
    }
}