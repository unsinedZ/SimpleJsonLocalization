using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Unsinedz.SimpleJsonLocalization.Strings;

namespace Unsinedz.SimpleJsonLocalization
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> to add SimpleJsonLocalization services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds SimpleJsonLocalization services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddSimpleJsonLocalization(this IServiceCollection services)
        {
            services.AddSingleton<IStringLocalizer, StringLocalizationManager>();
            return services;
        }
    }
}
