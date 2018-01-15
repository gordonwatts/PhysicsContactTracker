using System;
using System.Collections.Generic;
using System.Text;
using ContractTrackerInterfaces;

namespace ContactTrackerLib.Database
{
    /// <summary>
    /// Represents a place where all contacts are stored. They can be saved, updated, etc.
    /// Think of it like a local cache
    /// </summary>
    interface IContactStore
    {
        // Add a contact to the store.
        void Add(IContact contactToAdd);
    }
}
