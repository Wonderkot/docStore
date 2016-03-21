using System.Web.Mvc;

namespace DocStore.Controllers
{
    public class DocumentController : Controller
    {
        // GET: Document
        public ActionResult Index()
        {
            return View("Documents");
        }
    }
}