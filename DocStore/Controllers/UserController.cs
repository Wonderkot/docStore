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
            var session = NHibertnateSession.OpenSession();
            if (session == null)
            {
                var errorMessage = "Ошибка подключения к базе данных";
                Log.Error(errorMessage);
                ModelState.AddModelError("error", errorMessage);
                return View();
            }
            User userIndDb;
            try
            {
                var userCrit = session.CreateCriteria<User>();
                Log.Info($"Выполняется поиск пользователя {user.Name} в базе.");
                userCrit.Add(Restrictions.Eq("Name", user.Name));
                userIndDb = userCrit.UniqueResult<User>();
                if (userIndDb == null)
                {
                    throw new Exception($"Пользователь с именем {user.Name} не найден в базе.");
                }
                Log.Info($"Пользователь {user.Name} найден в базе, необходимо выполнить проверку пароля.");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error", e.Message);
                Log.Error(e);
                return View();
            }
            finally
            {
                Log.Debug("Выполняется закрытие sql-сессии");
                session.Close();
                Log.Debug("Закрытие sql-сессии заверншено");
            }

            Log.Debug("Производится вычисление хэша введенного пароля");
            var computedHash = ComputeHash(user);
            Log.Debug($"Полученное значение хэша:{computedHash}");

            if (computedHash.Equals(userIndDb.Password))
            {
                Log.Info($"Пользователь {userIndDb.Name} может работать с документами. Роль пользователя: {userIndDb.Role.Name}");
                return View("Documents");
            }
            var message = "Неверный пароль.";
            Log.Error(message);
            ModelState.AddModelError("error", message);
            return View();
        }

        /// <summary>
        /// Вычисление хша пароля, который ввел пользователь
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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