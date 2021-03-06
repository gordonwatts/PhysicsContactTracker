﻿using System;
using System.Threading.Tasks;
using InSpireHEPAccess;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils;
using Util_t;
using System.IO;

namespace InSpireHEPAccess_t
{
    [TestClass]
    public class InSpireContactFinder_t
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFComposer.Reset();
            AutoWebAccess.Reset();
            MEFComposer.AddObject(typeof(AutoWebAccess));
        }

        [TestCleanup]
        public void TestClean()
        {
            MEFComposer.Reset();
            AutoWebAccess.Reset();
        }

        [TestMethod]
        [DeploymentItem("983968.json")]
        public async Task InSpireFindGoodURL()
        {
            AutoWebAccess.AddUriResponse("http://inspirehep.net:80/record/983968?ln=en&of=recjson", new FileInfo("983968.json"));

            var finder = new InSpireContactFinder();
            var info = (await finder.FindContactAsync(new Uri("http://inspirehep.net/record/983968?ln=en")))
                .ToArray();
            Assert.AreEqual(1, info.Length);
            Assert.AreEqual("Gordon Thomas", info[0].FirstName);
            Assert.AreEqual("Watts", info[0].LastName);
        }

        [TestMethod]
        [DeploymentItem("1024481.json")]
        public async Task InSpireFindGoodURLDC()
        {
            AutoWebAccess.AddUriResponse("http://inspirehep.net:80/record/1024481?ln=en&of=recjson", new FileInfo("1024481.json"));

            var finder = new InSpireContactFinder();
            var info = (await finder.FindContactAsync(new Uri("http://inspirehep.net/record/1024481?ln=en")))
                .ToArray();
            Assert.AreEqual(1, info.Length);
            Assert.AreEqual("David", info[0].FirstName);
            Assert.AreEqual("Curtin", info[0].LastName);
        }

        [TestMethod]
        [DeploymentItem("1020448.json")]
        public async Task InSpireFindGoodURLDW()
        {
            AutoWebAccess.AddUriResponse("http://inspirehep.net:80/record/1020448?ln=en&of=recjson", new FileInfo("1020448.json"));

            var finder = new InSpireContactFinder();
            var info = (await finder.FindContactAsync(new Uri("http://inspirehep.net/record/1020448?ln=en")))
                .ToArray();
            Assert.AreEqual(1, info.Length);
            Assert.AreEqual("Daniel O.", info[0].FirstName);
            Assert.AreEqual("Whiteson", info[0].LastName);
        }

        [TestMethod]
        [ExpectedException(typeof(BadInSpireUrlException))]
        public async Task FindBadUrlContact()
        {
            var f = new InSpireContactFinder();
            var info = (await f.FindContactAsync(new Uri("https://www.nytimes.com")))
                .ToArray();
        }

        [TestMethod]
        [ExpectedException(typeof(BadInSpireUrlException))]
        public async Task FindBadUrlGoodOtherwise()
        {
            var f = new InSpireContactFinder();
            var info = (await f.FindContactAsync(new Uri("http://inspirehep.net/search?ln=en&p=find+j+%22Phys.Rev.Lett.%2C105%2A%22")))
                .ToArray();
        }

        [TestMethod]
        [DeploymentItem("1518295.json")]
        [ExpectedException(typeof(BadInSpireUrlException))]
        public async Task FindPaperUrlAsName()
        {
            AutoWebAccess.AddUriResponse("http://inspirehep.net:80/record/1518295?of=recjson", new FileInfo("1518295.json"));

            // A paper can't be re-read as an actual person just yet.
            var f = new InSpireContactFinder();
            var info = (await f.FindContactAsync(new Uri("http://inspirehep.net/record/1518295")))
                .ToArray();
        }
    }
}
