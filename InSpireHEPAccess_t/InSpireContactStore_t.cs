using ContractTrackerInterfaces;
using InSpireHEPAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Util_t;
using Utils;

namespace InSpireHEPAccess_t
{
    [TestClass]
    public class InSpireContactStore_t
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
        public void CreateInspireContactStore()
        {
            int count = 0;
            bool finished = false;
            using (var cs = new InSpireContactStore())
            {
                cs.ContactUpdateStream.Subscribe(
                    update => count++,
                    () => finished = true
                    );
            }
            Assert.AreEqual(0, count);
            Assert.IsTrue(finished, "Expected to see the stream closed when the item was disposed.");
        }

        [TestMethod]
        public void InspireNewItemBeforeAttached()
        {
            int count = 0;
            using (var cs = new InSpireContactStore())
            {
                cs.Add(GetInspireSimpleContact());

                // Check that we have a single contact.
                cs.ContactUpdateStream.Subscribe(u => count++);
            }
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void InspireNewItemAfterAttached()
        {
            int count = 0;
            using (var cs = new InSpireContactStore())
            {
                // Check that we have a single contact.
                cs.ContactUpdateStream.Subscribe(u => count++);

                // Add after.
                cs.Add(GetInspireSimpleContact());

            }
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        [DeploymentItem("1020448.json")]
        public async Task UpdateNoChange()
        {
            // Setup the first and second request.
            var f = new FileInfo("1020448.json");
            AutoWebAccess.AddUriResponse("http://inspirehep.net:80/record/1020448?ln=en&of=recjson", f);

            int count = 0;
            using (var cs = new InSpireContactStore())
            {
                // Watch for updates. Since no change, we should see only one.
                cs.ContactUpdateStream.Subscribe(u =>
                {
                    count++;
                });

                // Add.
                var c = GetInspireSimpleContact(f) as InSpireContact;
                cs.Add(c);

                // Trigger the update.
                await cs.Update();
            }
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        [DeploymentItem("1020448.json")]
        public async Task UpdateFirstName()
        {
            // Setup the first and second request.
            var f = new FileInfo("1020448.json");
            AutoWebAccess.AddUriResponse("http://inspirehep.net:80/record/1020448?ln=en&of=recjson", f);

            int count = 0;
            int updates = 0;
            using (var cs = new InSpireContactStore())
            {
                // Watch for updates. Since no change, we should see only one.
                cs.ContactUpdateStream.Subscribe(u =>
                {
                    count++;
                    if (u._reason == ContactTrackerLib.Database.ContactDB.UpdateReason.Update)
                    {
                        Assert.AreEqual("First Name", u._updateReasonText);
                        updates++;
                    }
                });

                // Add.
                var c = GetInspireSimpleContact(f) as InSpireContact;
                c.FirstName = "Mable";
                cs.Add(c);

                // Trigger the update.
                await cs.Update();
            }
            Assert.AreEqual(2, count);
            Assert.AreEqual(1, updates);
        }

        [TestMethod]
        [DeploymentItem("1020448.json")]
        public async Task UpdateLastName()
        {
            // Setup the first and second request.
            var f = new FileInfo("1020448.json");
            AutoWebAccess.AddUriResponse("http://inspirehep.net:80/record/1020448?ln=en&of=recjson", f);

            int count = 0;
            int updates = 0;
            using (var cs = new InSpireContactStore())
            {
                // Watch for updates. Since no change, we should see only one.
                cs.ContactUpdateStream.Subscribe(u =>
                {
                    count++;
                    if (u._reason == ContactTrackerLib.Database.ContactDB.UpdateReason.Update)
                    {
                        Assert.AreEqual("Last Name", u._updateReasonText);
                        updates++;
                    }
                });

                // Add.
                var c = GetInspireSimpleContact(f) as InSpireContact;
                c.LastName = "Mable";
                cs.Add(c);

                // Trigger the update.
                await cs.Update();
            }
            Assert.AreEqual(2, count);
            Assert.AreEqual(1, updates);
        }

        /// <summary>
        /// Create a simple contact
        /// </summary>
        /// <returns></returns>
        private IContact GetInspireSimpleContact() => new InSpireContact() { FirstName = "John", InspireRecordID = 2381831, LastName = "Doe", UniqueID = "1234566" };

        /// <summary>
        /// Load a contact from json
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private IContact GetInspireSimpleContact(FileInfo f) {
            var jsonData = File.ReadAllText(f.FullName);
            var contacts = InspireContactAccess.BuildContactListFromJSON($"File {f.Name}", jsonData).ToArray();
            if (contacts.Length != 1)
            {
                throw new InvalidOperationException($"file {f.Name} contains {contacts.Length} contacts - must contain exactly one (test error!!)");
            }
            return contacts[0];
        }
    }
}
