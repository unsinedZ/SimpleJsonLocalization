using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Unsinedz.SimpleJsonLocalization.Infrastructure;

namespace Unsinedz.SimpleJsonLocalization.Strings
{
    /// <summary>
    /// Manages string localizations.
    /// </summary>
    internal class StringLocalizationManager : LocalizationManager<string, string>, IStringLocalizer
    {
        /// <summary>
        /// Creates an instance of <see cref="StringLocalizationManager" />.
        /// </summary>
        /// <param name="options">The string localization options accessor.</param>
        public StringLocalizationManager(IOptions<StringLocalizationOptions> options) : base(options.Value?.DefaultCulture)
        {
            options.Value?.Providers?.ForEach(AddResourceProvider);
        }

        /// <inheritdoc />
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            if (!includeParentCultures)
                return this.GetMatchingProvider(this.DefaultCulture).Values.Select(x => new LocalizedString(x.Key, x.Value));

            return this.Providers.SelectMany(x => x.Value.Values).Select(x => new LocalizedString(x.Key, x.Value));
        }

        /// <inheritdoc />
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            this.DefaultCulture = culture;
            return this;
        }

        /// <inheritdoc />
        public LocalizedString this[string name] => new LocalizedString(name, this.Localize(name, out bool notFound, name), notFound);

        /// <inheritdoc />
        public LocalizedString this[string name, params object[] arguments] =>
            new LocalizedString(name, string.Format(this.Localize(name, out var notFound, name), arguments), notFound);
    }
}