using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContractTrackerInterfaces
{
    /// <summary>
    /// Helps track down a contact given various bits of information.
    /// </summary>
    public interface IContactFinder
    {
        /// <summary>
        /// A uri is given and a contact is returned from it (or a list of contacts).
        /// </summary>
        /// <param name="pointsToContact">A Uri which will identify a contact(s) for this provider</param>
        /// <returns>A list of contacts the user might have meant when looking at this page. It returns an empty list if no contact can be found/guessed.</returns>
        /// <exception cref="UriNotUnderstoodException">If the Uri poitns to a link that makes no sense to this provider</exception>
        Task<IEnumerable<IContact>> FindContactAsync(Uri pointsToContact);
    }

    /// <summary>
    /// Thrown when a you give to this contact finder isn't pointing to somethign it
    /// can understand.
    /// </summary>
    [Serializable]
    public class UriNotUnderstoodException : Exception
    {
        public UriNotUnderstoodException() { }
        public UriNotUnderstoodException(string message) : base(message) { }
        public UriNotUnderstoodException(string message, Exception inner) : base(message, inner) { }
        protected UriNotUnderstoodException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
