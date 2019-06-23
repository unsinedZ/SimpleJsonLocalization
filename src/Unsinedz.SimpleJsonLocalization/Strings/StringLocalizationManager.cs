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
    public class StringLocalizationManager : LocalizationManager<string, string>, IStringLocalizer
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
            var providers = new List<ILocalizableResourceProvider<string, string>> { GetMatchingProvider(DefaultCulture) };
            if (includeParentCultures)
            {
                var initialCulture = providers[0].GetCulture();
                var cultures = new List<CultureInfo>();
                while (!string.IsNullOrWhiteSpace(initialCulture.Name))
                    cultures.Add(initialCulture = initialCulture.Parent);

                providers.AddRange(cultures.Select(GetMatchingProvider).Distinct());
            }

            return providers.SelectMany(x => x.Values).Select(x => new LocalizedString(x.Key, x.Value));
        }

        /// <inheritdoc />
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            DefaultCulture = culture;
            return this;
        }

        /// <inheritdoc />
        public LocalizedString this[string name] => new LocalizedString(name, Localize(name, out bool notFound, name, CultureInfo.CurrentUICulture), notFound);

        /// <inheritdoc />
        public LocalizedString this[string name, params object[] arguments] =>
            new LocalizedString(name, string.Format(Localize(name, out var notFound, name, CultureInfo.CurrentUICulture), arguments), notFound);
    }
}