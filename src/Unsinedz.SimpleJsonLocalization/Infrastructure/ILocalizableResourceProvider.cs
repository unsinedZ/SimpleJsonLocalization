using System;
using System.Collections.Generic;
using System.Globalization;

namespace Unsinedz.SimpleJsonLocalization.Infrastructure
{
    /// <summary>
    /// The localizable resource provider signature definition.
    /// </summary>
    /// <typeparam name="TKey">The localization key type.</typeparam>
    /// <typeparam name="TResource">The localizable resource type.</typeparam>
    internal interface ILocalizableResourceProvider<TKey, TResource> : IDisposable
    {
        /// <summary>
        /// The dictionary, that contains all localized values.
        /// </summary>
        IDictionary<TKey, TResource> Values { get; }

        /// <summary>
        /// Gets localized resource by key.
        /// </summary>
        /// <param name="key">The localization key.</param>
        TResource Get(TKey key);

        /// <summary>
        /// Gets culture of current localizable resource provider.
        /// </summary>
        CultureInfo GetCulture();
    }
}