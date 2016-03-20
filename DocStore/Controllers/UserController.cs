using System;
using System.Web.Mvc;
using DocStore.Models;
using DocStore.Models.Manage;

namespace DocStore.Controllers
{
    public class UserController : Controller
    {
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
        // GET: User
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Login(User user)
        {
            try
            {
                if (user == null) throw new ArgumentNullException(nameof(user));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error", e.Message);
                Log.Error(e);
                return View();
            }
            Log.Debug($"Пользователь {user.Name} пытается войти в приложение.");
            if (!ModelState.IsValid) return View();
            string errMsg;
            if (UserManager.CheckUser(user, out errMsg))
            {
                return View("Documents");
            }
            //если возникли ошибки, то пишем о них
            Log.Error($"При попытке авторизации возникла ошибка: {errMsg}");
            ModelState.AddModelError("error", errMsg);
            return View();
        }

    }
}