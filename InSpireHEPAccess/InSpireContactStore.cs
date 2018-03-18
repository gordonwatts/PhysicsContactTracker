using ContactTrackerLib.Database;
using ContractTrackerInterfaces;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static ContactTrackerLib.Database.ContactDB;

namespace InSpireHEPAccess
{
    /// <summary>
    /// Contact store for Insipre Contacts. We gather the complete list of contacts here, hold onto them, update them, etc.
    /// </summary>
    public class InSpireContactStore : IContactStore, IDisposable
    {
        /// <summary>
        /// Return a stream on which we will send all contacts we know about. Make sure to bundle up any we know about already
        /// if we are attached to late.
        /// </summary>
        public IObservable<UpdateInfo> ContactUpdateStream
        {
            get
            {
                if (_localContactStore.IsEmpty)
                {
                    return _contactStoreStream;
                } else
                {
                    return _contactStoreStream
                        .StartWith(new UpdateInfo() { _reason = UpdateReason.Add, _contacts = _localContactStore.ToArray() });
                }
            }
        }

        /// <summary>
        /// Maintain our list of contacts connected to inspire.
        /// </summary>
        private ImmutableList<InSpireContact> _localContactStore = ImmutableList<InSpireContact>.Empty;

        /// <summary>
        /// Internal subject that will forward all updates
        /// </summary>
        private Subject<UpdateInfo> _contactStoreStream = new Subject<UpdateInfo>();

        /// <summary>
        /// Add a contact to our store. Make sure everyone connected to us knows we've added it.
        /// </summary>
        /// <param name="contactToAdd">An InSpire Contact</param>
        /// <remarks>
        /// TODO: We have a downcast here. Is that what we really want to do?
        /// </remarks>
        public void Add(IContact contactToAdd)
        {
            // Argument checking
            var inspireContact = (contactToAdd as InSpireContact)
                ?? throw new InvalidOperationException($"The contact ({contactToAdd.UniqueID}) is not an Inspire Contact!");

            // Next, send it out.
            _contactStoreStream.OnNext(new UpdateInfo() { _reason = UpdateReason.Add, _contacts = new[] { (contactToAdd) } });

            // Add it to the internal database.
            _localContactStore = _localContactStore.Add(inspireContact);
        }

        /// <summary>
        /// Close down any items we are tracking or need. Close down any streams still going.
        /// </summary>
        public void Dispose()
        {
            _contactStoreStream.OnCompleted();
        }
    }
}
