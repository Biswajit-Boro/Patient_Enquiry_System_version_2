using System.Web.Http;

namespace Patient_Enquiry_System_version_2.Controllers
{
    [RoutePrefix("api/health")]
    public class HealthController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(new { status = "running" });
        }
    }
}
