using ContractTrackerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;

namespace ContactMapper.Helpers
{
    /// <summary>
    /// Code to help with sync'ing our contact list with the master one.
    /// </summary>
    static class ContactSync
    {
        /// <summary>
        /// The ID for the contact list we create for this app.
        /// </summary>
        const string AppContactListName = "Physics Contact Tracker";

        /// <summary>
        /// Remove contacts from our list that no longer exist, add ones that don't exist yet,
        /// and update ones that are there with new information.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static async Task SyncContactList(IEnumerable<IContact> source)
        {
            var contactList = await GetAppContactList();

            // Because we don't know the order, create a map from the source so we can
            // quickly do lookups.
            var contactMap = source
                .Where(c => c.UniqueID != null)
                .ToDictionary(c => c.UniqueID, c => c);

            // Loop through all the contacts in our list.
            var seenContacts = new HashSet<string>();
            var toDelete = new List<Contact>();
            await contactList.ProcessAllContacts(async uwpContact =>
            {
                // What should we do with this contact?
                var id = uwpContact.Id;
                if (contactMap.TryGetValue(id, out IContact ourContact))
                {
                    // Make sure it is up to date.
                    seenContacts.Add(uwpContact.Id);
                    await SyncContact(contactList, uwpContact, ourContact);
                } else
                {
                    // Ok, the contact needs to be deleted. In order to
                    // make sure we don't mess up the list, add it to a list.
                    toDelete.Add(uwpContact);
                }
            });

            // Delete old contacts
            foreach (var oldContact in toDelete)
            {
                await contactList.DeleteContactAsync(oldContact);
            }

            // Add new contacts
            var newContacts = source
                .Where(c => c.UniqueID == null || !seenContacts.Contains(c.UniqueID));
            foreach (var newOurContact in newContacts)
            {
                var uwpContact = new Contact();
                await SyncContact(contactList, uwpContact, newOurContact);
            }
        }

        /// <summary>
        /// Make sure the data in our contact and the other contact are the same. If we update, then save it.
        /// </summary>
        /// <param name="uwpContact"></param>
        /// <param name="ourContact"></param>
        /// <returns></returns>
        private static async Task SyncContact(ContactList uwpList, Contact uwpContact, IContact ourContact)
        {
            // If somethign has changed, then we should resave.
            bool modified = false;

            if (uwpContact.FirstName != ourContact.FirstName)
            {
                modified = true;
                uwpContact.FirstName = ourContact.FirstName;
            }
            if (uwpContact.LastName != ourContact.LastName)
            {
                modified = true;
                uwpContact.LastName = ourContact.LastName;
            }

            if (modified)
            {
                await uwpList.SaveContactAsync(uwpContact);

                // Cache the contact so next time through we can be sure to properly sync things.
                ourContact.UniqueID = uwpContact.Id;
            }
        }

        /// <summary>
        /// Loop over all contacts and call a function that will process one of them.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="processor"></param>
        /// <returns></returns>
        private static async Task ProcessAllContacts (this ContactList source, Func<Contact, Task> processor)
        {
            var reader = source.GetContactReader();
            var batch = await reader.ReadBatchAsync();
            while (batch.Contacts.Count != 0)
            {
                foreach (var c in batch.Contacts)
                {
                    await processor(c);
                }
                batch = await reader.ReadBatchAsync();
            }
        }

        /// <summary>
        /// Return a contact list. Create it if necessary.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// First go at this came from https://social.msdn.microsoft.com/Forums/en-US/54f84ddf-bcf3-4c15-85dc-77b2688c2f62/uwpc-add-contacts-programatically?forum=wpdevelop
        /// </remarks>
        private static async Task<ContactList> GetAppContactList()
        {
            // Get the already created list.
            var store = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);
            var ourList = (await store.FindContactListsAsync())
                .Where(l => l.DisplayName == AppContactListName)
                .FirstOrDefault();

            // If we can't find it, then create it.
            if (ourList == null)
            {
                ourList = await store.CreateContactListAsync(AppContactListName);
                ourList.OtherAppReadAccess = ContactListOtherAppReadAccess.Full;
                ourList.OtherAppWriteAccess = ContactListOtherAppWriteAccess.SystemOnly;
                await ourList.SaveAsync();
            }

            return ourList;
        }
    }
}
