using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SimpleJsonLocalization.Infrastructure;
using Unsinedz.SimpleJsonLocalization.Infrastructure;

namespace Unsinedz.SimpleJsonLocalization.Strings
{
    /// <summary>
    /// The JSON string localizable resource provider.
    /// </summary>
    public class JsonStringProvider : ILocalizableResourceProvider<string, string>
    {
        /// <summary>
        /// The resource file name.
        /// </summary>
        protected string FileName { get; set; }

        /// <summary>
        /// The string localization descriptor.
        /// </summary>
        protected Lazy<StringLocalizationDescriptor> Descriptor { get; }

        /// <inheritdoc/>
        public IDictionary<string, string> Values => Descriptor.Value.Localizations?.ToDictionary(x => x.Key, x => x.Value);

        /// <summary>
        /// Creates an instance of <see cref="JsonStringProvider" />.
        /// </summary>
        /// <param name="fileName">The resource file name.</param>
        public JsonStringProvider(string fileName)
        {
            FileName = fileName;
            Descriptor = new Lazy<StringLocalizationDescriptor>(GetResourceDescriptor);
        }

        /// <inheritdoc/>
        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new System.ArgumentException("Key must not be empty.", nameof(key));

            return Descriptor.Value.Localizations?.SingleOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase))?.Value;
        }

        /// <inheritdoc/>
        public CultureInfo GetCulture() => new CultureInfo(Descriptor.Value.CultureName);

        /// <summary>
        /// Gets the string localization descriptor.
        /// </summary>
        protected StringLocalizationDescriptor GetResourceDescriptor()
        {
            try
            {
                var source = GetResourceFileContent();
                if (string.IsNullOrEmpty(source))
                    return null;

                var descriptor = JsonConvert.DeserializeObject<StringLocalizationDescriptor>(source);
                if (descriptor == null)
                    throw new InvalidOperationException(string.Format("Descriptor could not be loaded from resource file: \"{0}\".", FileName));

                return descriptor;
            }
            catch (JsonException jsonException)
            {
                throw new LocalizationException("Invalid JSON format.", jsonException);
            }
        }

        /// <summary>
        /// Gets the content string of the resource file.
        /// </summary>
        protected string GetResourceFileContent()
        {
            try
            {
                return File.ReadAllText(FileName);
            }
            catch (IOException ioException)
            {
                throw new LocalizationException($"An error accurred during reading localization file \"{FileName}\".", ioException);
            }
        }

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
            Descriptor.Value.Dispose();
        }

        #endregion
    }
}