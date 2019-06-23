using System;

namespace SimpleJsonLocalization.Infrastructure
{
    /// <summary>
    /// Used to indicate errors, that occurred during the localization process.
    /// </summary>
    public class LocalizationException : Exception
    {
        /// <summary>
        /// Creates an instance of <see cref="LocalizationException" />.
        /// </summary>
        public LocalizationException()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="LocalizationException" />.
        /// </summary>
        /// <param name="message">The error message.</param>
        public LocalizationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="LocalizationException" />.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public LocalizationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}