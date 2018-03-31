using ContractTrackerInterfaces;
using System;
using System.Composition;
using System.Net;
using System.Threading.Tasks;

namespace Utils
{
    /// <summary>
    /// Privdes access to the real web for everything in this project.
    /// </summary>
    [Export(typeof(IWebInterface))]
    public class WebAccessUtil : IWebInterface
    {
        /// <summary>
        /// Download a string from the internet.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<string> DownloadString(Uri uri)
        {
            var request = new WebClient();
            return await request.DownloadStringTaskAsync(uri);
        }
    }
}
