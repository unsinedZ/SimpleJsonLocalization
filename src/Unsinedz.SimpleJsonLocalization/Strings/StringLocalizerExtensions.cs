using System;
using Microsoft.Extensions.Localization;

namespace Unsinedz.SimpleJsonLocalization.Strings
{
    /// <summary>
    /// Extension methods for <see cref="IStringLocalizer"/>.
    /// </summary>
    public static class StringLocalizerExtensions
    {
        /// <summary>
        /// Returns localized enum value.
        /// </summary>
        /// <param name="stringLocalizer">The string localizer.</param>
        /// <param name="value">The value, that will be localized.</param>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        public static string LocalizeEnumValue<TEnum>(this IStringLocalizer stringLocalizer, TEnum value)
            where TEnum : Enum
        {
            if (stringLocalizer == null)
                throw new ArgumentNullException(nameof(stringLocalizer));

            var stringifiedValue = value.ToString();
            var key = $"{typeof(TEnum).Name}_{stringifiedValue}";
            var localized = stringLocalizer[key];
            return localized.ResourceNotFound
                ? stringifiedValue
                : localized.Value;
        }
    }
}