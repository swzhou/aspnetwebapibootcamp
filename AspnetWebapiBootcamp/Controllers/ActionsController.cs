using System.Web.Http;

namespace AspnetWebapiBootcamp.Controllers
{
    public class ActionsController: ApiController
    {
        [AcceptVerbs("PUT", "DELETE")]
        public string AvailableActions()
        {
            return "all";
        }

        [HttpGet]
        public string Details(int id)
        {
            return string.Format("Details of {0}", id);
        }

        [HttpGet]
        [ActionName("name")]
        public string GetName(int id)
        {
            return string.Format("Name of {0} is: Name.", id);
        }

        [NonAction]
        public string PrivateData()
        {
            return "private data";
        }
    }
}