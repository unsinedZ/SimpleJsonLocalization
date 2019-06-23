using System;

namespace Unsinedz.SimpleJsonLocalization.Infrastructure
{
    /// <summary>
    /// The resource localization.
    /// </summary>
    /// <typeparam name="TKey">The localization key type.</typeparam>
    /// <typeparam name="TValue">The localized value type.</typeparam>
    public class Localization<TKey, TValue> : IDisposable
    {
        /// <summary>
        /// The localization key.
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        /// The localized value.
        /// </summary>
        public TValue Value { get; set; }

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
            if (Key is IDisposable disposableKey)
                disposableKey.Dispose();

            if (Value is IDisposable disposableValue)
                disposableValue.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}