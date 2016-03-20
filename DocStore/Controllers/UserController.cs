using System;
using System.Text;
using System.Web.Mvc;
using DocStore.Models;
using DocStore.Models.Nhibernate;
using NHibernate.Criterion;

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
            try
            {
                if (user == null) throw new ArgumentNullException(nameof(user));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error", e.Message);
                Console.WriteLine(e);
                return View();
            }
            if (!ModelState.IsValid) return View();
            var session = NHibertnateSession.OpenSession();
            if (session == null)
            {
                ModelState.AddModelError("error", "Ошибка подключения к базе данных");
                return View();
            }
            User userIndDb;
            try
            {
                var userCrit = session.CreateCriteria<User>();
                userCrit.Add(Restrictions.Eq("Name", user.Name));
                userIndDb = userCrit.UniqueResult<User>();
                if (userIndDb == null)
                {
                    throw new Exception($"Пользователь с именем {user.Name} не найден в базе.");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error", e.Message);
                Console.WriteLine(e);
                return View();
            }

            var computedHash = ComputeHash(user);


            if (computedHash.Equals(userIndDb.Password))
            {
                return View("Documents");
            }
            ModelState.AddModelError("error", "Пользователь не найден.");
            return View();
        }

        private string ComputeHash(User user)
        {
            byte[] hash;
            using (var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider())
                hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
            var sb = new StringBuilder();
            foreach (byte b in hash) sb.AppendFormat("{0:x2}", b);
            string computedHash = sb.ToString();
            return computedHash;
        }
    }
}