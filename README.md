Bind complex value(such as JSON, XML) to options

How to use
=============

## Install the [Tuhu.Extensions.Configuration.ValueBinder.Json](https://www.nuget.org/packages/Tuhu.Extensions.Configuration.ValueBinder.Json) package.

``` PS
Install-Package Tuhu.Extensions.Configuration.ValueBinder.Json
```
In your testing project, add the following framework
### Bind IConfigurationSection json value

``` C#
services.ConfigureJsonValue<TOptions>([string name, ]IConfigurationSection section, [NotNull] Func<FileConfigurationProvider> creator)
```

### Bind json value

``` C#
services.ConfigureJsonValue<TOptions>([string name, ]string value, [NotNull] Func<FileConfigurationProvider> creator)
```

### Bind IConfiguration json values

Map child section `Key` to options name, map empty section `key` or `Value`(if is IConfigurationSection) to default options

``` C#
services.ConfigureJsonValues<TOptions>(IConfiguration configuration, [NotNull] Func<FileConfigurationProvider> creator)
```

## Install the [Tuhu.Extensions.Configuration.ValueBinder](https://www.nuget.org/packages/Tuhu.Extensions.Configuration.ValueBinder) package.

``` PS
Install-Package Tuhu.Extensions.Configuration.ValueBinder
```
In your testing project, add the following framework

### Bind IConfigurationSection value

``` C#
services.ConfigureValue<TOptions>([string name, ]IConfigurationSection section, [NotNull] Func<FileConfigurationProvider> creator)
```

### Bind string value

``` C#
services.ConfigureValue<TOptions>([string name, ]string value, [NotNull] Func<FileConfigurationProvider> creator)
```

### Bind IConfiguration values

Map child section `Key` to options name, map empty section `key` or `Value`(if is IConfigurationSection) to default options

``` C#
services.ConfigureValues<TOptions>(IConfiguration configuration, [NotNull] Func<FileConfigurationProvider> creator)
```
