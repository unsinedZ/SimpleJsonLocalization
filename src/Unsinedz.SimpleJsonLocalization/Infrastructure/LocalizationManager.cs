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
        protected CultureInfo DefaultCulture { get; set; } = CultureInfo.InvariantCulture;

        /// <summary>
        /// Fallback to default culture if the localization was not found for the specified one.
        /// </summary>
        protected bool AllowFallbackToDefaultCulture { get; set; }

        /// <summary>
        /// Creates an instnce of <see cref="LocalizationManager{TResourceKey, TResourceValue}" />.
        /// </summary>
        /// <param name="defaultCulture">The default culture, that is used as a fallback during localization.</param>
        public LocalizationManager(bool allowFallbackToDefaultCulture)
        {
            AllowFallbackToDefaultCulture = allowFallbackToDefaultCulture;
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
            var provider = GetMatchingProvider(culture);
            if (provider == null && (culture == DefaultCulture || !CanFallbackToDefaultCulture() || (provider = GetMatchingProvider(DefaultCulture)) == null))
            {
                resourceNotFound = true;
                return defaultValue;
            }

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
        /// Gets a value indicating whether fallback to default culture is possible.
        /// </summary>
        /// <returns><c>true</c> if fallback is possible, otherwise <c>false</c>.</returns>
        protected bool CanFallbackToDefaultCulture() => AllowFallbackToDefaultCulture && DefaultCulture != null;

        /// <summary>
        /// Gets provider, that contains localizations for the specified culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        protected ILocalizableResourceProvider<TResourceKey, TResourceValue> GetMatchingProvider(CultureInfo culture)
        {
            if (Providers.TryGetValue(culture, out var matchingProvider))
                return matchingProvider;

            if (!culture.IsNeutralCulture && Providers.TryGetValue(culture.Parent, out var neutralProvider))
                return neutralProvider;

            return null;
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