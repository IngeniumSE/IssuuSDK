---
sidebar_position: 3
tags: [HTTP Client, API Client, Dependency Injection]
---

# API Client Usage

There are a number of ways to create an API client for accessing the Issuu API. The approach you take will depend on how your app works.

:::tip[Examples]

Examples on this page are using the C# default constructors feature for brevity. The approaches listed here will work with typical constructors.

:::

## Using Dependency Injection (DI)

You can use DI to inject a client into your app code. 

### Using the default client

The default client is pre-configured to use the configured `IssuuSettings`. The client will be disposed when the owning scope is disposed.

```csharp
public class MyService(IIssuuApiClient client)
{
  public async Task DoSomething()
  {
    // Do something with client.
  }
}
```

### Using the API client factory

It is possible to inject the API client factory and settings instead. You need to manage the lifetime of the client itself.

```csharp
public class MyService(IIssuuApiClientFactory clientFactory, IssuuSettings settings)
{
  public async Task DoSomething()
  {
    using var client = clientFactory.CreateApiClient(settings);

    // Do something with client.
  }
}
```

If you are not using the pre-configured `IssuuSettings`, you can provided your own:

```csharp
public class MyService(IIssuuApiClientFactory clientFactory)
{
  public async Task DoSomething()
  {
    var settings = new IssuuSettings();
    using var client = clientFactory.CreateApiClient(settings);

    // Do something with client.
  }
}
```

### Manually creating a client

You can manually create a client yourself, but you are responsible for managing its lifetime. The following are some examples.

#### Creating an API client manually

```csharp
var httpClient = new HttpClient();
var settings = new IssuuSettings();

var apiClient = new IssuuApiClient(httpClient, settings);
```

#### Creating an API client factory manually

```csharp
var httpClientFactory = new IssuuHttpClientFactory();
var apiClientFactory = new IssuuApiClientFactory(httpClientFactory);
var settings = new IssuuSettings();

var apiClient = apiClientFactory.CreateApiClient(settings);
```

### Managing the HTTP Client

If you need to control how the `HttpClient` is created, you can implement your own implementaton of `IIssuuHttpClientFactory`:

```csharp
public class MyIssuuHttpClientFactory : IIssuuHttpClientFactory
{
  public HttpClient CreateClient(string name)
  {
    return new HttpClient();
  }
}
```

If you are using the standard approach to dependency injection, you can register your own implementation, which will be used instead of the default implementation:

```csharp
services.AddIssuu();
services.AddScoped<IIssuuHttpClientFactory, MyIssuuHttpClientFactory>();
```