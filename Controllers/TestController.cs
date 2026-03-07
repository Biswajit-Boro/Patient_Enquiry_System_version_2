using System.Web.Http;

namespace Patient_Enquiry_System_version_2.Controllers
{
    [RoutePrefix("api/test")]
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok("Web API Working");
        }
    }
}
