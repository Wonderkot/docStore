using System;
using System.Web;
using NHibernate;
using NHibernate.Cfg;

namespace DocStore.Models.Nhibernate
{
    public class NHibertnateSession
    {
        public static ISession OpenSession()
        {
            ISession openSession = null;
            try
            {
                var configuration = new Configuration();
                var configurationPath = HttpContext.Current.Server.MapPath(@"~\Models\Nhibernate\hibernate.cfg.xml");
                configuration.Configure(configurationPath);
                var userConfigFile = HttpContext.Current.Server.MapPath(@"~\Models\Nhibernate\User.hbm.xml");
                configuration.AddFile(userConfigFile);

                var roleConfigFile = HttpContext.Current.Server.MapPath(@"~\Models\Nhibernate\Role.hbm.xml");
                configuration.AddFile(roleConfigFile);

                ISessionFactory sessionFactory = configuration.BuildSessionFactory();
                openSession = sessionFactory.OpenSession();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return openSession;
        }
    }
}