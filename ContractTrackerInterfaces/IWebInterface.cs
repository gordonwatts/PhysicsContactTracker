using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContractTrackerInterfaces
{
    /// <summary>
    /// Allows us to substitue other web fetchers for getting data from the internet.
    /// This allows for some modularity and for dealing with testing more easily.
    /// Usually put in via MEF or similar.
    /// </summary>
    public interface IWebInterface
    {
        /// <summary>
        /// Async routine that returns data from a remote location
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<string> DownloadString(Uri uri);
    }
}
