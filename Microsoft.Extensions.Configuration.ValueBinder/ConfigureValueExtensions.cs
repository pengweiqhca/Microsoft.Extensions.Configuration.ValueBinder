using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.ValueBinder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureValueExtensions
    {
        private static readonly object SyncLock = new object();

        /// <summary>Attempts to bind the given object instance to configuration section value by FileConfigurationProvider.</summary>
        /// <param name="services">The <see cref="T:IServiceCollection" /> to add the service to.</param>
        /// <param name="section">The configuration value to bind.</param>
        /// <param name="creator">return FileConfigurationProvider</param>
        public static IServiceCollection ConfigureValue<TOptions>(this IServiceCollection services, IConfigurationSection section, [NotNull] Func<FileConfigurationProvider> creator) where TOptions : class =>
            services.ConfigureValue<TOptions>(Options.Options.DefaultName, section, creator);

        /// <summary>Attempts to bind the given object instance to configuration section value by FileConfigurationProvider.</summary>
        /// <param name="services">The <see cref="T:IServiceCollection" /> to add the service to.</param>
        /// <param name="name">The name of the options instance being watche.</param>
        /// <param name="section">The configuration value to bind.</param>
        /// <param name="creator">return FileConfigurationProvider</param>
        public static IServiceCollection ConfigureValue<TOptions>(this IServiceCollection services, string name, IConfigurationSection section, [NotNull] Func<FileConfigurationProvider> creator) where TOptions : class
        {
            if (creator == null) throw new ArgumentNullException(nameof(creator));
          
            FileConfigurationProvider provider = null;
            return services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(new ConfigurationChangeTokenSource<TOptions>(name, section))
                .Configure<TOptions>(name, options => Bind(options, section.Value, ref provider, creator));
        }

        /// <summary>Attempts to bind the given object instance to configuration section value by FileConfigurationProvider.</summary>
        /// <param name="services">The <see cref="T:IServiceCollection" /> to add the service to.</param>
        /// <param name="configuration">The configuration section value to bind.</param>
        /// <param name="creator">return FileConfigurationProvider</param>
        public static IServiceCollection ConfigureValues<TOptions>(this IServiceCollection services, IConfiguration configuration, [NotNull] Func<FileConfigurationProvider> creator) where TOptions : class
        {
            if (creator == null) throw new ArgumentNullException(nameof(creator));

            FileConfigurationProvider provider = null;
            return services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(new ConfigurationChangeTokenSource<TOptions>(configuration))
                .AddOptions()
                .AddSingleton<IConfigureOptions<TOptions>>(new SectionValueConfigureOptions<TOptions>(configuration,
                    (options, value) => Bind(options, value, ref provider, creator)));
        }

        /// <summary>Attempts to bind the given object instance to configuration section value by FileConfigurationProvider.</summary>
        /// <param name="services">The <see cref="T:IServiceCollection" /> to add the service to.</param>
        /// <param name="value">The value to bind.</param>
        /// <param name="creator">return FileConfigurationProvider</param>
        public static IServiceCollection ConfigureValue<TOptions>(this IServiceCollection services, string value, [NotNull] Func<FileConfigurationProvider> creator) where TOptions : class =>
            services.ConfigureValue<TOptions>(Options.Options.DefaultName, value, creator);

        /// <summary>Attempts to bind the given object instance to configuration section value by FileConfigurationProvider.</summary>
        /// <param name="services">The <see cref="T:IServiceCollection" /> to add the service to.</param>
        /// <param name="name">The name of the options instance being watche.</param>
        /// <param name="value">The value to bind.</param>
        /// <param name="creator">return FileConfigurationProvider</param>
        public static IServiceCollection ConfigureValue<TOptions>(this IServiceCollection services, string name, [NotNull] string value, [NotNull] Func<FileConfigurationProvider> creator) where TOptions : class
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (creator == null) throw new ArgumentNullException(nameof(creator));

            FileConfigurationProvider provider = null;
            return services.Configure<TOptions>(name, options => Bind(options, value, ref provider, creator));
        }

        private static void Bind<TOptions>(TOptions options, string value, ref FileConfigurationProvider provider, Func<FileConfigurationProvider> creator) where TOptions : class
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            if (provider == null)
                lock (SyncLock)
                    if (provider == null)
                        provider = creator();

            Bind(options, value, provider);
        }

        /// <summary>Attempts to bind the given object instance to configuration section value by FileConfigurationProvider.</summary>
        /// <param name="options">The object to bind.</param>
        /// <param name="value">The value to bind.</param>
        /// <param name="provider">The <see cref="T:FileConfigurationProvider" /> to load the value.</param>
        public static void Bind<TOptions>([NotNull] TOptions options, string value, [NotNull] FileConfigurationProvider provider) where TOptions : class
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(value)) return;
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            var root = new ConfigurationRoot(new List<IConfigurationProvider> { provider });

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                provider.Load(stream);

            root.Bind(options);
        }
    }
}
