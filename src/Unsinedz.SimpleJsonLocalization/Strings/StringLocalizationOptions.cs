using System.Collections.Generic;
using System.Globalization;
using Unsinedz.SimpleJsonLocalization.Infrastructure;

namespace Unsinedz.SimpleJsonLocalization.Strings
{
    /// <summary>
    /// The string localization options.
    /// </summary>
    internal class StringLocalizationOptions
    {
        /// <summary>
        /// The default culture, that is used as a fallback during localization.
        /// </summary>
        public CultureInfo DefaultCulture { get; set; }

        /// <summary>
        /// The enumeration, that contains the localizable resource providers.
        /// </summary>
        public IEnumerable<ILocalizableResourceProvider<string, string>> Providers { get; set; }
    }
}