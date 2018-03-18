using ContractTrackerInterfaces;
using InSpireHEPAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reactive.Subjects;

namespace InSpireHEPAccess_t
{
    [TestClass]
    public class InSpireContactStore_t
    {
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

        /// <summary>
        /// Create a simple contact
        /// </summary>
        /// <returns></returns>
        private IContact GetInspireSimpleContact() => new InSpireContact() { FirstName = "John", InspireRecordID = 2381831, LastName = "Doe", UniqueID = "1234566" };
    }
}
