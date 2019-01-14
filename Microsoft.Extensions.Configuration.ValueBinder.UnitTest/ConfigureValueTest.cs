using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Extensions.Configuration.ValueBinder.UnitTest
{
    public class ConfigureJsonValueTest
    {
        private static readonly IDictionary<string, string> Data = new Dictionary<string, string> { ["options"] = Json };
        private const string Json = "{ \"name\": \"Jone\", \"age\": \"30\" }";

        [Fact]
        public void ConfigureConfiguration()
        {
            var options = ConfigureOptions(builder => builder.AddInMemoryCollection(Data),
                (cfg, services) => services.ConfigureJsonValue<TestOptions>(cfg.GetSection("options")));

            AssertOptions(options.CurrentValue, "Jone", 30);
        }

        [Fact]
        public void ConfigureConfigurationWithName()
        {
            var options = ConfigureOptions(builder => builder.AddInMemoryCollection(Data),
                (cfg, services) => services.ConfigureJsonValue<TestOptions>("Jone", cfg.GetSection("options")));

            AssertOptions(options.CurrentValue, null, 0);

            AssertOptions(options.Get("Jone"), "Jone", 30);
        }

        [Fact]
        public void ConfigureJsonValue()
        {
            var options = ConfigureOptions(services => services.ConfigureJsonValue<TestOptions>(Json));

            AssertOptions(options.CurrentValue, "Jone", 30);
        }

        [Fact]
        public void ConfigureJsonValueWithName()
        {
            var options = ConfigureOptions(services => services.ConfigureJsonValue<TestOptions>("Jone", Json));

            AssertOptions(options.CurrentValue, null, 0);

            AssertOptions(options.Get("Jone"), "Jone", 30);
        }

        [Fact]
        public void ConfigureJsonValues()
        {
            var options = ConfigureOptions(builder => builder.AddInMemoryCollection(new Dictionary<string, string>(Data) {["options:Same"] = "{ \"name\": \"Same\", \"age\": 20 }"}),
                (cfg, services) => services.ConfigureJsonValues<TestOptions>(cfg.GetSection("options")));

            AssertOptions(options.CurrentValue, "Jone", 30);
            AssertOptions(options.Get("Same"), "Same", 20);
        }

        [Fact]
        public void ConfigurationChangeTokenSource()
        {
            TestProvider provider = null;
            var data = new Dictionary<string, string>(Data);

            var options = ConfigureOptions(builder => builder.Add(provider = new TestProvider(data)),
                (cfg, services) => services.ConfigureJsonValue<TestOptions>(cfg.GetSection("options")));

            Assert.NotNull(provider);

            AssertOptions(options.CurrentValue, "Jone", 30);

            data["options"] = "{ \"name\": \"Same\", \"age\": 20 }";

            provider.Reload();

            AssertOptions(options.CurrentValue, "Same", 20);
        }

        [Fact]
        public void MultiConfigure()
        {
            var options = ConfigureOptions(services =>
            {
                services.ConfigureJsonValue<TestOptions>(Json);

                services.Configure<TestOptions>(o =>
                {
                    o.Name = "Same";
                    o.Age = 20;
                });
            });

            AssertOptions(options.CurrentValue, "Same", 20);
        }

        private static IOptionsMonitor<TestOptions> ConfigureOptions(Action<IServiceCollection> servicesBuilder)
        {
            var services = new ServiceCollection();

            servicesBuilder?.Invoke(services);

            return services.BuildServiceProvider().GetRequiredService<IOptionsMonitor<TestOptions>>();
        }

        private static IOptionsMonitor<TestOptions> ConfigureOptions(Action<ConfigurationBuilder> configurationBuilder, Action<IConfiguration, IServiceCollection> servicesBuilder)
        {
            var builder = new ConfigurationBuilder();

            configurationBuilder?.Invoke(builder);

            var services = new ServiceCollection();

            IConfiguration configuration = builder.Build();

            services.AddSingleton(configuration);

            servicesBuilder?.Invoke(configuration, services);

            return services.BuildServiceProvider().GetRequiredService<IOptionsMonitor<TestOptions>>();
        }

        private static void AssertOptions(TestOptions options, string name, int age)
        {
            Assert.NotNull(options);

            Assert.Equal(name, options.Name);
            Assert.Equal(age, options.Age);
        }

        private class TestOptions
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        private class TestProvider : ConfigurationProvider, IConfigurationSource
        {
            public TestProvider(IDictionary<string, string> data) => Data = data;

            public IConfigurationProvider Build(IConfigurationBuilder builder) => this;

            public void Reload() => OnReload();
        }
    }
}
