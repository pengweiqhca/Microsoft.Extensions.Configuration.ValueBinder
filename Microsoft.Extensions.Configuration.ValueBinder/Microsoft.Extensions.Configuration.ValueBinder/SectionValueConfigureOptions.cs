using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.Configuration.ValueBinder
{
    internal class SectionValueConfigureOptions<TOptions> : IConfigureNamedOptions<TOptions> where TOptions : class
    {
        private readonly IConfiguration _configuration;
        private readonly Action<TOptions, string> _configure;

        public SectionValueConfigureOptions([NotNull] IConfiguration configuration, [NotNull] Action<TOptions, string> configure)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _configure = configure ?? throw new ArgumentNullException(nameof(configure));
        }

        public void Configure(string name, TOptions options)
        {
            if (string.IsNullOrEmpty(name))
            {
                var value = _configuration.GetSection(Options.Options.DefaultName).Value;
                if (value == null && _configuration is IConfigurationSection cs)
                    value = cs.Value;

                _configure(options, value);
            }
            else
                _configure(options, _configuration.GetSection(name).Value);
        }

        public void Configure(TOptions options) => Configure(Options.Options.DefaultName, options);
    }
}
