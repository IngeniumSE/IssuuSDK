---
sidebar_position: 1
tags: [Draft, Document]
---

# Getting a list of drafts

This page discusses how to read a list of drafts on Issuu

### Getting a list of drafts

:::tip[Issuu API]

This method corresponds to calling `HTTP GET https://api.issuu.com/v2/drafts`.<br />
API documentation: https://api.issuu.com/v2/reference/#get-/drafts

:::

To read a list of draft documents, we can use the `Drafts.GetDraftsAsync` operation. This operation will return a page of a set of documents.

```csharp
public async Task ReadDraftDocuments(int page = 1, int size = 10, CancellationToken ct)
{
    var response = await _client.Drafts.GetDraftsAsync(page, size ct);
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