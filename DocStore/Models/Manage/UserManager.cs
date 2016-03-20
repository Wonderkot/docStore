using System;
using System.Text;
using DocStore.Models.Nhibernate;
using NHibernate;
using NHibernate.Criterion;

namespace DocStore.Models.Manage
{
    /// <summary>
    /// Класс для управления пользователями
    /// </summary>
    public class UserManager
    {
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Проверка существования пользователя
        /// </summary>
        /// <param name="user">Проверяемый пользователь</param>
        /// <seealso cref="User"/>
        /// <param name="msg">Сообщение об ошибке</param>
        /// <returns></returns>
        public static bool CheckUser(User user, out string msg)
        {
            msg = string.Empty;
            var session = NHibertnateSession.OpenSession();
            if (session == null)
            {
                msg = "Ошибка подключения к базе данных";
                return false;
            }
            User userInDb;
            try
            {
                userInDb = SearchUser(user, session);
                if (userInDb == null)
                {
                    throw new Exception($"Пользователь с именем {user.Name} не найден в базе.");
                }
                Log.Info($"Пользователь {user.Name} найден в базе, необходимо выполнить проверку пароля.");
            }
            catch (Exception e)
            {
                Log.Error(e);
                msg = e.Message;
                return false;
            }
            finally
            {
                Log.Debug("Выполняется закрытие sql-сессии");
                session.Close();
                Log.Debug("Закрытие sql-сессии завершено");
            }
            Log.Debug("Производится вычисление хэша введенного пароля");
            var computedHash = ComputeHash(user);
            Log.Debug($"Полученное значение хэша:{computedHash}");

            if (computedHash.Equals(userInDb.Password))
            {
                if (userInDb.Role != null)
                    Log.Info($"Пользователь {userInDb.Name} может работать с документами. Роль пользователя: {userInDb.Role.Name}");
                return true;
            }
            msg = "Неверный пароль.";
            return false;
        }

        public static bool AddUser(User user, out string msg)
        {
            msg = string.Empty;
            var session = NHibertnateSession.OpenSession();
            if (session == null)
            {
                msg = "Ошибка подключения к базе данных";
                return false;
            }
            try
            {
                var userInDb = SearchUser(user, session);
                if (userInDb != null)
                {
                    throw new Exception($"Пользователь с именем {user.Name} уже существует в базе.");
                }
                Log.Debug("Перед добавлением в базу необходимо расчитать хэш.");
                var hash = ComputeHash(user);
                user.Password = hash;
                using (var tx = session.BeginTransaction())
                {
                    Log.Debug("Выполняется добавление записи о новом пользователе.");
                    session.Save(user);
                    tx.Commit();
                }
                Log.Debug("Добавление записи успешно завершено.");

            }
            catch (Exception e)
            {
                Log.Error(e);
                msg = "Возникла ошибка при добавлении пользователя.";
                return false;
            }
            finally
            {
                Log.Debug("Выполняется закрытие sql-сессии");
                session.Close();
                Log.Debug("Закрытие sql-сессии завершено");
            }
            return true;
        }

        public static bool DeleteUser(User user, out string msg)
        {
            msg = string.Empty;
            var session = NHibertnateSession.OpenSession();
            if (session == null)
            {
                msg = "Ошибка подключения к базе данных";
                return false;
            }
            try
            {
                var userInDb = SearchUser(user, session);
                if (userInDb == null)
                {
                    throw new Exception($"Пользователь с именем {user.Name} не найден в базе, удаление невозможно.");
                }
                using (var tx = session.BeginTransaction())
                {
                    Log.Debug("Выполняется удаление записи о пользователе.");
                    session.Delete(user);
                    tx.Commit();
                }
                Log.Debug("Удаление записи успешно завершено.");

            }
            catch (Exception e)
            {
                Log.Error(e);
                msg = "Возникла ошибка при удалении пользователя.";
                return false;
            }
            finally
            {
                Log.Debug("Выполняется закрытие sql-сессии");
                session.Close();
                Log.Debug("Закрытие sql-сессии завершено");
            }
            return true;
        }

        /// <summary>
        /// Поиск пользователя в БД
        /// </summary>
        /// <param name="user">Искомый пользователь</param>
        /// <param name="session">SQL-сессия</param>
        /// <returns>Найденный пользователь</returns>
        private static User SearchUser(User user, ISession session)
        {
            var userCrit = session.CreateCriteria<User>();
            Log.Info($"Выполняется поиск пользователя {user.Name} в базе.");
            userCrit.Add(Restrictions.Eq("Name", user.Name));
            var userIndDb = userCrit.UniqueResult<User>();
            return userIndDb;
        }

        /// <summary>
        /// Вычисление хэша пароля, который ввел пользователь
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private static string ComputeHash(User user)
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