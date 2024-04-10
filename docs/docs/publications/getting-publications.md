---
sidebar_position: 1
tags: [Publication, Document]
---

# Getting a list of publications

This page discusses how to read a list of publications on Issuu

### Getting a list of publications

:::tip[Issuu API]

This method corresponds to calling `HTTP GET https://api.issuu.com/v2/publications`.<br />
API documentation: https://api.issuu.com/v2/reference/#get-/publications

:::

To read a list of published documents, we can use the `Publications.GetPublicationsAsync` operation. This operation will return a page of a set of documents.

```csharp
public async Task ReadPublishedDocuments(int page = 1, int size = 10, CancellationToken ct)
{
    var response = await _client.Publications.GetPublicationsAsync(page, size ct);
    if (response.IsSuccess && response.Data is { Length: >0 })
    {
        var documents = response.Data;
        // Do something with the documents.
    }
}
```

If the response is successful, the `Meta` property should be populated with additional paging information. You can use this to determine if you can read any other pages of information.

:::tip[Paging Defaults]

The following defaults are used for `page` and `size` if you do not provide them:

| Parameter | Type | Default |
|---|---|---|
| `page` | `int` | 1 |
| `size` | `int` |  10 |

:::