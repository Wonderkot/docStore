using System;
using System.Collections.Generic;
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
            if (UserManager.CheckUser(ref user, out errMsg))
            {
                return View("Menu", user);
            }
            //если возникли ошибки, то пишем о них
            Log.Error($"При попытке авторизации возникла ошибка: {errMsg}");
            ModelState.AddModelError("error", errMsg);
            return View();
        }

        public PartialViewResult EditUser()
        {
            string errMsg;
            List<Role> roles = UserManager.GetRoles(out errMsg);
            ModelState.AddModelError("error", errMsg);
            ViewBag.Roles = roles;
            return PartialView("UserEdit");
        }

        [HttpPost]
        public PartialViewResult AddUser(User user)
        {
            string msg;
            UserManager.AddUser(user, out msg);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                ModelState.AddModelError("error", msg);
            }
            return PartialView("UserEdit");
        }

    }
}