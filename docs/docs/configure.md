---
sidebar_position: 2
title: Configure
description: Details how to configure the IssuuSDK library within your project
tags: [Getting started, Configuration]
---

# Configure

The IssuuSDK targets .NET Standard 2.0 (`netstandard2.0`) which means you can install the SDK on any project that supports .NET Standard 2.0. You can view the [supported frameworks and platforms](https://www.nuget.org/packages/IssuuSDK#supportedframeworks-body-tab) on NuGet.org.

## .NET Core and .NET 5+

Apps written for the newer .NET platform can be configured to use the IssuuSDK through app configuration and service registration. 

### Configuring Issuu settings
To configure the IssuuSDK, add the following to your `appsettings.json` file:

```json
{
  "Issuu": {
    "Token": "{token}"
  }
}
```

:::warning[Authentication Tokens]

The IssuuSDK does not currently support generating authentication tokens. To generate a token for your app, please visit [API Settings](https://issuu.com/home/settings/apicredentials) when you have signed into Issuu.

:::

For a complete list of available settings, please [click here](/docs/reference/settings)

### Service registration
To add IssuuSDK services to your Dependency Injection container, you need to add a call to `AddIssuu()` to your `IServiceCollection`. There are a few _flavours_ of a .NET app, so you'll need to decide which one you are using

#### Using `WebApplication.CreateBuilder()`
This is the typical approach to modern ASP.NET Core apps, typically using Minimal APIs.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIssuu();
```

#### Using a Startup class
This is an approach used by ASP.NET Core web apps.

```csharp
public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddIssuu();
  }
}
```

#### Using `HostBuilder`
The `HostBuilder` approach targets non-ASP.NET Core scenarios, such as Windows services, or Function apps (Azure Functions, AWS Lambda, etc.) that require host lifetime management.

```csharp
var builder = new HostBuilder()
  .ConfigureServices(services => {
    servies.AddIssuu();
  });
```

:::warning[Note on Lifetime]

The `AddIssuu()` extension method registers a per-scope lifetime for the default API client. This is typical for ASP.NET Core apps where the lifetime scope is per-request. For non-ASP.NET Core hosts, you may need to manage the lifetime through a lifetime scope, otherwise you may up with a singleton instance of the client.

:::

## .NET Framework

### Vanilla .NET Framework

If you are targeting .NET Framework but will be managing the lifetime of the SDK services yourself, you can easily create an instance of a client and settings. See [Client Usage](/docs/client-usage) for more information.

### Integration through an Inversion of Control (IoC) container

If you are targeting .NET Framework, you may or may not be using a IoC container, such as Autofac, or StructureMap, etc. It is possible to wire up the IssuuSDK using the same strategy as the default for ASP.NET Core, however as there are numerous IOC containers available, we haven't documented them here. Below is an outline of the recommended service lifetime for the services provided by the SDK:

| Service | Implementation | Recommended Lifetime | Notes |
| --- | --- | --- | --- |
| `IssuuSettings` | `IssuuSettings` | `Singleton` | Pre-configure this instance. |
| `IIssuuHttpClientFactory` | `IssuuHttpClientFactory` | `Scoped` | This is for customising a `HttpClient` instance. |
| `IIsuuApiClientFactory` | `IssuuApiClientFactory` | `Scoped` | |
| `IIsuuApiClient` | `IssuuApiClient` | `Scoped` | This is the default instance when injected directly. |