using System;
using System.Collections.Generic;
using System.Text;

namespace InSpireHEPAccess.Utils
{
    static class UriBuilderUtils
    {
        /// <summary>
        /// Convert a Uri to a builder.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static UriBuilder AsBuilder (this Uri source)
        {
            return new UriBuilder(source);
        }

        /// <summary>
        /// Add a query to the uri in a builder.
        /// </summary>
        /// <param name="uBuilder">Builder to add to the query in</param>
        /// <param name="extraQueryString">The properly formatted extra query info to be added to the string.</param>
        public static UriBuilder AddQuery (this UriBuilder uBuilder, string extraQueryString)
        {
            uBuilder.Query = string.IsNullOrWhiteSpace(uBuilder.Query)
                ? extraQueryString
                : $"{uBuilder.Query.Substring(1)}&{extraQueryString}";
            return uBuilder;
        }
    }
}
