using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Patient_Enquiry_System_version_2.Data;

namespace Patient_Enquiry_System_version_2.Controllers
{
    [RoutePrefix("api/admin")]
    public class AdminController : ApiController
    {
        [HttpGet]
        [Route("enquiries/pending")]
        public async Task<IHttpActionResult> GetPendingEnquiries()
        {
            using (var context = new ApplicationDbContext())
            {
                var enquiries = context.Enquiries
                    .Where(e => e.Status == "Pending")
                    .OrderByDescending(e => e.CreatedAt)
                    .ToList();

                return Ok(enquiries);
            }
        }

        [HttpPut]
        [Route("enquiries/{id}/start")]
        public async Task<IHttpActionResult> MarkInProgress(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var enquiry = context.Enquiries.FirstOrDefault(e => e.Id == id);

                if (enquiry == null)
                    return NotFound();

                enquiry.Status = "InProgress";
                await context.SaveChangesAsync();

                return Ok("Enquiry marked as InProgress.");
            }
        }

        [HttpPut]
        [Route("enquiries/{id}/close")]
        public async Task<IHttpActionResult> CloseEnquiry(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var enquiry = context.Enquiries.FirstOrDefault(e => e.Id == id);

                if (enquiry == null)
                    return NotFound();

                enquiry.Status = "Closed";
                await context.SaveChangesAsync();

                return Ok("Enquiry closed successfully.");
            }
        }
    }
}
