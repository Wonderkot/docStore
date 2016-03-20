using DocStore.Models.Nhibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;

namespace DocStoreDocStoreTests.Models.Nhibernate
{
    [TestClass()]
    public class NHibertnateSessionTests
    {
        [TestMethod()]
        public void OpenSessionTest()
        {
            ISession session = NHibertnateSession.OpenSession();
            Assert.IsNotNull(session);
        }
    }
}