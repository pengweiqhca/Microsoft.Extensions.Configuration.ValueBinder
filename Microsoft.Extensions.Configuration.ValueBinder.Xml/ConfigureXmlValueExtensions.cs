using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Xml;
using System;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureValueExtensions
    {
        private static readonly Func<FileConfigurationProvider> ProviderCreator = () => new XmlConfigurationProvider(new XmlConfigurationSource { Optional = true });

        /// <summary>Attempts to bind the given object instance to configuration section value by XmlConfigurationProvider.</summary>
        /// <param name="services">The <see cref="T:IServiceCollection" /> to add the service to.</param>
        /// <param name="section">The configuration value to bind.</param>
        public static IServiceCollection ConfigureXmlValue<TOptions>(this IServiceCollection services, IConfigurationSection section) where TOptions : class =>
            services.ConfigureValue<TOptions>(Options.Options.DefaultName, section, ProviderCreator);

        /// <summary>Attempts to bind the given object instance to configuration section value by XmlConfigurationProvider.</summary>
        /// <param name="services">The <see cref="T:IServiceCollection" /> to add the service to.</param>
        /// <param name="name">The name of the options instance being watche.</param>
        /// <param name="section">The configuration value to bind.</param>
        public static IServiceCollection ConfigureXmlValue<TOptions>(this IServiceCollection services, string name, IConfigurationSection section) where TOptions : class =>
            services.ConfigureValue<TOptions>(name, section, ProviderCreator);

        /// <summary>Attempts to bind the given object instance to configuration section value by XmlConfigurationProvider.</summary>
        /// <param name="services">The <see cref="T:IServiceCollection" /> to add the service to.</param>
        /// <param name="configuration">The configuration section value to bind.</param>
        public static IServiceCollection ConfigureXmlValues<TOptions>(this IServiceCollection services, IConfiguration configuration) where TOptions : class =>
            services.ConfigureValues<TOptions>(configuration, ProviderCreator);

        /// <summary>Attempts to bind the given object instance to configuration section value by XmlConfigurationProvider.</summary>
        /// <param name="services">The <see cref="T:IServiceCollection" /> to add the service to.</param>
        /// <param name="value">The value to bind.</param>
        public static IServiceCollection ConfigureXmlValue<TOptions>(this IServiceCollection services, string value) where TOptions : class =>
            services.ConfigureValue<TOptions>(Options.Options.DefaultName, value, ProviderCreator);

        /// <summary>Attempts to bind the given object instance to configuration section value by XmlConfigurationProvider.</summary>
        /// <param name="services">The <see cref="T:IServiceCollection" /> to add the service to.</param>
        /// <param name="name">The name of the options instance being watche.</param>
        /// <param name="value">The value to bind.</param>
        public static IServiceCollection ConfigureXmlValue<TOptions>(this IServiceCollection services, string name, [NotNull] string value) where TOptions : class =>
            services.ConfigureValue<TOptions>(name, value, ProviderCreator);
    }
}
