# IssuuSDK

A .NET SDK built for the Issuu v2 API

[![.github/workflows/main.yml](https://github.com/IngeniumSE/IssuuSDK/actions/workflows/main.yml/badge.svg)](https://github.com/IngeniumSE/IssuuSDK/actions/workflows/main.yml) [![.github/workflows/release.yml](https://github.com/IngeniumSE/IssuuSDK/actions/workflows/release.yml/badge.svg)](https://github.com/IngeniumSE/IssuuSDK/actions/workflows/release.yml)

## Installation

The SDK is available as a NuGet package. You can install it using the following command:

```
dotnet add package IssuuSDK
```

## Authentication

This SDK does not currently provide the ability to **generate** an Issuu auth token. Please visit https://issuu.com/home/settings/apicredentials to generate an auth token.

## Usage

### .NET Core & .NET 5+
You can easily add the IssuuSDK to your project by referencing the NuGet package and adding the following configuration and service registration:

```json
{
	"Issuu": {
		"Token": "<token-value>"
	}
}
```

```csharp
services.AddIssuu();
```

This will register a default per-request `IIssuuApiClient` instance in the service collection. You can then inject this into your services and use it to interact with the Issuu API.

### .NET Framework

For use on .NET Framework would largely depend on your dependency injection strategy (if you use one).

You can easily create an instance of the `IssuuApiClient` class and use it to interact with the Issuu API.

```csharp
var http = new HttpClient();ite d
var client = new IssuuApiClient(http, new IssuuSettings());
```

Or, alternatively through the API client factory:

```csharp
var clientFactory = new IssuuApiClientFactory(new IssuuHttpClientFactory());
var client = clientFactory.CreateClient(new IssuuSettings());
```

**NOTE** - On .NET Framework, it is recommended to use a single instance of `HttpClient` for the lifetime of your application. This is because the `HttpClient` class is designed to be reused and not disposed of after each request.

A `IIssuuHttpClientFactory` can be implemented to manage the lifecycle of the `HttpClient` instance.

### Debugging

To aid in debugging results from the Issuu API, you can enable the following settings:

```json
{
	"Issuu": {
		"CaptureRequestContent": true,
		"CaptureResponseContent": true
	}
}
```

These settings, when enabled will capture the request and response content for each API call, and the content of these will be available to the `IssuuResponse` as `RequestContent` and `ResponseContent` properties. The SDK will automatically map these results, but for unexpected results, it is useful to understand what has been sent/received.

## Open Source

This SDK is open source and is available under the MIT license. Feel free to contribute to the project by submitting pull requests or issues.

- .NET Platform by Microsoft and contributors - https://github.com/dotnet
- Ben.Demystifier by Ben Adams - https://github.com/benaadams/Ben.Demystifier
- FluentValidation by the Jeremy Skinner and contributors - https://github.com/FluentValidation/FluentValidation
- MinVer by Adam Ralph and contributors - https://github.com/adamralph/minver
- SlugGenerator by Artem Polishchuk - https://github.com/polischuk/SlugGenerator
