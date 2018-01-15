using ContractTrackerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactTrackerLib.Database
{
    /// <summary>
    /// Main database for contacts. Manages and keeps them all in sync, and has interfaces
    /// to save them, etc.
    /// </summary>
    public class ContactDB
    {
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
        void Add (IContact contactToAdd, IContactStore preferedStorage = null)
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
        /// Add a new contact store to our list. We'll keep everything up to date from it.
        /// </summary>
        /// <param name="s"></param>
        void Add(IContactStore s)
        {
            // Connect ourselves to the store. The first thing it should do is send
            // us a complete list of contacts. We'll then forward that on to everyone.
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
        /// Return a stream that will contain all updates. The initial set of updates will list all
        /// the contacts. It assumes that 
        /// </summary>
        /// <returns></returns>
        IObservable<UpdateInfo> UpdateStream()
        {
            return null;
        }
    }
}
