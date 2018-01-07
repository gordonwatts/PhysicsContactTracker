using System;
using System.Collections.Generic;
using System.Text;

namespace ContractTrackerInterfaces
{
    /// <summary>
    /// The contact details for a person from whatever store or provider we are dealing with.
    /// </summary>
    /// <remarks>
    /// The general properties on this should return a value without any off-machine network access. In short,
    /// when this is first setup you should make sure to fetch the required values.
    /// </remarks>
    public interface IContact
    {
        // First and last name for the contact
        string FirstName { get; }
        string LastName { get; }

        /// <summary>
        /// A unique identification string that is used by the Windows Contact system. Do not set.
        /// </summary>
        /// <remarks>
        /// This is set by the UWP Contact system and used to track what is going on. No need to do anything
        /// but store any value set.
        /// </remarks>
        string UniqueID { get; set; }
    }
}
