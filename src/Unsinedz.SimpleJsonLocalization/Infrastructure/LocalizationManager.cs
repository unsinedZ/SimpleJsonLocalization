using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace Unsinedz.SimpleJsonLocalization.Infrastructure
{
    /// <summary>
    /// Manages resource localizations.
    /// </summary>
    /// <typeparam name="TResourceKey">The resource key type.</typeparam>
    /// <typeparam name="TResourceValue">The resource value type.</typeparam>
    public class LocalizationManager<TResourceKey, TResourceValue> : IResourceReader
    {
        /// <summary>
        /// The dictionary, that contains localizable resource providers per culture.
        /// </summary>
        protected ConcurrentDictionary<CultureInfo, ILocalizableResourceProvider<TResourceKey, TResourceValue>> Providers { get; }
            = new ConcurrentDictionary<CultureInfo, ILocalizableResourceProvider<TResourceKey, TResourceValue>>();

        /// <summary>
        /// The default culture, that is used as a fallback during localization.
        /// </summary>
        protected CultureInfo DefaultCulture { get; set; }

        /// <summary>
        /// Creates an instnce of <see cref="LocalizationManager{TResourceKey, TResourceValue}" />.
        /// </summary>
        /// <param name="defaultCulture">The default culture, that is used as a fallback during localization.</param>
        public LocalizationManager(CultureInfo defaultCulture = null)
        {
            DefaultCulture = defaultCulture ?? CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Adds resource provider to the store, that is used to retrieve resource localizations.
        /// </summary>
        /// <param name="provider">The localizable resource provider.</param>
        public void AddResourceProvider(ILocalizableResourceProvider<TResourceKey, TResourceValue> provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var culture = provider.GetCulture();
            if (culture == null)
                throw new InvalidOperationException("Culture could not be retrieved from provider.");

            if (Providers.ContainsKey(culture))
                throw new InvalidOperationException($"Provider with culture \"{culture.Name}\" was already added.");

            Providers.TryAdd(culture, provider);
        }

        /// <summary>
        /// Localizes resource indentified by localization key using specified culture.
        /// </summary>
        /// <param name="key">The resource localization key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="culture">The localization culture.</param>
        /// <returns>
        /// Localized value, if it is present. Otherwise, <paramref name="defaultValue" />.
        /// </returns>
        public TResourceValue Localize(TResourceKey key, TResourceValue defaultValue = default(TResourceValue), CultureInfo culture = null)
            => Localize(key, out var _, defaultValue, culture);

        /// <summary>
        /// Localizes resource indentified by localization key using specified culture.
        /// </summary>
        /// <param name="key">The resource localization key.</param>
        /// <param name="resourceNotFound">The flag, that indicates whether the resource was found.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="culture">The localization culture.</param>
        /// <returns>
        /// Localized value, if it is present. Otherwise, <paramref name="defaultValue" />.
        /// </returns>
        public TResourceValue Localize(TResourceKey key, out bool resourceNotFound, TResourceValue defaultValue = default(TResourceValue), CultureInfo culture = null)
        {
            var provider = GetMatchingProvider(culture ?? DefaultCulture);
            var result = provider.Get(key);
            if (IsValueAbsent(result))
            {
                resourceNotFound = true;
                return defaultValue;
            }

            resourceNotFound = false;
            return result;
        }

        /// <summary>
        /// Gets provider, that contains localizations for the specified culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        protected ILocalizableResourceProvider<TResourceKey, TResourceValue> GetMatchingProvider(CultureInfo culture)
        {
            CultureInfo specifiedNeutralCulture = null;
            CultureInfo defaultNeutralCulture = null;
            if (IsCultureSupported(culture))
                return Providers[culture];

            if (!culture.IsNeutralCulture && IsCultureSupported(specifiedNeutralCulture = MakeNeutral(culture)))
                return Providers[specifiedNeutralCulture];

            if (!culture.Equals(DefaultCulture))
            {
                if (!IsCultureSupported(DefaultCulture))
                    return Providers[DefaultCulture];

                if (!DefaultCulture.IsNeutralCulture && IsCultureSupported(defaultNeutralCulture = MakeNeutral(DefaultCulture)))
                    return Providers[DefaultCulture];
            }

            throw new ArgumentException($"Provider for the culture \"{culture.LCID}\" does not exist.", nameof(culture));
        }

        /// <summary>
        /// Checks whether specified culture can be localized.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns><c>true<c/>, if culture can be localized. Otherwise, <c>false</c>.</returns>
        protected bool IsCultureSupported(CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));

            return Providers.ContainsKey(culture);
        }

        /// <summary>
        /// Returns a neutral version of specified culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        protected CultureInfo MakeNeutral(CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));

            if (culture.IsNeutralCulture)
                return culture;

            return new CultureInfo(culture.TwoLetterISOLanguageName);
        }

        /// <summary>
        /// Checks whether the localized value is absent.
        /// </summary>
        /// <param name="value">The localized value.</param>
        /// <returns><c>true</c>, if the value is absent. Otherwise, <c>false</c>.</returns>
        protected bool IsValueAbsent(TResourceValue value) => value == null;

        /// <summary>
        /// Closes managed IO resources. 
        /// </summary>
        public void Close()
        {
        }

        /// <summary>
        /// Gets the dictionary enumerator.
        /// </summary>
        public IDictionaryEnumerator GetEnumerator()
        {
            return GetMatchingProvider(DefaultCulture).Values
                .ToDictionary(x => x.Key, x => x.Value)
                .GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #region IDisposable implementation

        private bool _disposed = false;

        /// <summary>
        /// The method used to clean resources used by current instance.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            Providers?.ForEach(x => x.Value.Dispose());
        }
        
        #endregion
    }
}