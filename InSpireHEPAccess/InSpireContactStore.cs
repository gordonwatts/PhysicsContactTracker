using ContactTrackerLib.Database;
using ContractTrackerInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Utils;
using static ContactTrackerLib.Database.ContactDB;

namespace InSpireHEPAccess
{
    /// <summary>
    /// Contact store for Insipre Contacts. We gather the complete list of contacts here, hold onto them, update them, etc.
    /// </summary>
    public class InSpireContactStore : IContactStore, IDisposable
    {
        /// <summary>
        /// Make sure all our references are resolved.
        /// </summary>
        public InSpireContactStore()
        {
            MEFComposer.Resolve(this);
        }

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

        private class ContactChangesActions
        {
            public string _name;
            public Func<InSpireContact, InSpireContact, bool> _testForChange;
        }

        private static List<ContactChangesActions> ContactChangedMessages = new List<ContactChangesActions>
        {
            new ContactChangesActions() { _name = "First Name", _testForChange = (old_c, new_c) => old_c.FirstName != new_c.FirstName},
            new ContactChangesActions() { _name = "Last Name", _testForChange = (old_c, new_c) => old_c.LastName != new_c.LastName},
        };

        /// <summary>
        /// Update all of our contacts from the main store. If anything has changed, then propagate it along.
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            foreach (var contact in _localContactStore)
            {
                var newVersion = await RefreshContact(contact);

                // Check for changes.
                var changes = ContactChangedMessages
                    .Where(c => c._testForChange(contact, newVersion))
                    .Select(c => c._name)
                    .ToArray();

                // If there are changes, we need to send it around.
                if (changes.Any())
                {
                    var text = changes
                        .Aggregate((old, newc) => $"{old}, {newc}");

                    _localContactStore = _localContactStore.Remove(contact);
                    _localContactStore = _localContactStore.Add(newVersion);

                    _contactStoreStream.OnNext(new UpdateInfo() { _contacts = new[] { newVersion }, _reason = UpdateReason.Update, _updateReasonText = text });
                }
            }
        }

        /// <summary>
        /// Pointer to the code that does the work of fetching data from the big-bad internet.
        /// </summary>
        [Import()]
        public IWebInterface _webAccess { get; set; }

        /// <summary>
        /// Use the HEPNAMEs DB to refresh the data in this contact.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        private Task<InSpireContact> RefreshContact(InSpireContact contact)
        {
            return contact.UpdateContact(_webAccess);
        }
    }
}
