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

            // Next, lets turn that into a contact card.
            return inspireData
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
