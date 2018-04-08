using ContractTrackerInterfaces;
using InSpireHEPAccess.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSpireHEPAccess
{
    /// <summary>
    /// Code to access InSpire on the web.
    /// </summary>
    static class InspireContactAccess
    {
        public static async Task<IEnumerable<IContact>> FindContactFromHEPNamesAsync(Uri pointsToContact, IWebInterface webAccess)
        {
            // Rebuild the query with of=recjson to get back the proper response.
            var jsonUri = pointsToContact.AsBuilder().AddQuery("of=recjson").Uri;

            // Build the contact and make sure we can parse JSON into an object.
            var jsonData = await webAccess.DownloadString(jsonUri);
            return BuildContactListFromJSON($"The uri {pointsToContact.OriginalString}", jsonData);
        }

        /// <summary>
        /// Return the contact list from this json load.
        /// </summary>
        /// <param name="pointsToContact"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static IEnumerable<IContact> BuildContactListFromJSON(string errMsg, string jsonData)
        {
            var inspireData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataModels.InSpireHEPAccess.DataModels.Welcome[]>(jsonData);

            // Only return real author records. If this uri points to something, but it isn't
            // something we know how to work with, then we report this as an error.
            // TODO: Extend this so you can put in a paper or similar.
            var inspireDataFiltered = inspireData
                .Where(i => i.Collection.Any(c => c.Primary == "HEPNAMES"));
            if (inspireData.Length != 0 && inspireDataFiltered.Count() == 0)
            {
                throw new BadInSpireUrlException($"{errMsg} points to a valid InSpire record, but not a HEPNAMES one!");
            }

            // Next, lets turn that into a contact card.
            return inspireDataFiltered
                .Where(i => i.Collection.Any(c => c.Primary == "HEPNAMES"))
                .SelectMany(i => i.Authors.Select(a => (ath: a, id: i.Recid)))
                .Select(a => a.ath.AsContact(a.id));
        }

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

        /// <summary>
        /// Return the Uri for the contact.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public static Uri AsContactUri(this InSpireContact contact)
        {
            return new Uri($"http://inspirehep.net:80/record/{contact.InspireRecordID}?ln=en");
        }

        /// <summary>
        /// Given a current contact, re-fetch it from InSpire. This should be an inspire contact or
        /// very bad things will happen!
        /// </summary>
        /// <param name="contact">InSpriteContact instance</param>
        /// <param name="webAccess">How we can get at the web</param>
        /// <returns></returns>
        public static async Task<InSpireContact> UpdateContact(this InSpireContact contact, IWebInterface webAccess)
        {
            var val = (await FindContactFromHEPNamesAsync(contact.AsContactUri(), webAccess)).ToArray();
            if (val.Length != 1)
            {
                throw new InvalidOperationException("Request for single contact brought back more than one contact");
            }
            return val[0] as InSpireContact;
        }
    }
}
