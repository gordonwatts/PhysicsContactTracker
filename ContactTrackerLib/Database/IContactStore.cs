using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ContractTrackerInterfaces;

namespace ContactTrackerLib.Database
{
    /// <summary>
    /// Represents a place where all contacts are stored. They can be saved, updated, etc.
    /// Think of it like a local cache
    /// </summary>
    public interface IContactStore
    {
        /// <summary>
        /// Stream out items as they are updated.
        /// </summary>
        IObservable<ContactDB.UpdateInfo> ContactUpdateStream { get; }

        // Add a contact to the store.
        void Add(IContact contactToAdd);

        // Update all contacts in the store
        Task Update();
    }
}
