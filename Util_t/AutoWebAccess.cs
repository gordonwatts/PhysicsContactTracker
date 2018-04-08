using ContractTrackerInterfaces;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Util_t
{
    [Export(typeof(IWebInterface))]
    public class AutoWebAccess : IWebInterface
    {
        public Task<string> DownloadString(Uri uri)
        {
            var r = _responses
                ?.Where(rs => rs.checker(uri.OriginalString))
                .FirstOrDefault();

            if (r == null)
            {
                throw new InvalidOperationException($"Test failure - system requested uri '{uri.OriginalString}' and we don't know how to respond.");
            }

            return Task.FromResult<string>(r.response);
        }

        /// <summary>
        /// Hold onto whatever we need to keep track of.
        /// </summary>
        private class UriResponse
        {
            /// <summary>
            /// See if this uri matches
            /// </summary>
            public Func<string, bool> checker;

            /// <summary>
            /// The json (or whatever) response.
            /// </summary>
            public string response;
        }

        private static List<UriResponse> _responses = null;

        /// <summary>
        /// Reset everything to null
        /// </summary>
        public static void Reset()
        {
            _responses = null;
        }


        /// <summary>
        /// Store a Uri response for a complete URI.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="fileInfo"></param>
        public static void AddUriResponse(string v, FileInfo fileInfo)
        {
            _responses = _responses ?? new List<UriResponse>();
            _responses.Add(new UriResponse() { checker = s => s == v, response = File.ReadAllText(fileInfo.FullName) });
        }
    }
}
