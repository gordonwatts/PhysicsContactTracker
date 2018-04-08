using ContractTrackerInterfaces;
using InSpireHEPAccess.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using ContactTrackerLib;
using System.Composition;
using Utils;

namespace InSpireHEPAccess
{
    /// <summary>
    /// Thrown when the URL for an InSpire is somehow invalid.
    /// </summary>
    [Serializable]
    public class BadInSpireUrlException : Exception
    {
        public BadInSpireUrlException() { }
        public BadInSpireUrlException(string message) : base(message) { }
        public BadInSpireUrlException(string message, Exception inner) : base(message, inner) { }
        protected BadInSpireUrlException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    /// <summary>
    /// Find contacts on InSpire
    /// </summary>
    /// <remarks>
    /// Inspire HEP WebApi docs: http://inspirehep.net/info/hep/api
    /// </remarks>
    public class InSpireContactFinder : IContactFinder
    {
        public InSpireContactFinder()
        {
            MEFComposer.Resolve(this);
        }

        /// <summary>
        /// Look for a contact on Inspire given a particular web page.
        /// </summary>
        /// <param name="pointsToContact">A Uri from inspire pointing somewhere useful</param>
        /// <returns></returns>
        public Task<IEnumerable<IContact>> FindContactAsync(Uri pointsToContact)
        {
            // Make sure this is an InspireQuery
            if (pointsToContact.Host != "inspirehep.net")
            {
                throw new BadInSpireUrlException($"Uri {pointsToContact.OriginalString} does not seem to point at a valid InSpire HEPNAMEs database");
            }
            if (!pointsToContact.PathAndQuery.StartsWith("/record"))
            {
                throw new BadInSpireUrlException($"Uri {pointsToContact.OriginalString} does not seem to point to a InSpire record at all!");
            }

            // This must refer to a unique record.

            // It must be a HepNames database pointers.
            return FindContactFromHEPNamesAsync(pointsToContact);
        }

        /// <summary>
        /// Pointer to the code that does the work of fetching data from the big-bad internet.
        /// </summary>
        [Import()]
        public IWebInterface _webAccess { get; set; }

        /// <summary>
        /// This is a HepNames query. Return the person it is attached to.
        /// </summary>
        /// <param name="pointsToContact"></param>
        /// <returns></returns>
        private async Task<IEnumerable<IContact>> FindContactFromHEPNamesAsync(Uri pointsToContact)
        {
            return await InspireContactAccess.FindContactFromHEPNamesAsync(pointsToContact, _webAccess);
        }
    }
}
