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
        string FirstName { get; }
        string LastName { get; }
    }
}
