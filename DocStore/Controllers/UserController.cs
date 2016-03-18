using System.Web.Mvc;
using DocStore.Models;

namespace DocStore.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Login(User user)
        {
            //пока просто сравниваем имя
            if (ModelState.IsValid)
            {
                if (user.Name.Equals("admin"))
                {
                    return View("Documents");
                }
                ModelState.AddModelError("error", "Пользователь не найден.");
                return View();
            }
            return View();
        }
    }
}