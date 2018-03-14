using ContactTrackerLib.Database;
using ContractTrackerInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using System.Threading;

namespace ContractTrackerLib_t
{
    [TestClass]
    public class ContactDB_t
    {
        [TestMethod]
        public void TestEmptyDB()
        {
            var db = new ContactDB();
            int count = 0;
            db.UpdateStream
                .Subscribe(c => count++);
            db.ShutDown();

            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void EmptyDBWithSource()
        {
            var db = new ContactDB();
            int count = 0;
            db.Add(new dummyStore());

            db.UpdateStream
                .Subscribe(c => count++);
            db.ShutDown();

            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void AddEmptyStoreLater()
        {
            var db = new ContactDB();
            int count = 0;

            db.UpdateStream
                .Subscribe(c => count++);
            db.Add(new dummyStore());
            db.ShutDown();

            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void OneContactSubscribeAfterAdded()
        {
            var db = new ContactDB();
            var store = new dummyStore();
            db.Add(store);
            store.AddDummyContacts(1);

            Thread.Sleep(10);

            int count = 0;
            db.UpdateStream
                .Subscribe(c => count++);
            db.ShutDown();

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void OneContactSubscribeBeforeAdded()
        {
            var db = new ContactDB();
            var store = new dummyStore();
            db.Add(store);

            int count = 0;
            db.UpdateStream
                .Subscribe(c => count++);
            store.AddDummyContacts(1);
            db.ShutDown();

            Assert.AreEqual(1, count);
        }

    }

    internal static class DBUtils
    {
        public static void AddDummyContacts (this IContactStore source, int number)
        {
            foreach (var index in Enumerable.Range(1, number))
            {
                source.Add(new dummyContact(index));
            }
        }
    }

    /// <summary>
    /// A dummmy contact
    /// </summary>
    internal class dummyContact : IContact
    {
        private int index;

        public dummyContact(int index)
        {
            this.index = index;
        }

        public string FirstName => throw new NotImplementedException();

        public string LastName => throw new NotImplementedException();

        public string UniqueID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    internal class dummyStore : IContactStore
    {
        private ImmutableList<IContact> _mylist = ImmutableList<IContact>.Empty;
        private Subject<ContactDB.UpdateInfo> _sender = new Subject<ContactDB.UpdateInfo>();

        public IObservable<ContactDB.UpdateInfo> ContactUpdateStream => _sender;

        public void Add(IContact contactToAdd)
        {
            var info = new ContactDB.UpdateInfo()
            {
                _contacts = new[] { contactToAdd },
                _reason = ContactDB.UpdateReason.Add
            };
            _sender.OnNext(info);
            _mylist = _mylist.Add(contactToAdd);
        }
    }
}
