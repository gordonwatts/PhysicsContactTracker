using System;
using System.Threading.Tasks;
using InSpireHEPAccess;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InSpireHEPAccess_t
{
    [TestClass]
    public class InSpireContactFinder_t
    {
        [TestMethod]
        public async Task InSpireFindGoodURL()
        {
            var finder = new InSpireContactFinder();
            var info = (await finder.FindContactAsync(new Uri("http://inspirehep.net/record/983968?ln=en")))
                .ToArray();
            Assert.AreEqual(1, info.Length);
            Assert.AreEqual("Gordon Thomas", info[0].FirstName);
            Assert.AreEqual("Watts", info[0].LastName);
        }
    }
}
