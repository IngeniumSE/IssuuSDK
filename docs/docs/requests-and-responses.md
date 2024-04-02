---
sidebar_position: 4
tags: [Request, Response, API Client, Debugging]
---

# Requests and Responses

While the IssuuSDK provides standard operations supported by the Issuu API, it is possible to craft your own requests if the IssuuSDK does not currently support an operation you need.

## Requests

The IssuuSDK provides two request types, `IssuuRequest` and `IssuuRequest<TData>`. These are defined here: https://github.com/IngeniumSE/IssuuSDK/blob/main/libs/IssuuSDK/IssuuRequest.cs

The latter `IssuuRequest<TData>` inherits from `IssuuRequest` but is tailored towards sending payload data to the Issuu API.

:::tip[File Content]

You'd still need to use `IssuuRequest` for sending payload data where that payload is a file, as `IssuuRequest<TData>` will use JSON serialization on `TData` as the payload.

:::

### HTTP GET example - returning a single resource

```csharp
var request = new IssuuRequest(
    HttpMethod.Get,
    $"/drafts/{slug}"
);

var response = await apiClient.FetchAsync<Document>(
    request,
    async httpResponse => await Map(httpResponse)
);
```

In the above example, we're crafting a HTTP GET request to the `https://api.issuu.com/v2/drafts/{slug}` resource, where `{slug}` is the document slug, and then mapping the response back as a `Document`. The `response` instance will be of type `IssuuResponse<Document>`.

:::tip[Transforming HTTP responses]

The `FetchAsync` method provides a mapping function that allows you to transform your `HttpResponse` into your requested type. You will be responsible for transforming the `HttpContent` into your desired type.

The IssuuSDK does provide some container types for desializing the raw JSON content, where you know the expected shape of the output.

:::

### HTTP GET example - returning a set of resources, with a query string

```csharp
var request = new IssuuRequest(
    HttpMethod.Get,
    $"/drafts/",
    new QueryStringBuilder()
        .AddParameter("page", 1)
        .AddParameter("size", 10)
        .Build()
);

var response = await apiClient.FetchAsync<Document[]>(
    request,
    async httpResponse => await Map(httpResponse)
);
```

In the above example, we're crafting a HTTP GET request to the `https://api.issuu.com/v2/drafts` resource, and then mapping the response back as an array `Document`. The `response` instance will be of type `IssuuResponse<Document[]>`. You can use any collection type that is supported by deserializing a JSON array by `System.Text.Json`

### HTTP POST example - posting payload data

```csharp
var draft = new Draft();
var request = new IssuuRequest<Draft>(
    HttpMethod.Post,
    $"/drafts/",
    draft
);

var response = await apiClient.SendAsync(request);
```

:::tip[`SendAsync` vs. `FetchAsync`]

You can utilise `SendAsync` when you do not expect, or do not care about any result data. `FetchAsync` should be used when you are expecting some output data from the Issuu API.

:::

### HTTP PATCH example - posting a file using a path

```csharp
var request = new IssuuRequest(
    new HttpMethod("PATCH"),
    $"/drafts/{slug}/upload",
    useMultipartContent: true,
    filePath: "/path/to.file",
    fileName: "custom_file_name.pdf",
    formData: new Dictionary<string, string>
    {
        ["confirmCopright"] = "true"
    }
);

var response = await apiClient.SendAsync(request);
```

When posting a file, you must ensure `useMultipartContent` is `true`. Internally, the IssuuSDK will generate a bounded mutipart content container, which will contain the `StreamContent` for the file, and optionally any other form data, allowing you to post a file, and provide additional parameters at the same time.

If you do not provide `fileName`, the IssuuSDK will use the file name from the provided `filePath`.

### HTTP PATCH example - posting a file using a stream

```csharp
var stream = new FileStream("/path/to.file");

var request = new IssuuRequest(
    new HttpMethod("PATCH"),
    $"/drafts/{slug}/upload",
    useMultipartContent: true,
    fileStream: stream,
    fileName: "custom_file_name.pdf",
    formData: new Dictionary<string, string>
    {
        ["confirmCopright"] = "true"
    }
);

var response = await apiClient.SendAsync(request);
```

If you are providing a `Stream`, the `fileName` parameter is required, as this cannot be determined otherwise. Using a `Stream` allows you to target any stream, as long as it can be rewound.

## Responses

The `IssuuResponse` and `IssuuResponse<TData>` types are used to represents responses returned by the Issuu API. The latter type, `IssuuResponse<TData>` is used when there is payload data in the HTTP response that you want deserialized into the `TData` type. These are defined here: https://github.com/IngeniumSE/IssuuSDK/blob/main/libs/IssuuSDK/IssuuResponse.cs.

:::tip[Issuu API challenges]

