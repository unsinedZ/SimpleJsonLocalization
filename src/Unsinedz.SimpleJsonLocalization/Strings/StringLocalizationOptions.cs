using System.Collections.Generic;
using System.Globalization;
using Unsinedz.SimpleJsonLocalization.Infrastructure;

namespace Unsinedz.SimpleJsonLocalization.Strings
{
    /// <summary>
    /// The string localization options.
    /// </summary>
    public class StringLocalizationOptions
    {
        /// <summary>
        /// The default culture, that is used as a fallback during localization.
        /// </summary>
        public CultureInfo DefaultCulture { get; set; }

        /// <summary>
        /// The value indicating whether the default culture should be used if the localization was not found.
        /// </summary>
        public bool AllowFallbackToDefaultCulture { get; set; }

        /// <summary>
        /// The enumeration, that contains the localizable resource providers.
        /// </summary>
        public IEnumerable<ILocalizableResourceProvider<string, string>> Providers { get; set; }
    }
}