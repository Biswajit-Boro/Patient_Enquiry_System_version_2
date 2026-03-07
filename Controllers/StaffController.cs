using Patient_Enquiry_System_version_2.Data;
using Patient_Enquiry_System_version_2.Models;
using Patient_Enquiry_System_version_2.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Patient_Enquiry_System_version_2.Controllers
{
    public class StaffController : Controller
    {
        // =====================================================
        // DASHBOARD
        // =====================================================
        public ActionResult Index(string filter = "All")
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Tickets.AsQueryable();

                if (filter == "Open")
                {
                    query = query.Where(t => t.Status == "Open");
                }
                else if (filter == "InProgress")
                {
                    query = query.Where(t => t.Status == "InProgress");
                }
                else if (filter == "Closed")
                {
                    query = query.Where(t => t.Status == "Closed");
                }

                var tickets = query
                    .OrderByDescending(t => t.CreatedAt)
                    .ToList();

                ViewBag.CurrentFilter = filter;

                return View(tickets);
            }
        }


        // =====================================================
        // DETAILS
        // Loads ONLY latest 50 messages for performance
        // Older messages loaded via AJAX infinite scroll
        // =====================================================
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            using (var context = new ApplicationDbContext())
            {
                // get ticket info
                var ticket = context.Tickets
                    .FirstOrDefault(t => t.TicketId == id.Value);

                if (ticket == null)
                {
                    return RedirectToAction("Index");
                }

                // =====================================================
                // LOAD LAST 50 MESSAGES ONLY
                // =====================================================
                var messages = context.TicketMessages
                    .Where(m => m.TicketId == id.Value)

                    // newest first
                    .OrderByDescending(m => m.TicketMessageId)

                    // take latest 50
                    .Take(50)

                    // restore correct order
                    .OrderBy(m => m.TicketMessageId)

                    .ToList();


                var viewModel = new StaffTicketDetailsViewModel
                {
                    Ticket = ticket,
                    Messages = messages
                };

                return View(viewModel);
            }
        }


        // =====================================================
        // HISTORY PAGE
        // =====================================================
        public ActionResult History(string search = "")
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Tickets.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim();

                    query = query.Where(t =>

                        t.TicketId.ToString().Contains(search)

                        ||

                        t.PatientId.Contains(search)

                        ||

                        context.TicketMessages
                            .Any(m =>
                                m.TicketId == t.TicketId &&
                                m.MessageText.Contains(search)
                            )
                    );
                }

                var tickets = query
                    .OrderByDescending(t => t.CreatedAt)
                    .ToList();

                ViewBag.Search = search;

                return View(tickets);
            }
        }


        // =====================================================
        // REPLY
        // =====================================================
        [HttpPost]
        public async Task<ActionResult> Reply(
            int? TicketId,
            string MessageText,
            string NewStatus,
            string NewPriority)
        {
            if (TicketId == null)
            {
                return RedirectToAction("Index");
            }

            using (var context = new ApplicationDbContext())
            {
                var ticketService = new TicketService(context);
                var patientService = new PatientService(context);
                var telegramService = new TelegramService();

                var ticket = ticketService.GetTicketById(TicketId.Value);

                if (ticket == null)
                {
                    return RedirectToAction("Index");
                }

                await ticketService.SaveMessageAsync(
                    TicketId.Value,
                    "Staff",
                    MessageText
                );

                await ticketService.UpdateTicketStatusAsync(
                    TicketId.Value,
                    NewStatus
                );

                if (!string.IsNullOrWhiteSpace(NewPriority))
                {
                    ticket.Priority = NewPriority;
                    await context.SaveChangesAsync();
                }

                var patient = patientService
                    .GetByPatientId(ticket.PatientId);

                if (patient?.TelegramUserId != null)
                {
                    await telegramService.SendMessageAsync(
                        patient.TelegramUserId.Value,
                        "Doctor Reply:\n\n" + MessageText
                    );
                }
            }

            return RedirectToAction(
                "Details",
                new { id = TicketId.Value }
            );
        }


        // =====================================================
        // NEW — AJAX MESSAGE ENDPOINT
        // Safe read-only endpoint
        //
        // URL:
        // /Staff/GetMessages?ticketId=1
        //
        // Returns JSON messages
        // =====================================================

        // =====================================================
        // GET NEW MESSAGES AFTER LAST ID
        // Used for live chat update
        //
        // Example:
        // /Staff/GetMessages?ticketId=1&lastMessageId=50
        //
        // Returns only newer messages
        // =====================================================
        [HttpGet]
        public JsonResult GetMessages(int ticketId, int lastMessageId = 0)
        {
            using (var context = new ApplicationDbContext())
            {
                var messages = context.TicketMessages
                    .Where(m =>
                        m.TicketId == ticketId &&
                        m.TicketMessageId > lastMessageId
                    )
                    .OrderBy(m => m.TicketMessageId)
                    .Select(m => new
                    {
                        m.TicketMessageId,
                        m.SenderType,
                        m.MessageText,
                        m.SentAt
                    })
                    .ToList();

                return Json(
                    messages,
                    JsonRequestBehavior.AllowGet
                );
            }
        }


        // =====================================================
        // GET OLDER MESSAGES BEFORE GIVEN ID
        // Used for infinite upward scroll
        // =====================================================
        public JsonResult GetOlderMessages(int ticketId, int firstMessageId)
        {
            using (var context = new ApplicationDbContext())
            {
                var messages = context.TicketMessages
                    .Where(m =>
                        m.TicketId == ticketId &&
                        m.TicketMessageId < firstMessageId
                    )
                    .OrderByDescending(m => m.TicketMessageId)
                    .Take(50)
                    .Select(m => new
                    {
                        m.TicketMessageId,
                        m.SenderType,
                        m.MessageText,
                        m.SentAt
                    })
                    .ToList();

                messages.Reverse();

                return Json(messages,
                    JsonRequestBehavior.AllowGet);
            }
        }

    

    // =====================================================
// ANNOUNCEMENT PAGE
// =====================================================
public ActionResult Announcement()
        {
            using (var context = new ApplicationDbContext())
            {
                var history = context.Announcements
                    .OrderByDescending(a => a.AnnouncementId)
                    .Take(20)
                    .ToList();

                return View(history);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SendAnnouncement(string Message)
        {
            if (string.IsNullOrWhiteSpace(Message))
                return RedirectToAction("Announcement");

            using (var context = new ApplicationDbContext())
            {
                var telegramService = new TelegramService();

                // get all active patients who have a TelegramUserId
                var patients = context.Patients
                    .Where(p => p.IsActive && p.TelegramUserId != null)
                    .ToList();

                int sent = 0;
                int failed = 0;

                foreach (var patient in patients)
                {
                    try
                    {
                        await telegramService.SendMessageAsync(
                            patient.TelegramUserId.Value,
                            "📢 Hospital Announcement:\n\n" + Message
                        );
                        sent++;
                    }
                    catch
                    {
                        failed++;
                    }
                }

                // save to database
                context.Announcements.Add(new Announcement
                {
                    Message = Message,
                    SentAt = DateTime.UtcNow,
                    TotalSent = sent,
                    TotalFailed = failed
                });

                await context.SaveChangesAsync();

                TempData["Result"] = $"✅ Sent: {sent} | ❌ Failed: {failed}";

                return RedirectToAction("Announcement");
            }
        }
    } }