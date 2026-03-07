using System.Web;
using System.Web.Mvc;

namespace Patient_Enquiry_System_version_2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
