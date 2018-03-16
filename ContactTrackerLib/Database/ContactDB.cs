using ContractTrackerInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ContactTrackerLib.Database
{
    /// <summary>
    /// Main database for contacts. Manages and keeps them all in sync, and has interfaces
    /// to save them, etc.
    /// </summary>
    public class ContactDB : IDisposable
    {
        /// <summary>
        /// Initalize the object and the internal logic for new contacts
        /// </summary>
        public ContactDB()
        {
            // Each time a new contact comes in from any contact store, we need to update and maintain our local
            // contact list.
            _ContactStoreSubscriptions.Add(_contactStoreStream
                .Subscribe(c => UpdateLocalStore(c)));
        }

        /// <summary>
        /// Track our local store of contacts
        /// </summary>
        private ImmutableList<IContact> _localContactStore = ImmutableList<IContact>.Empty;

        /// <summary>
        /// A new contact has come in from one of the underlying stores. Update our local database of contacts
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private void UpdateLocalStore(UpdateInfo c)
        {
            switch (c._reason)
            {
                case UpdateReason.Add:
                    _localContactStore = _localContactStore.AddRange(c._contacts);
                    break;

                case UpdateReason.Remove:
                    _localContactStore = _localContactStore.RemoveRange(c._contacts);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Our prefered storage
        /// </summary>
        private IContactStore _preferedStore = null;

        /// <summary>
        /// Add a contact to the database.
        /// </summary>
        /// <remarks>Do not alter once handed off</remarks>
        /// <param name="contactToAdd">The contact to add. Don't modify once handed off.</param>
        /// <param name="preferedStorage">Where to store it. If null, stored in default spot</param>
        public void Add (IContact contactToAdd, IContactStore preferedStorage = null)
        {
            if (preferedStorage == null)
            {
                _preferedStore.Add(contactToAdd);
            }
            else
            {
                preferedStorage.Add(contactToAdd);
            }
        }

        /// <summary>
        /// Track subscriptions we are keeping.
        /// </summary>
        private List<IDisposable> _ContactStoreSubscriptions = new List<IDisposable>();

        /// <summary>
        /// Add a new contact store to our list. We'll keep everything up to date from it.
        /// </summary>
        /// <param name="s"></param>
        public void Add(IContactStore s)
        {
            if (_preferedStore != null)
            {
                throw new InvalidOperationException("Only one contact store is possible at a time right now!");
            }

            // Connect ourselves to the store. The first thing it should do is send
            // us a complete list of contacts. We'll then forward that on to everyone.
            _preferedStore = s;
            _ContactStoreSubscriptions.Add(s.ContactUpdateStream.Subscribe(_contactStoreStream));
        }

        /// <summary>
        /// What is the reason for this update?
        /// </summary>
        public enum UpdateReason
        {
            Add, Remove
        }

        /// <summary>
        /// The reason for the update message
        /// </summary>
        public struct UpdateInfo
        {
            public UpdateReason _reason;
            public IContact[] _contacts;
        }

        /// <summary>
        /// All items coming from the contact store(s) will come in through here. So new stuff, etc.,
        /// is all mirriored through this stream.
        /// </summary>
        private Subject<UpdateInfo> _contactStoreStream = new Subject<UpdateInfo>();

        /// <summary>
        /// Return a stream that will contain all updates. The initial set of updates will list all
        /// the contacts. You do not have to pre-populate any lists before you subcribe here.
        /// </summary>
        /// <returns>An observable to subscrbe to for new or removed contacts</returns>
        public IObservable<UpdateInfo> UpdateStream
        {
            get
            {
                // TODO: This seems like a race condition - not sure how to deal with it.
                if (_localContactStore.IsEmpty)
                {
                    return _contactStoreStream;
                }
                else
                {
                    return _contactStoreStream
                        .StartWith(new UpdateInfo() { _reason = UpdateReason.Add, _contacts = _localContactStore.ToArray() });
                }
            }
        }

        /// <summary>
        /// Shut down 
        /// </summary>
        public void ShutDown()
        {
            var tmp = _contactStoreStream;
            _contactStoreStream = null;
            tmp.OnCompleted();
        }

        /// <summary>
        /// We maintain lots of contact subscriptiosn - so we'd better get rid of them.
        /// </summary>
        public void Dispose()
        {
            foreach (var c in _ContactStoreSubscriptions)
            {
                c.Dispose();
            }
        }
    }
}