The Issuu API returns results in an often inconsistent _shape_ that may differ from operation to operation. It is recommended to use the standard `Drafts` and `Publications` operations provided by the API client, which handles these different result shapes, but return a consistent code API _shape_ instead.

:::

Here are common properties on `IssuuResponse`:

| Property | Type | Notes |
|---|---|---|
| `IsSuccess` | `bool` | Was the HTTP status a success code? |
| `Error` | `Error?` | The captured error (if applicable) |
| `Links` | `Dictionary<string, string>?` | Any captured links |
| `Meta` | `Meta?` | Metadata, such as paging information |
| `StatusCode` | `HttpStatusCode` | The HTTP status code of the response |
| `RateLimiting` | `RateLimiting?` | Information on rate limiting |
| `RequestMethod` | `HttpMethod` | The HTTP method used |
| `RequestUri` | `Uri` | The requested URI resource |
| `RequestContent` | `string?` | See [Developer-friendly Debugging](#developer-friendly-debugging) |
| `ResponseContent` | `string?` | See [Developer-friendly Debugging](#developer-friendly-debugging) |

You should always check `IsSuccess` after receiving a response. The `Error` object should always have a value when `IsSuccess` is `false`.

Additional properties on `IssuuResponse<TData>`:
| Property | Type | Notes |
|---|---|---|
| `Data` | `TData?` | Whatever data type you are expecting |
| `HasData` | `bool` | True, if valid data was deserialized from the response |

:::tip[`IsSuccess` and `HasData`]

It may not be enough simply to check `IsSuccess` when handling response payload data. The `HasData` value is a `null`-check on the `TData?` value.

Also note though, if you are expecting a collection of items, the `HasData` will simply check that the collection is not `null`, it will not check for the presence of items within the collection.

:::

### Meta

The `Meta` property of the response is a colletion of pagination-related properties:

| Property | Type | Notes |
|---|---|---|
| `Page` | `int` | The currently requested page |
| `PageSize` | `int` | The size of the requested page |
| `TotalItems` | `int` | The total number of items for the given request |
| `TotalPages` | `int` | The total number of pages for the given number of items |

### Rate Limiting

The `RateLimiting` property of the response is the current state of rate limiting for your authentication token. If you exceed rate limiting, the Issuu API may fail requests.

| Property | Type | Notes |
|---|---|---|
| `Remaining` | `int` | The number of API calls remaining until reset |
| `Limit` | `int` | The total number of API calls that can be made |
| `Reset` | `DateTimeOffset` | The date/time that the rate limit stats are reset |

:::warning[Rate Limiting]

The IssuuSDK will not handle throttling of requests based on the rate limiting values that are returned. If you are performing a high volume of operations against the Issuu API, you are responsible for throttling those requests yourself.

:::

### Links

The `Links` dictionary may contain additional links to related resources. The schema suggests these can be _anything_, but typical HATEOS-style links may be `self`, `next`, `prev`, but the IssuuSDK cannot guarantee these are present consistently.

### Errors

The `Error` property will contain any captured error information by the IssuuAPI:

| Property | Type | Notes |
|---|---|---|
| `Errors` | `Dictionary<string, string>?` | Specific field or data-level error messages |
| `Exception` | `Exception?` | The captured exception |
| `Message` | `string` | The main error message |

:::warning[Non-API errors]

If the IssuuSDK encounters issues that are not caused by the Issuu API, such as transient network faults, or an inability for the SDK to deserialize response data (perhaps for yet unsupported operations), these will also surface as a populated `Error` value.

:::

### Developer-friendly Debugging

The primary consumer of this SDK are developers, so the IssuuSDK has been designed to give as much information as possible to developers to aid in understanding any integration issues.

#### Request and Response Content

The `IssuuResponse` and `IssuuResponse<TData>` type contains useful properties for debugging requests and responses.

| Property | Type | Notes |
|---|---|---|
| `StatusCode` | `HttpStatusCode` | The HTTP status code of the response |
| `RateLimiting` | `RateLimiting?` | Information on rate limiting |
| `RequestMethod` | `HttpMethod` | The HTTP method used |
| `RequestUri` | `Uri` | The requested URI resource |
| `RequestContent` | `string?` | The captured HTTP request content |
| `ResponseContent` | `string?` | The captured HTTP response content |

By default, the `RequestContent` and `ResponseContent` properties will not be populated, these have to be enabled in your app configuration:

```json
{
  "Issuu": {
    "Token": "{token}",
    "CaptureRequestContent": true,
    "CaptureResponseContent": true
  }
}
```

Alternatively, you can explicitly enable them through `IssuuSettings`:

```
var settings = new IssuuSettings
{
    Token = "{token}",
    CaptureRequestContent = true,
    CaptureResponseContent = true
};
```