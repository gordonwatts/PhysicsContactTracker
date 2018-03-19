using ContractTrackerInterfaces;
using InSpireHEPAccess.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

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
        /// This is a HepNames query. Return the person it is attached to.
        /// </summary>
        /// <param name="pointsToContact"></param>
        /// <returns></returns>
        private async Task<IEnumerable<IContact>> FindContactFromHEPNamesAsync(Uri pointsToContact)
        {
            // Rebuild the query with of=recjson to get back the proper response.
            var jsonUri = pointsToContact.AsBuilder().AddQuery("of=recjson").Uri;

            // Build the contact and make sure we can parse JSON into an object.
            var request = new WebClient();
            var jsonData = await request.DownloadStringTaskAsync(jsonUri);
            var inspireData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataModels.InSpireHEPAccess.DataModels.Welcome[]>(jsonData);

            // Only return real author records. If this uri points to something, but it isn't
            // something we know how to work with, then we report this as an error.
            // TODO: Extend this so you can put in a paper or similar.
            var inspireDataFiltered = inspireData
                .Where(i => i.Collection.Any(c => c.Primary == "HEPNAMES"));
            if (inspireData.Length != 0 && inspireDataFiltered.Count() == 0)
            {
                throw new BadInSpireUrlException($"The Uri {pointsToContact.OriginalString} points to a valid InSpire record, but not a HEPNAMES one!");
            }

            // Next, lets turn that into a contact card.
            return inspireDataFiltered
                .Where(i => i.Collection.Any(c => c.Primary == "HEPNAMES"))
                .SelectMany(i => i.Authors.Select(a => (ath: a, id: i.Recid)))
                .Select(a => a.ath.AsContact(a.id));
        }
    }

    static class InSpireContactFinderUtils
    {
        /// <summary>
        /// Helper method to seperate the conversion logic out
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static InSpireContact AsContact(this DataModels.InSpireHEPAccess.DataModels.Author a, long id)
        {
            return new InSpireContact()
            {
                FirstName = a.FirstName,
                LastName = a.LastName,
                InspireRecordID = id,
            };
        }
    }

}
