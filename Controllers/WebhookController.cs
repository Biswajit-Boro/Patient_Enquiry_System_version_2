using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Configuration;
using Newtonsoft.Json;
using Patient_Enquiry_System_version_2.Data;
using Patient_Enquiry_System_version_2.Models;
using Patient_Enquiry_System_version_2.Services;

namespace Patient_Enquiry_System_version_2.Controllers
{
    [RoutePrefix("api/webhook")]
    public class WebhookController : ApiController
    {
        [HttpPost]
        [Route("telegram")]
        public async Task<IHttpActionResult> Telegram([FromBody] TelegramUpdate update)
        {
            try
            {
                var expectedSecret = ConfigurationManager.AppSettings["TelegramWebhookSecret"];

                if (!Request.Headers.Contains("X-Telegram-Bot-Api-Secret-Token"))
                    return Unauthorized();

                var receivedSecret = Request.Headers
                    .GetValues("X-Telegram-Bot-Api-Secret-Token")
                    .FirstOrDefault();

                if (receivedSecret != expectedSecret)
                    return Unauthorized();

                if (update?.message?.from == null)
                    return Ok();

                using (var context = new ApplicationDbContext())
                {
                    context.WebhookLogs.Add(new WebhookLog
                    {
                        RawJson = JsonConvert.SerializeObject(update),
                        ReceivedAt = DateTime.UtcNow
                    });

                    await context.SaveChangesAsync();

                    var idempotencyService = new IdempotencyService(context);

                    if (await idempotencyService.IsProcessedAsync(update.update_id))
                        return Ok();

                    var patientService = new PatientService(context);
                    var telegramService = new TelegramService();
                    var ticketService = new TicketService(context);

                    var telegramUserId = update.message.from.id;
                    var messageText = update.message.text?.Trim();

                    var existingPatient = await patientService
                        .GetPatientByTelegramUserIdAsync(telegramUserId);

                    // =========================
                    // 🔴 NOT AUTHENTICATED
                    // =========================
                    if (existingPatient == null)
                    {
                        if (string.IsNullOrWhiteSpace(messageText))
                        {
                            await telegramService.SendMessageAsync(
                                telegramUserId,
                                "Please enter your Patient ID (Example: PID-1001)."
                            );
                        }
                        else
                        {
                            var patient = await patientService
                                .GetPatientByPatientIdAsync(messageText);

                            if (patient == null)
                            {
                                await telegramService.SendMessageAsync(
                                    telegramUserId,
                                    "Invalid Patient ID. Please try again (Example: PID-1001)."
                                );
                            }
                            else
                            {
                                await patientService
                                    .BindTelegramUserAsync(patient, telegramUserId);

                                await telegramService.SendMessageAsync(
                                    telegramUserId,
                                    $"Welcome {patient.Name}. You are now authenticated. Please describe your issue."
                                );
                            }
                        }

                        await idempotencyService.MarkAsProcessedAsync(update.update_id);
                        return Ok();
                    }

                    // =========================
                    // 🟢 AUTHENTICATED
                    // =========================

                    // ── FILE HANDLING ──
                    string fileId = null;
                    string fileType = null;

                    if (update.message.photo != null && update.message.photo.Length > 0)
                    {
                        // take highest quality photo
                        fileId = update.message.photo[update.message.photo.Length - 1].file_id;
                        fileType = "image";
                    }
                    else if (update.message.document != null)
                    {
                        var mimeType = update.message.document.mime_type ?? "";
                        var fileName = update.message.document.file_name ?? "";

                        bool isPdf = mimeType == "application/pdf" || fileName.EndsWith(".pdf");
                        bool isImage = mimeType.StartsWith("image/");

                        if (isPdf || isImage)
                        {
                            fileId = update.message.document.file_id;
                            fileType = isPdf ? "pdf" : "image";
                        }
                        else
                        {
                            // unsupported file type
                            await telegramService.SendMessageAsync(
                                telegramUserId,
                                "⚠️ Unsupported file type.\n\nPlease send only:\n📷 Images (JPG, PNG)\n📄 PDF files\n🧪 Lab reports (as PDF or image)"
                            );
                            await idempotencyService.MarkAsProcessedAsync(update.update_id);
                            return Ok();
                        }
                    }

                    // ── SKIP IF NO TEXT AND NO FILE ──
                    if (string.IsNullOrWhiteSpace(messageText) && fileId == null)
                    {
                        await idempotencyService.MarkAsProcessedAsync(update.update_id);
                        return Ok();
                    }

                    // ── GET OR CREATE TICKET ──
                    var activeTicket = ticketService
                        .GetActiveTicket(existingPatient.PatientId);

                    if (activeTicket == null)
                    {
                        activeTicket = await ticketService
                            .CreateTicketAsync(existingPatient.PatientId);

                        await telegramService.SendMessageAsync(
                            telegramUserId,
                            "Your ticket has been created. Our medical team will respond shortly."
                        );
                    }

                    // ── SAVE FILE IF PRESENT ──
                    if (fileId != null)
                    {
                        var savePath = System.Web.Hosting.HostingEnvironment
                            .MapPath("~/uploads/files/");

                        if (!System.IO.Directory.Exists(savePath))
                            System.IO.Directory.CreateDirectory(savePath);

                        var fileUrl = await telegramService
                            .DownloadFileAsync(fileId, savePath);

                        await ticketService.SaveMessageAsync(
                            activeTicket.TicketId,
                            "Patient",
                            messageText ?? $"[{fileType} file]",
                            fileUrl,
                            fileType
                        );

                        await telegramService.SendMessageAsync(
                            telegramUserId,
                            "✅ File received successfully. Our team will review it."
                        );
                    }
                    // ── SAVE TEXT MESSAGE ──
                    else
                    {
                        await ticketService.SaveMessageAsync(
                            activeTicket.TicketId,
                            "Patient",
                            messageText
                        );
                    }

                    await idempotencyService.MarkAsProcessedAsync(update.update_id);
                }

                return Ok();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}