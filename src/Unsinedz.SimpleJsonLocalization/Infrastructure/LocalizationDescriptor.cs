using System;
using System.Collections.Generic;
using System.Linq;

namespace Unsinedz.SimpleJsonLocalization.Infrastructure
{
    /// <summary>
    /// The descriptor, that contains localizations for particular culture.
    /// </summary>
    /// <typeparam name="TLocalization">The localization type.</typeparam>
    /// <typeparam name="TKey">The localization key type.</typeparam>
    /// <typeparam name="TValue">The localized value type.</typeparam>
    internal class LocalizationDescriptor<TLocalization, TKey, TValue> : IDisposable where TLocalization : Localization<TKey, TValue>
    {
        /// <summary>
        /// The name of the culture.
        /// </summary>
        /// <example>
        /// "en-gb", "en-us".
        /// </example>
        public string CultureName { get; set; }

        /// <summary>
        /// The enumeration, that contains all localizations for the <see cref="CultureName" />.
        /// </summary>
        public IEnumerable<TLocalization> Localizations { get; set; }

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
            Localizations?.ForEach(x => x.Dispose());
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}